using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PinkDao;
using PinkoWorkerCommon.BaseMessageHandlers;
using PinkoWorkerCommon.Interface;

namespace PinkoServices.Handlers
{
    public class HandlerPinkoRoleHeartbeat : InboundMessageHandler<PinkoRoleHeartbeat>
    {
        /// <summary>
        /// Set handler 
        /// </summary>
        public override void HandlerAction(IBusMessageInbound msg, PinkoRoleHeartbeat type)
        {
            Trace.TraceInformation("HandlerPinkoRoleHeartbeat: HandlerAction: {0} - ResponderMachine: {1} - ResponderDateTime: {2}", msg.Verbose(), type.ResponderMachine, type.ResponderDateTime);
        }
    }
}
