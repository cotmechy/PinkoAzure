using System;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoCommon;
using PinkoCommon.Interface;
using PinkoMocks;
using PinkoWorkerCommon.Handler;

namespace PinkoTests
{
    /// <summary>
    /// Summary description for PinkoPingMessageHandlerTests
    /// </summary>
    [TestClass]
    public class PinkoPingMessageHandlerTests
    {
        [TestMethod]
        public void TestPinkoPingMessageHandler()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var pinkoApplication = pinkoContainer.Resolve<IPinkoApplication>();
            var pinkoPingMessageHandler = pinkoContainer.Resolve<BusListenerPinkoPingMessage>().Register();
            var pm = new PinkoMsgPing { SenderMachine = "UnitTestClientMachine" };
            var pme = new PinkoServiceMessageEnvelop() { Message = pm, QueueName = "QueueNameRquest", ReplyTo = "QueueNameResponse" };
            IBusMessageOutbound outboundMsg = null;

            // Listen for outbound traffic
            pinkoApplication
                .GetSubscriber<IBusMessageOutbound>()
                .Subscribe(x => outboundMsg = x);

            // Send mock message
            pinkoApplication
                .GetBus<Tuple<IBusMessageInbound, PinkoMsgPing>>()
                .Publish(new Tuple<IBusMessageInbound, PinkoMsgPing>(pme, pm));

            Assert.IsNotNull(outboundMsg);
            Assert.IsNotNull(outboundMsg.Message is PinkoMsgPing);
        }
    }
}
