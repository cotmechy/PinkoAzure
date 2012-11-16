using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.Interface;
using PinkoCommon.Utility;
using PinkoExpressionCommon;
using PinkoWorkerCommon.Utility;

namespace PinkoWorkerCommon.RoleHandlers
{
    /// <summary>
    /// Calc engine
    /// </summary>
    public class RoleCalculateSubsCalcExpressionHandler
    {
        /// <summary>
        /// Initialize and register role handler
        /// </summary>
        /// <returns></returns>
        public RoleCalculateSubsCalcExpressionHandler Register()
        {
            Debug.Assert(PinkoContainer.Registrations.Count(x => x.RegisteredType == typeof(InboundMessageReactiveListener<PinkoMsgCalculateExpression>)) == 1);
            Debug.Assert(PinkoContainer.Registrations.Count(x => x.RegisteredType == typeof(InboundMessageReactiveListener<PinkoMsgClientConnect>)) == 1);
            Debug.Assert(PinkoContainer.Registrations.Count(x => x.RegisteredType == typeof(InboundMessageReactiveListener<PinkoMsgClientTimeout>)) == 1);

            //
            // Handle: Client subscription
            //
            PinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpression>>()
                .Subscribers.Add(new Handlesubscriber<PinkoMsgCalculateExpression>()
                {
                    FilterCallback = (m, p) => p.MsgAction == PinkoMessageAction.ManagerSubscription,
                    Callback = (m, e) => ProcessSubscribe(m, e)
                });

            //
            // Handle: PinkoMsgClientConnect 
            //
            PinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgClientConnect>>()
                .Subscribers.Add(new Handlesubscriber<PinkoMsgClientConnect>()
                {
                    Callback = (m, e) => ProcessConnectionChange(m, e)
                });


            //
            // Handle: PinkoMsgClientConnect 
            //
            PinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgClientTimeout>>()
                .Subscribers.Add(new Handlesubscriber<PinkoMsgClientTimeout>()
                {
                    Callback = (m, e) => ProcessTimedOutClients(m, e)
                });


            //
            // TODO: Stop _calcTimer
            //

            // timer to re-run calculations in parallel mode
            _calcTimer = PinkoApplication.RunActionInTimer(PinkoConfiguration.RunCalcIntervalMs, RunParallelCalculation);

            return this;
        }

        /// <summary>
        /// Process real time data subscription
        /// </summary>
        public IBusMessageOutbound ProcessSubscribe(IBusMessageInbound envelop, PinkoMsgCalculateExpression expression)
        {
            var response = (IBusMessageOutbound)envelop;
            var resultMsg = new PinkoMsgCalculateExpressionResult().FromRequest(expression);

            response.Message = resultMsg;
            response.WebRoleId = expression.DataFeedIdentifier.WebRoleId;

            // Wait for calculation to end before processing standard request.
            // Avoid have to lock while performing parallel calculations.
            //
            // WARNING: Subscriber collection are NOT thread safe
            //
            using (CalculatingMutex.LockDisposible())
            {
                switch (expression.MsgAction)
                {
                    case PinkoMessageAction.ManagerSubscription:
                        {
                            //
                            // Typed expressions supported are double and double[]
                            //
                            switch (resultMsg.ResultType)
                            {
                                // Single double result
                                case PinkoCalculateExpressionDaoExtensions.ResultDouble:
                                    {
                                        SubscribersDouble.UpdateSubscriber(resultMsg, () => PinkoExpressionEngine.ParseAndCompile<double[]>(expression.GetExpression()));
                                    }
                                    break;

                                // 2-dim double[][]
                                case PinkoCalculateExpressionDaoExtensions.ResultDoubleSeries:
                                    {
                                        SubscribersDoubleDouble.UpdateSubscriber(resultMsg, () => PinkoExpressionEngine.ParseAndCompile<double[][]>(expression.GetExpression()));
                                    }
                                    break;

                                // Invalid result type
                                default:
                                    {
                                        resultMsg.ResultType = PinkoErrorCode.FormulaTypeNotSupported;
                                        response.ErrorCode = PinkoErrorCode.FormulaTypeNotSupported;
                                        response.ErrorDescription = PinkoMessagesText.FormulaNotSupported;
                                        response.ErrorSystem = PinkoMessagesText.FormulaNotSupported;
                                    }
                                    break;
                            }
                        }
                        break;

                    //case PinkoMessageAction.ReconnectSubscribe:
                    //    {
                    //        // TODO: Add unsubscribe
                    //    }
                    //    break;

                    default:
                        {
                            resultMsg.ResultType = PinkoErrorCode.ActionNotSupported;
                            response.ErrorCode = PinkoErrorCode.ActionNotSupported;
                            response.ErrorDescription = PinkoMessagesText.ActionNotSupported;
                            response.ErrorSystem = PinkoMessagesText.ActionNotSupported;
                        }
                        break;
                }
            }


            return response;
        }

