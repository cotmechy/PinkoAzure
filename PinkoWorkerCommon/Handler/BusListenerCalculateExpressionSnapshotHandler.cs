using System;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.Interface;
using PinkoCommon.Utility;
using PinkoExpressionCommon;
using PinkoWorkerCommon.Extensions;

namespace PinkoWorkerCommon.Handler
{
    /// <summary>
    /// Calculate an expression request in server 
    /// </summary>
    public class BusListenerCalculateExpressionSnapshotHandler : InboundMessageHandler<PinkoMsgCalculateExpression>
    {
        /// <summary>
        /// Handle adhoc request
        /// </summary>
        public override IBusMessageOutbound ProcessRequest(IBusMessageInbound msg, PinkoMsgCalculateExpression expression)
        {
            var outMsg = (IBusMessageOutbound)msg;
            var marketEnv = PinkoMarketEnvManager.GetMarketEnv(expression.DataFeedIdentifier.MaketEnvId);
            var resultMsg = new PinkoMsgCalculateExpressionResult().FromRequest(expression);
            
            outMsg.Message = resultMsg;
            outMsg.WebRoleId = expression.DataFeedIdentifier.WebRoleId;
            outMsg.ErrorCode = PinkoErrorCode.Success;

            var ex = TryCatch.RunInTry(() =>
            {
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
                                resultMsg.ResultsTupple = expression.ExpressionFormulas.GetTupleResult(results);
                        }
                        break;

                        // 2-dim double[][]
                    case PinkoCalculateExpressionDaoExtensions.ResultDoubleSeries:
                        {
                            var complExp = PinkoExpressionEngine.ParseAndCompile<double[][]>(finalFormExp);
                            var results = PinkoExpressionEngine.Invoke(marketEnv, complExp);

                            if (null != results)
                                resultMsg.ResultsTupple = expression.ExpressionFormulas.GetTupleResult(results);
                        }
                        break;

                    default:
                        {
                            resultMsg.ResultType = PinkoErrorCode.FormulaTypeNotSupported;
                            outMsg.ErrorCode = PinkoErrorCode.FormulaTypeNotSupported;
                            outMsg.ErrorDescription = PinkoMessagesText.FormulasSupported;
                            outMsg.ErrorSystem = PinkoMessagesText.FormulasSupported;
                        }
                        break;
                }
            });

            // Exception ? 
            if (null != ex)
            {
                outMsg.ErrorCode = PinkoErrorCode.UnexpectedException;
                outMsg.ErrorDescription = ex.Message;
                outMsg.ErrorSystem = ex.ToString();
                resultMsg.ResultType = PinkoErrorCode.UnexpectedException;
            }

            Trace.TraceInformation("BusListenerCalculateExpression: {0}", outMsg.Verbose());
            Trace.TraceInformation("BusListenerCalculateExpression: {0}", resultMsg.Verbose());

            return outMsg;
        }

        /// <summary>
        /// Allow only snapshot based messages
        /// </summary>
        protected override bool FilterIncomingMsg(System.Tuple<IBusMessageInbound, PinkoMsgCalculateExpression> msg)
        {
            return msg.Item2.MsgAction == PinkoMessageAction.UserSnapshot;
        }

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
