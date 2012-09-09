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
    public class BusListenerCalculateExpressionResult : InboundMessageHandler<PinkoCalculateExpressionResult>
    {
        /// <summary>
        /// Set handler 
        /// </summary>
        public override IBusMessageOutbound ProcessRequest(IBusMessageInbound msg, PinkoCalculateExpressionResult result)
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
                        WebRoleSignalRManager.PinkoSingalHubContext.Clients
                            .expressionResponseError(result.ClientCtx, result.ResultValue, msg.ErrorCode, msg.ErrorDescription);
                    else
                        WebRoleSignalRManager.PinkoSingalHubContext.Clients
                            .expressionResponse(result.ClientCtx, result.ResultType, result.ResultValue);
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