        /// <summary>
        /// Find clients subscriptions that timed out and remove them
        /// </summary>
        public IBusMessageOutbound ProcessTimedOutClients(IBusMessageInbound envelop, PinkoMsgClientTimeout clientTimeout)
        {
            using (CalculatingMutex.LockDisposible())
            {
                Trace.TraceInformation("Client Timeout. Unsubscribing: {0}", clientTimeout.DataFeedIdentifier.Verbose());
                SubscribersDouble.RemoveIdentifier(clientTimeout.DataFeedIdentifier);
            }

            return null;
        }


        /// <summary>
        /// Process real time data subscription
        /// </summary>
        public IBusMessageOutbound ProcessConnectionChange(IBusMessageInbound envelop, PinkoMsgClientConnect clientConnect)
        {
            using (CalculatingMutex.LockDisposible())
            {
                Trace.TraceInformation("Client Timeout: {0}", clientConnect.DataFeedIdentifier.Verbose());
                SubscribersDouble.UpdateIdentifier(clientConnect.DataFeedIdentifier);
            }
            return null;
        }


        /// <summary>
        /// run parallel calculation
        /// </summary>
        public void RunParallelCalculation()
        {
            var typeCalculators = new[]
                {
                    new Tuple<IPinkoCalculator, object>((IPinkoCalculator) PinkoContainer.Resolve<PinkoCalculator<double[]>>(), SubscribersDouble),
                    new Tuple<IPinkoCalculator, object>((IPinkoCalculator) PinkoContainer.Resolve<PinkoCalculator<double[][]>>(), SubscribersDoubleDouble)
                };

            // Wait for calculation to end before processing standard request.
            // Avoid have to lock while performing parallel calculations.
            using (CalculatingMutex.LockDisposible())
                PinkoApplication.ForEachParallel(typeCalculators, x => x.Item1.Caculate(x.Item2));
        }

        /// <summary>
        /// Collection holding double[] subscriptions
        /// </summary>
        public PinkoExpressionSubscribers<double[]> SubscribersDouble = new PinkoExpressionSubscribers<double[]>();

        /// <summary>
        /// Collection holding double[][] subscriptions
        /// </summary>
        public PinkoExpressionSubscribers<double[][]> SubscribersDoubleDouble = new PinkoExpressionSubscribers<double[][]>();

        /// <summary>
        /// Event when Role is calculating
        /// </summary>
        public PinkoMutex CalculatingMutex = new PinkoMutex();

        /// <summary>
        /// timer 
        /// </summary>
        private IDisposable _calcTimer;

        /// <summary>
        /// IPinkoMarketEnvManager
        /// </summary>
        [Dependency]
        public IPinkoMarketEnvManager PinkoMarketEnvManager { get; set; }

        /// <summary>
        /// IPinkoExpressionEngine
        /// </summary>
        [Dependency]
        public IPinkoExpressionEngine PinkoExpressionEngine { get; set; }


        /// <summary>
        /// IPinkoConfiguration
        /// </summary>
        [Dependency]
        public IPinkoConfiguration PinkoConfiguration { private get; set; }

        /// <summary>
        /// IUnityContainer
        /// </summary>
        [Dependency]
        public IUnityContainer PinkoContainer { get; set; }


        /// <summary>
        /// PinkoApplication
        /// </summary>
        [Dependency]
        public IPinkoApplication PinkoApplication { get; set; }
    }
}
