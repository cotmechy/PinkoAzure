using System;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.Interface;
using PinkoCommon.Utility;
using PinkoExpressionCommon;

namespace PinkoCalcEngineWorker.Handlers
{
    /// <summary>
    /// Calcualte an expresion request
    /// </summary>
    public class HandleCalculateExpression : InboundMessageHandler<PinkoCalculateExpressionDao>
    {
        /// <summary>
        /// Handle adhoc request
        /// </summary>
        public override IBusMessageOutbound ProcessRequest(IBusMessageInbound msg, PinkoCalculateExpressionDao expression)
        {
            var outMsg = (IBusMessageOutbound)msg;
            var marketEnv = PinkoMarketEnvManager.GetMarketEnv(expression.MaketEnvId);

            var ex = TryCatch.RunInTry(() =>
            {
                //
                // Typed expressions supported are double and double[]
                //
                switch (expression.ResultType)
                {
                    case PinkoCalculateExpressionDaoExtensions.ResultDouble:
                        {
                            var complExp = PinkoExpressionEngine.ParseAndCompile<double>(expression.ExpressionFormula);
                            expression.ResultValue = PinkoExpressionEngine.Invoke(marketEnv, complExp);
                        }
                        break;

                    case PinkoCalculateExpressionDaoExtensions.ResultDoubleSeries:
                        {
                            var complExp = PinkoExpressionEngine.ParseAndCompile<double[]>(expression.ExpressionFormula);
                            expression.ResultValue = PinkoExpressionEngine.Invoke(marketEnv, complExp);
                        }
                        break;

                    default:
                        {
                            expression.ResultValue = PinkoMessagesText.Error;
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
                expression.ResultValue = PinkoMessagesText.Error;
            }

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
