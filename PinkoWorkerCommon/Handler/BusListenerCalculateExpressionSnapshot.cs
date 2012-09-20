using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.Interface;
using PinkoCommon.Utility;
using PinkoExpressionCommon;

namespace PinkoWorkerCommon.Handler
{
    /// <summary>
    /// Calculate an expression request in server 
    /// </summary>
    public class BusListenerCalculateExpressionSnapshot : InboundMessageHandler<PinkoMsgCalculateExpression>
    {
        /// <summary>
        /// Constructor - BusListenerCalculateExpression 
        /// </summary>
        public BusListenerCalculateExpressionSnapshot()
        {
            ProcessRequestHandler = ProcessRequest;
        }

        /// <summary>
        /// Handle adhoc request
        /// </summary>
        public IBusMessageOutbound ProcessRequest(IBusMessageInbound msg, PinkoMsgCalculateExpression expression)
        {
            var outMsg = (IBusMessageOutbound)msg;
            var marketEnv = PinkoMarketEnvManager.GetMarketEnv(expression.DataFeedIdentifier.MaketEnvId);
            var result = new PinkoMsgCalculateExpressionResult().FromRequest(expression);
            
            outMsg.Message = result;
            outMsg.WebRoleId = expression.DataFeedIdentifier.WebRoleId;
            outMsg.ErrorCode = PinkoErrorCode.Success;

            var ex = TryCatch.RunInTry(() =>
            {
                // Must put braces around it to process
                var finalFormExp = string.Format("{0} {1} {2}", "{", result.ExpressionFormula, "}");

                //
                // Typed expressions supported are double and double[]
                //
                switch (result.ResultType)
                {
                    case PinkoCalculateExpressionDaoExtensions.ResultDouble:
                        {
                            var complExp = PinkoExpressionEngine.ParseAndCompile<double>(finalFormExp);
                            result.ResultValue = PinkoExpressionEngine.Invoke(marketEnv, complExp);
                        }
                        break;

                    case PinkoCalculateExpressionDaoExtensions.ResultDoubleSeries:
                        {
                            var complExp = PinkoExpressionEngine.ParseAndCompile<double[]>(finalFormExp);
                            result.ResultValue = PinkoExpressionEngine.Invoke(marketEnv, complExp);
                        }
                        break;

                    default:
                        {
                            result.ResultValue = PinkoMessagesText.Error;
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
                result.ResultValue = PinkoMessagesText.Error;
            }

            Trace.TraceInformation("BusListenerCalculateExpression: {0}", outMsg.Verbose());
            Trace.TraceInformation("BusListenerCalculateExpression: {0}", result.Verbose());

            return outMsg;
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
