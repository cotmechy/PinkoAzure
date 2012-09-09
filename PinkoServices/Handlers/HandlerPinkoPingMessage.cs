using System;
using PinkDao;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.Interface;

namespace PinkoServices.Handlers
{
    ///// <summary>
    ///// Handler Ping message response
    ///// </summary>
    //public class HandlerPinkoPingMessage : InboundMessageHandler<PinkoPingMessage>
    //{
    //    /// <summary>
    //    /// Handle message
    //    /// </summary>
    //    public override IBusMessageOutbound ProcessRequest(IBusMessageInbound msg, PinkoPingMessage typedMsg)
    //    {
    //        typedMsg.ResponderMachine = PinkoApplication.MachineName;
    //        typedMsg.ResponderDateTime = DateTime.Now;

    //        msg.Message = typedMsg;

    //        // Respond to ping message
    //        return (IBusMessageOutbound) msg;
    //    }
    //}
}
