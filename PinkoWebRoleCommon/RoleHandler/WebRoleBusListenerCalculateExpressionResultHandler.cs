using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.Extension;
using PinkoCommon.Interface;
using PinkoCommon.Utility;
using PinkoWebRoleCommon.Interface;

namespace PinkoWebRoleCommon.RoleHandler
{
    public class WebRoleBusListenerCalculateExpressionResultHandler
    {
        /// <summary>
        /// Initialize and register role handler
        /// </summary>
        /// <returns></returns>
        public WebRoleBusListenerCalculateExpressionResultHandler Register()
        {
            PinkoMsgCalculateExpressionResultRouter.Subscribers.Add(new Handlesubscriber<PinkoMsgCalculateExpressionResult>()
            {
                FilterCallback = (m, p) => true,
                Callback = (m, e) => ProcessSubscribe(m, e)
            });

            return this;
        }

        /// <summary>
        /// Process real time data subscription
        /// </summary>
        public IBusMessageOutbound ProcessSubscribe(IBusMessageInbound envelop, PinkoMsgCalculateExpressionResult result)
        {
            var msg = (IBusMessageOutbound)envelop;

            //Trace.TraceInformation("(RoleBusListenerCalculateExpressionResultHandler): {0}", msg.Verbose());
            //Trace.TraceInformation("(RoleBusListenerCalculateExpressionResultHandler): {0}", result.Verbose());

            // Send response via SignalR.  
            // 
            // consider:
            //  - client context
            //  - direct client response

            //TryCatch.RunInTry(() =>
            //{
                if (msg.ErrorCode != PinkoErrorCode.Success)
                {
                    WebRoleSignalRManager.PinkoSingalHubContext.Clients[result.DataFeedIdentifier.SignalRId]
                        .expressionResponseError(result.DataFeedIdentifier.ClientCtx, result.DataFeedIdentifier.SubscribtionId, "#ERROR", msg.ErrorCode, msg.ErrorDescription);
                }
                else
                {
                    result.ResultsTupples.ForEach(respExp =>
                        WebRoleSignalRManager.PinkoSingalHubContext.Clients[result.DataFeedIdentifier.SignalRId]
                            .expressionResponse(result.DataFeedIdentifier.ClientCtx, respExp.PointSeries, result.ResultType, respExp.OriginalFormula.RuntimeId)
                        );
                }
            //});

            // no response to server
            return null;
        }


        /// <summary>
        /// IUnityContainer
        /// </summary>
        [Dependency]
        public IWebRoleSignalRManager WebRoleSignalRManager { get; set; }

        /// <summary>
        /// BusListenerUserSubscriberExpressionHandler
        /// </summary>
        [Dependency]
        public InboundMessageReactiveListener<PinkoMsgCalculateExpressionResult> PinkoMsgCalculateExpressionResultRouter { get; set; }
    }
}
