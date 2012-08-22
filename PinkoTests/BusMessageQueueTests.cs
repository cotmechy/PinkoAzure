using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkoCommon;
using PinkoCommon.Interface;
using PinkoMocks;
using Microsoft.Practices.Unity;
using PinkoWorkerCommon.Utility;

namespace PinkoTests
{
    /// <summary>
    /// Summary description for BusMessageQueueTests
    /// </summary>
    [TestClass]
    public class BusMessageQueueTests
    {
        /// <summary>
        /// Basic double expression
        /// </summary>
        [TestMethod]
        public void TestGetGetIncomingSubscriber()
        {
            var pinkoContainer = PinkoContainerMock.GetMokContainer();
            var messageBus = pinkoContainer.Resolve<IBusMessageServer>();
            var config = pinkoContainer.Resolve<IPinkoConfiguration>();

            var incomingTopic = messageBus.GetTopic(config.PinkoMessageBusToWebAllRolesTopic);

            Tuple<IBusMessageInbound, string> msg = null;
            incomingTopic.GetIncomingSubscriber<string>().Subscribe(x => msg = x);

            // Simulate incoming topic message
            incomingTopic.Send(new PinkoServiceMessageEnvelop()
                                   {
                                       Message = "SringIncomingMessageTest"
                                   });

            Assert.IsNotNull(msg);
            Assert.IsTrue(msg.Item2.Equals("SringIncomingMessageTest"));
        }
    }
}
