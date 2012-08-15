using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoCommon.Interface;
using PinkoMocks;
using PinkoServices.Handlers;
using PinkoWebRoleCommon;
using Microsoft.Practices.Unity;
using PinkoWorkerCommon.Utility;

namespace PinkoTests.PinkoWebRoleTests
{
    /// <summary>
    /// Summary description for WebRoleConnectManagertests
    /// </summary>
    [TestClass]
    public class WebRoleConnectManagertests
    {
        /// <summary>
        /// Test initalization
        /// </summary>
        [TestMethod]
        public void TestPinkoPingMessageHandler()
        {
            var pinkoContainer = PinkoContainerMock.GetMokContainer();
            var pinkoApplication = pinkoContainer.Resolve<IPinkoApplication>();
            var pinkoPingMessageHandler = pinkoContainer.Resolve<HandlerPinkoPingMessage>().Register();
            var pm = new PinkoPingMessage { SenderMachine = "UnitTestClientMachine" };
            var pme = new PinkoServiceMessageEnvelop() { Message = pm, QueueName = "QueueNameResponse" };

            var webRoleConnect = pinkoContainer.Resolve<WebRoleConnectManager>();

            Task.Factory.StartNew(() => webRoleConnect.Initialize());




        }
    }
}