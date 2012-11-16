//using System;
//using Microsoft.Practices.Unity;
//using PinkDao;
//using PinkoCommon.BaseMessageHandlers;
//using PinkoCommon.Interface;
//using PinkoExpressionCommon;

//namespace PinkoCommon
//{
//    /// <summary>
//    /// Handler Ping message response
//    /// </summary>
//    public class BusListenerPinkoPingMessage 
//    {
//        /// <summary>
//        /// Handle message
//        /// </summary>
//        public IBusMessageOutbound ProcessRequest(IBusMessageInbound msg, PinkoMsgPing typedMsg)
//        {
//            typedMsg.ResponderMachine = PinkoApplication.MachineName;
//            typedMsg.ResponderDateTime = DateTime.Now;

//            msg.Message = typedMsg;

//            // Respond to ping message
//            return (IBusMessageOutbound)msg;
//        }

//        /// <summary>
//        /// Initialize and register role handler
//        /// </summary>
//        /// <returns></returns>
//        public RoleCalculateExpressionSnapshotHandler Register()
//        {
//            PinkoMsgCalculateExpressionRouter.Subscribers.Add(new Handlesubscriber<PinkoMsgCalculateExpression>()
//            {
//                FilterCallback = (m, p) => p.MsgAction == PinkoMessageAction.UserSnapshot,
//                Callback = (m, e) => ProcessSubscribe(m, e)
//            });

//            return this;
//        }

//        /// <summary>
//        /// Process real time data subscription
//        /// </summary>
//        public IBusMessageOutbound ProcessSubscribe(IBusMessageInbound envelop, PinkoMsgCalculateExpression expression)
//        {
//            var response = (IBusMessageOutbound)envelop;

//            return response;
//        }


//        /// <summary>
//        /// BusListenerUserSubscriberExpressionHandler
//        /// </summary>
//        [Dependency]
//        public InboundMessageHandler<PinkoMsgPing> PinkoMsgPingRouter { get; set; }

//    }
//}
