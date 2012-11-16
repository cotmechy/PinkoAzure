using System.Diagnostics;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.Interface;
using PinkoExpressionCommon;
using PinkoWorkerCommon.Extensions;

namespace PinkoWorkerCommon.RoleHandlers
{
    public class RoleCalculateExpressionSnapshotHandler
    {
        /// <summary>
        /// Initialize and register role handler
        /// </summary>
        /// <returns></returns>
        public RoleCalculateExpressionSnapshotHandler Register()
        {
            PinkoMsgCalculateExpressionReactiveListener.Subscribers.Add(new Handlesubscriber<PinkoMsgCalculateExpression>()
            {
                FilterCallback = (m, p) => p.MsgAction == PinkoMessageAction.UserSnapshot,
                Callback = (m, e) => ProcessSubscribe(m, e)
            });

            return this;
        }

        /// <summary>
        /// Process real time data subscription
        /// </summary>
        public IBusMessageOutbound ProcessSubscribe(IBusMessageInbound envelop, PinkoMsgCalculateExpression expression)
        {
            var response = (IBusMessageOutbound)envelop;
            var marketEnv = PinkoMarketEnvManager.GetMarketEnv(expression.DataFeedIdentifier.MaketEnvId);
            var resultMsg = new PinkoMsgCalculateExpressionResult().FromRequest(expression);

            response.Message = resultMsg;
            response.WebRoleId = expression.DataFeedIdentifier.WebRoleId;

            var finalFormExp = expression.GetExpression();

            Trace.TraceInformation("Calculating: {0}", expression.Verbose());

            //
            // Typed expressions supported are double and double[]
            //
            switch (resultMsg.ResultType)
            {
                // Single double result
                case PinkoCalculateExpressionDaoExtensions.ResultDouble:
                    {
                        var complExp = PinkoExpressionEngine.ParseAndCompile<double[]>(finalFormExp);
                        var results = PinkoExpressionEngine.Invoke(marketEnv, complExp);

                        if (null != results)
                            resultMsg.ResultsTupples = expression.ExpressionFormulas.GetTupleResult(results);
                    }
                    break;

                // 2-dim double[][]
                case PinkoCalculateExpressionDaoExtensions.ResultDoubleSeries:
                    {
                        var complExp = PinkoExpressionEngine.ParseAndCompile<double[][]>(finalFormExp);
                        var results = PinkoExpressionEngine.Invoke(marketEnv, complExp);

                        if (null != results)
                            resultMsg.ResultsTupples = expression.ExpressionFormulas.GetTupleResult(results);
                    }
                    break;

                default:
                    {
                        resultMsg.ResultType = PinkoErrorCode.FormulaTypeNotSupported;
                        response.ErrorCode = PinkoErrorCode.FormulaTypeNotSupported;
                        response.ErrorDescription = PinkoMessagesText.FormulaNotSupported;
                        response.ErrorSystem = PinkoMessagesText.FormulaNotSupported;
                    }
                    break;
            }

            return response;
        }


        /// <summary>
        /// IPinkoMarketEnvManager
        /// </summary>
        [Dependency]
        public IPinkoMarketEnvManager PinkoMarketEnvManager { get; set; }


        /// <summary>
        /// BusListenerUserSubscriberExpressionHandler
        /// </summary>
        [Dependency]
        public InboundMessageReactiveListener<PinkoMsgCalculateExpression> PinkoMsgCalculateExpressionReactiveListener { get; set; }

        /// <summary>
        /// IPinkoExpressionEngine
        /// </summary>
        [Dependency]
        public IPinkoExpressionEngine PinkoExpressionEngine { get; set; }
    }
}
