//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using PinkDao;
//using PinkoCommon.BaseMessageHandlers;
//using PinkoCommon.Interface;

//namespace PinkoWorkerCommon.Handler
//{
//    ///// <summary>
//    ///// Handler Ping message response
//    ///// </summary>
//    //public class BusListenerPinkoPingMessage : InboundMessageHandler<PinkoMsgPing>
//    //{
//    //    /// <summary>
//    //    /// Handle message
//    //    /// </summary>
//    //    public override IBusMessageOutbound ProcessRequest(IBusMessageInbound msg, PinkoMsgPing typedMsg)
//    //    {
//    //        typedMsg.ResponderMachine = PinkoApplication.MachineName;
//    //        typedMsg.ResponderDateTime = DateTime.Now;

//    //        msg.Message = typedMsg;

//    //        // Respond to ping message
//    //        return (IBusMessageOutbound)msg;
//    //    }
//    //}
//}
