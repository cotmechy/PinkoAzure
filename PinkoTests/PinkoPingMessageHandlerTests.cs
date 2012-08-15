using System;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoCommon.Interface;
using PinkoMocks;
using PinkoServices.Handlers;
using PinkoWorkerCommon.Utility;

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
            var pinkoContainer = PinkoContainerMock.GetMokContainer();
            var pinkoApplication = pinkoContainer.Resolve<IPinkoApplication>();
            var pinkoPingMessageHandler = pinkoContainer.Resolve<HandlerPinkoPingMessage>().Register();
            var pm = new PinkoPingMessage { SenderMachine = "UnitTestClientMachine" };
            var pme = new PinkoServiceMessageEnvelop() {Message = pm, QueueName = "QueueNameResponse"};
            IBusMessageOutbound outboundMsg = null;

            // Listen for outbound messages
            pinkoApplication
                .GetSubscriber<IBusMessageOutbound>()
                .Subscribe(x => outboundMsg = x);

            // Send mock message
            pinkoApplication
                .GetBus<Tuple<IBusMessageInbound, PinkoPingMessage>>()
                .Publish(new Tuple<IBusMessageInbound, PinkoPingMessage>(pme, pm));

            Assert.IsNotNull(outboundMsg);
            Assert.IsNotNull(outboundMsg.Message is PinkoPingMessage);
            Assert.IsNotNull(((PinkoPingMessage)outboundMsg.Message).ResponderMachine.Equals(pinkoApplication.MachineName));
        }
    }
}
