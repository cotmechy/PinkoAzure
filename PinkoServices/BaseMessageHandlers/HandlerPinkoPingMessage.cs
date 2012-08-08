using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using PinkDao;
using PinkoWorkerCommon.Interface;

namespace PinkoServices.BaseMessageHandlers
{
    /// <summary>
    /// Handler Ping message response
    /// </summary>
    public class HandlerPinkoPingMessage : InboundMessageHandler<PinkoPingMessage>
    {
        /// <summary>
        /// Handle message
        /// </summary>
        public override void HandlerAction(IBusMessageInbound msg, PinkoPingMessage typedMsg)
        {
            typedMsg.ResponderMachine = PinkoApplication.MachineName;
            typedMsg.ResponderDateTime = DateTime.Now;

            msg.Message = typedMsg;

            ReplyQueue.Publish((IBusMessageOutbound) msg);
        }
    }
}
