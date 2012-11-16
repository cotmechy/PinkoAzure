using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoCommon;
using PinkoCommon.Interface;
using PinkoMocks;
using PinkoWebRoleCommon;
using Microsoft.Practices.Unity;

namespace PinkoTests.PinkoWebRoleTests
{
    /// <summary>
    /// Summary description for WebRoleConnectManagertests
    /// </summary>
    [TestClass]
    public class WebRoleConnectManagerTests
    {
        /// <summary>
        /// Test initalization
        /// </summary>
        [TestMethod]
        public void TestPinkoPingMessageHandler()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var pinkoApplication = pinkoContainer.Resolve<IPinkoApplication>();
            //var pinkoPingMessageHandler = pinkoContainer.Resolve<HandlerPinkoPingMessage>().Register();
            var config = pinkoContainer.Resolve<IPinkoConfiguration>();
            var messageBus = pinkoContainer.Resolve<IBusMessageServer>();

            //var pm = new PinkoPingMessage { SenderMachine = "UnitTestClientMachine" };
            //var pme = new PinkoServiceMessageEnvelop() { Message = pm, QueueName = "QueueNameResponse" };

            var webRoleConnect = pinkoContainer.Resolve<WebRoleConnectManager>();

            Task.Factory.StartNew(() => webRoleConnect.Initialize());

            Tuple<IBusMessageInbound, PinkoMsgRoleHeartbeat> msg = null;
            var incomingTopic = messageBus.GetTopic(config.PinkoMessageBusToWebRolesAllTopic);
            incomingTopic.GetIncomingSubscriber<PinkoMsgRoleHeartbeat>().Subscribe(x => msg = x);

            // Simulate incoming topic message
            incomingTopic.Send(new PinkoServiceMessageEnvelop(pinkoApplication)
            {
                Message = new PinkoMsgRoleHeartbeat()
            });

            Assert.IsNotNull(msg);
        }
    }
}