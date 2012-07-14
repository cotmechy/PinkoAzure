using System.Diagnostics;
using PinkoWorkerCommon.BaseMessageHandlers;
using PinkoWorkerCommon.Interface;

namespace PinkoServices.Handlers
{
    /// <summary>
    /// Handler string response
    /// </summary>
    public class HandlerStringMessage : InboundMessageHandler<string>
    {
        /// <summary>
        /// Handle message
        /// </summary>
        public override void HandlerAction(IBusMessageInbound msg, string typedMsg)
        {
            Trace.TraceInformation("{0}: {1} - {2}", GetType(), msg.Verbose(), typedMsg);
        }
    }
}