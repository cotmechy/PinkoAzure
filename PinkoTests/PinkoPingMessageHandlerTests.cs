using System;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoCommon;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.Interface;
using PinkoMocks;
using PinkoWorkerCommon.RoleHandlers;

namespace PinkoTests
{
    /// <summary>
    /// Summary description for PinkoPingMessageHandlerTests
    /// </summary>
    [TestClass]
    public class PinkoPingMessageHandlerTests
    {
        /// <summary>
        /// assure routings are registered
        /// </summary>
        [TestMethod]
        public void TestRoutingsExists()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<RoleBusListenerPinkoPingMessageHandle>().Register();

            Assert.IsTrue(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgPing>>().Subscribers.Count == 1);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestPinkoPingMessageHandler()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();

            pinkoContainer.RegisterInstance(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgPing>>().Register());
            var pinkoPingMessageHandler = pinkoContainer.Resolve<RoleBusListenerPinkoPingMessageHandle>().Register();

            var pm = new PinkoMsgPing { SenderMachine = "UnitTestClientMachine" };
            var pme = new PinkoServiceMessageEnvelop() { Message = pm, QueueName = "QueueNameRquest", ReplyTo = "QueueNameResponse" };

            pinkoPingMessageHandler.PinkoMsgPingReactiveListener.HandlerAction(pme, pm);

            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 1);
            Assert.IsNotNull(outboundMessages.OutboundMessages.First().Message is PinkoMsgPing);
        }
    }
}
