//using System;
//using System.Diagnostics;
//using System.Reactive.Concurrency;
//using System.Reactive.Linq;
//using System.Threading;
//using Microsoft.Practices.Unity;
//using PinkDao;
//using PinkoCommon;
//using PinkoCommon.BaseMessageHandlers;
//using PinkoCommon.Interface;
//using PinkoCommon.Utility;
//using PinkoExpressionCommon;
//using PinkoWorkerCommon.Utility;

//namespace PinkoWorkerCommon.Handler
//{
//    /// <summary>
//    /// Handle calc engine subscription formulas. Usually sent by BusListenerUserSubscriberExpressionHandler.
//    /// </summary>
//    public class BusListenerCalculateSubsCalcExpressionHandler : InboundMessageHandler<PinkoMsgCalculateExpression>
//    {
//        /// <summary>
//        /// Register Handler with RxMesagebus. Subscribe for incoming messages to process in the Rx Bus 
//        /// </summary>
//        public override InboundMessageHandler<PinkoMsgCalculateExpression> Register()
//        {
//            //
//            // TODO: Stop timer 
//            //
//            _calcTimer = PinkoApplication.RunActionInTimer(PinkoConfiguration.RunCalcIntervalMs, RunParallelCalculation);
            
//            return base.Register();
//        }


//        ///// <summary>
//        ///// Connect to topics 
//        ///// </summary>
//        //public override void ConnectToTopics()
//        //{
//        //    ConnectToStandardMessageTopics(PinkoConfiguration.PinkoMessageBusToWorkerAllSubscriptionManagerTopic, PinkoConfiguration.PinkoMessageBusToWorkerSubscriptionManagerTopic);
//        //}

//        /// <summary>
//        /// Handler
//        /// </summary>
//        public IBusMessageOutbound ProcessRequest(IBusMessageInbound msg, PinkoMsgCalculateExpression expression)
//        {
//            var response = (IBusMessageOutbound)msg;
//            var resultMsg = new PinkoMsgCalculateExpressionResult().FromRequest(expression);

//            response.Message = resultMsg;
//            response.WebRoleId = expression.DataFeedIdentifier.WebRoleId;
//            response.ErrorCode = PinkoErrorCode.Success;

//            var ex = TryCatch.RunInTry(() =>
//                {
//                    // Wait for calculation to end before processing standard request.
//                    // Avoid have to lock while performing parallel calculations.
//                    //
//                    // WARNING: Subscriber collection are NOT thread safe
//                    //
//                    using (CalculatingMutex.LockDisposible())
//                    {
//                        switch (expression.MsgAction)
//                        {
//                            case PinkoMessageAction.ManagerSubscription:
//                                {
//                                    //
//                                    // Typed expressions supported are double and double[]
//                                    //
//                                    switch (resultMsg.ResultType)
//                                    {
//                                        // Single double result
//                                        case PinkoCalculateExpressionDaoExtensions.ResultDouble:
//                                            {
//                                                SubscribersDouble.UpdateSubscriber(resultMsg, PinkoExpressionEngine.ParseAndCompile<double[]>(expression.GetExpression()));
//                                            }
//                                            break;

//                                        // 2-dim double[][]
//                                        case PinkoCalculateExpressionDaoExtensions.ResultDoubleSeries:
//                                            {
//                                                SubscribersDoubleDouble.UpdateSubscriber(resultMsg, PinkoExpressionEngine.ParseAndCompile<double[][]>(expression.GetExpression()));
//                                            }
//                                            break;

//                                        // Invalid result type
//                                        default:
//                                            {
//                                                resultMsg.ResultType = PinkoErrorCode.FormulaTypeNotSupported;
//                                                response.ErrorCode = PinkoErrorCode.FormulaTypeNotSupported;
//                                                response.ErrorDescription = PinkoMessagesText.FormulaNotSupported;
//                                                response.ErrorSystem = PinkoMessagesText.FormulaNotSupported;
//                                            }
//                                            break;
//                                    }
//                                }
//                                break;

