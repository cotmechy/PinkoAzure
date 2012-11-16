using System;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.Interface;

namespace PinkoWorkerCommon.RoleHandlers
{
    public class RoleBusListenerPinkoPingMessageHandle
    {
        /// <summary>
        /// Initialize and register role handler
        /// </summary>
        /// <returns></returns>
        public RoleBusListenerPinkoPingMessageHandle Register()
        {

            PinkoMsgPingReactiveListener.Subscribers.Add(new Handlesubscriber<PinkoMsgPing>()
            {
                FilterCallback = (m, p) => true,
                Callback = (m, e) => ProcessSubscribe(m, e)
            });

            return this;
        }

        /// <summary>
        /// Process real time data subscription
        /// </summary>
        public IBusMessageOutbound ProcessSubscribe(IBusMessageInbound envelop, PinkoMsgPing pinkoMsgPing)
        {
            var response = (IBusMessageOutbound)envelop;

            pinkoMsgPing.ResponderMachine = PinkoApplication.MachineName;
            pinkoMsgPing.ResponderDateTime = DateTime.Now;

            response.Message = pinkoMsgPing;

            return response;
        }


        /// <summary>
        /// PinkoApplication
        /// </summary>
        [Dependency]
        public IPinkoApplication PinkoApplication { get; set; }

        /// <summary>
        /// BusListenerUserSubscriberExpressionHandler
        /// </summary>
        [Dependency]
        public InboundMessageReactiveListener<PinkoMsgPing> PinkoMsgPingReactiveListener { get; set; }
    }
}
