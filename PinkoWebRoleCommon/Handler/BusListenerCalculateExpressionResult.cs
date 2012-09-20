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
using PinkoWebRoleCommon.Interface;

namespace PinkoWebRoleCommon.Handler
{
    /// <summary>
    /// Calculate an expression request in server 
    /// </summary>
    public class BusListenerCalculateExpressionResult : InboundMessageHandler<PinkoMsgCalculateExpressionResult>
    {
        /// <summary>
        /// Constructor - BusListenerCalculateExpressionResult 
        /// </summary>
        public BusListenerCalculateExpressionResult()
        {
            ProcessRequestHandler = ProcessRequest;
        }

        /// <summary>
        /// Set handler 
        /// </summary>
        public IBusMessageOutbound ProcessRequest(IBusMessageInbound msg, PinkoMsgCalculateExpressionResult result)
        {
            Trace.TraceInformation("(BusListenerCalculateExpressionResult): {0}", msg.Verbose());
            Trace.TraceInformation("(BusListenerCalculateExpressionResult): {0}", result.Verbose());

            // Send response via SignalR.  
            // 
            // consider:
            //  - client context
            //  - direct client response

            TryCatch.RunInTry(() =>
                {
                    if (msg.ErrorCode != PinkoErrorCode.Success)
                        WebRoleSignalRManager.PinkoSingalHubContext.Clients[result.DataFeedIdentifier.SignalRId]
                            .expressionResponseError(result.DataFeedIdentifier.ClientCtx, result.ExpressionFormula, result.ResultValue, msg.ErrorCode, msg.ErrorDescription);
                    else
                        WebRoleSignalRManager.PinkoSingalHubContext.Clients[result.DataFeedIdentifier.SignalRId]
                            .expressionResponse(result.DataFeedIdentifier.ClientCtx, result.ExpressionFormula, result.ResultType, result.ResultValue);
                });
            return null;
        }

        /// <summary>
        /// IUnityContainer
        /// </summary>
        [Dependency]
        public IWebRoleSignalRManager WebRoleSignalRManager { get; set; }
    }
}
