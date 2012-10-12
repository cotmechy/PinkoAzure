using System.Diagnostics;
using System.Threading;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.Interface;
using PinkoCommon.Utility;
using PinkoExpressionCommon;
using PinkoWorkerCommon.Utility;

namespace PinkoWorkerCommon.Handler
{
    /// <summary>
    /// Handle calc engine subscription formulas. Usually sent by BusListenerUserSubscriberExpressionHandler.
    /// </summary>
    public class BusListenerCalculateSubsCalcExpressionHandler : InboundMessageHandler<PinkoMsgCalculateExpression>
    {
        /// <summary>
        /// Handler
        /// </summary>
        public override IBusMessageOutbound ProcessRequest(IBusMessageInbound msg, PinkoMsgCalculateExpression expression)
        {
            var response = (IBusMessageOutbound)msg;
            var resultMsg = new PinkoMsgCalculateExpressionResult().FromRequest(expression);

            response.Message = resultMsg;
            response.WebRoleId = expression.DataFeedIdentifier.WebRoleId;
            response.ErrorCode = PinkoErrorCode.Success;

            var ex = TryCatch.RunInTry(() =>
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
                                            SubscribersDouble.UpdateSubscriber(resultMsg, PinkoExpressionEngine.ParseAndCompile<double[]>(expression.GetExpression()) );
                                        }
                                        break;

                                    // 2-dim double[][]
                                    case PinkoCalculateExpressionDaoExtensions.ResultDoubleSeries:
                                        {
                                            SubscribersDoubleDouble.UpdateSubscriber(resultMsg, PinkoExpressionEngine.ParseAndCompile<double[][]>(expression.GetExpression()));
                                        }
                                        break;
                                }
                            }
                            break;

                        case PinkoMessageAction.ManagerUnsubscribe:
                            {
                                // TODO: 
                            }
                            break;

                        default:
                            {
                                resultMsg.ResultType = PinkoErrorCode.FormulaTypeNotSupported;
                                response.ErrorCode = PinkoErrorCode.FormulaTypeNotSupported;
                                response.ErrorDescription = PinkoMessagesText.FormulasSupported;
                                response.ErrorSystem = PinkoMessagesText.FormulasSupported;
                            }
                            break;
                    }
                });

            // Exception ? 
            if (null != ex)
            {
                response.ErrorCode = PinkoErrorCode.UnexpectedException;
                response.ErrorDescription = ex.Message;
                response.ErrorSystem = ex.ToString();
                resultMsg.ResultType = PinkoErrorCode.UnexpectedException;
            }

            Trace.TraceInformation("BusListenerCalculateSubsCalcExpressionHandler: {0}", response.Verbose());
            Trace.TraceInformation("BusListenerCalculateSubsCalcExpressionHandler: {0}", resultMsg.Verbose());

            return response;
        }


        /// <summary>
        /// Filter PinkoMsgCalculateExpression actions
        /// </summary>
        protected override bool FilterIncomingMsg(System.Tuple<IBusMessageInbound, PinkoMsgCalculateExpression> msg)
        {
            return msg.Item2.MsgAction == PinkoMessageAction.ManagerSubscription;
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
        public PinkoManualResetEvent CalculatingEv = new PinkoManualResetEvent(false);

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
    }
}