//                            case PinkoMessageAction.ManagerUnsubscribe:
//                                {
//                                    // TODO: Add unsubscribe
//                                }
//                                break;

//                            default:
//                                {
//                                    resultMsg.ResultType = PinkoErrorCode.ActionNotSupported;
//                                    response.ErrorCode = PinkoErrorCode.ActionNotSupported;
//                                    response.ErrorDescription = PinkoMessagesText.ActionNotSupported;
//                                    response.ErrorSystem = PinkoMessagesText.ActionNotSupported;
//                                }
//                                break;
//                        }
//                    }
//                });

//            // Exception ? 
//            if (null != ex)
//            {
//                // Must unsubscribe if any issues
//                SubscribersDouble.RemoveSubscriber(resultMsg);

//                response.ErrorCode = PinkoErrorCode.UnexpectedException;
//                response.ErrorDescription = ex.Message;
//                response.ErrorSystem = ex.ToString();
//                resultMsg.ResultType = PinkoErrorCode.UnexpectedException;
//            }

//            Trace.TraceInformation("BusListenerCalculateSubsCalcExpressionHandler: {0}", response.Verbose());
//            Trace.TraceInformation("BusListenerCalculateSubsCalcExpressionHandler: {0}", resultMsg.Verbose());

//            return response;
//        }

//        /// <summary>
//        /// run parallel calculation
//        /// </summary>
//        public void RunParallelCalculation()
//        {
//            var typeCalculators = new[]
//                {
//                    new Tuple<IPinkoCalculator, object>((IPinkoCalculator) PinkoContainer.Resolve<PinkoCalculator<double[]>>(), SubscribersDouble),
//                    new Tuple<IPinkoCalculator, object>((IPinkoCalculator) PinkoContainer.Resolve<PinkoCalculator<double[][]>>(), SubscribersDoubleDouble)
//                };

//            // Wait for calculation to end before processing standard request.
//            // Avoid have to lock while performing parallel calculations.
//            using (CalculatingMutex.LockDisposible())
//                PinkoApplication.ForEachParallel(typeCalculators, x => x.Item1.Caculate(x.Item2));
//        }

//        /// <summary>
//        /// Filter PinkoMsgCalculateExpression actions
//        /// </summary>
//        protected bool FilterIncomingMsg(System.Tuple<IBusMessageInbound, PinkoMsgCalculateExpression> msg)
//        {
//            return msg.Item2.MsgAction == PinkoMessageAction.ManagerSubscription;
//        }

//        /// <summary>
//        /// Collection holding double[] subscriptions
//        /// </summary>
//        public PinkoExpressionSubscribers<double[]> SubscribersDouble = new PinkoExpressionSubscribers<double[]>();

//        /// <summary>
//        /// Collection holding double[][] subscriptions
//        /// </summary>
//        public PinkoExpressionSubscribers<double[][]> SubscribersDoubleDouble = new PinkoExpressionSubscribers<double[][]>();

//        /// <summary>
//        /// Event when Role is calculating
//        /// </summary>
//        public PinkoMutex CalculatingMutex = new PinkoMutex();

//        /// <summary>
//        /// timer 
//        /// </summary>
//        private IDisposable _calcTimer;

//        /// <summary>
//        /// IPinkoMarketEnvManager
//        /// </summary>
//        [Dependency]
//        public IPinkoMarketEnvManager PinkoMarketEnvManager { get; set; }

//        /// <summary>
//        /// IPinkoExpressionEngine
//        /// </summary>
//        [Dependency]
//        public IPinkoExpressionEngine PinkoExpressionEngine { get; set; }


//        /// <summary>
//        /// IPinkoConfiguration
//        /// </summary>
//        [Dependency]
//        public IPinkoConfiguration PinkoConfiguration { private get; set; }

//    }
//}
