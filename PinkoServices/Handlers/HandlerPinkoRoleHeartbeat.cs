using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PinkDao;
using PinkoWebRoleCommon.HubModels;
using PinkoWorkerCommon.BaseMessageHandlers;
using PinkoWorkerCommon.Interface;

namespace PinkoServices.Handlers
{
    public class HandlerPinkoRoleHeartbeat : InboundMessageHandler<PinkoRoleHeartbeatHub>
    {
        /// <summary>
        /// Set handler 
        /// </summary>
        public override void HandlerAction(IBusMessageInbound msg, PinkoRoleHeartbeatHub type)
        {
            Trace.TraceInformation("HandlerPinkoRoleHeartbeat: HandlerAction: {0} - ResponderMachine: {1} - ResponderDateTime: {2}", msg.Verbose(), type.ResponderMachine, type.ResponderDateTime);
        }
    }
}
