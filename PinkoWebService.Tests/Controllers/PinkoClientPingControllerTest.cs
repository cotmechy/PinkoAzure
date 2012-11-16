using System;
using System.Linq;
using System.Net;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoCommon;
using PinkoCommon.Interface;
using PinkoMocks;
using PinkoWebRoleCommon.Interface;
using PinkoWebService.Controllers;

namespace PinkoWebService.Tests.Controllers
{
    [TestClass]
    public class PinkoClientPingControllerTest
    {
        [TestMethod]
        public void TestSendMessage()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var pinkoConfiguration = pinkoContainer.Resolve<IPinkoConfiguration>();
            var webRoleConnectManager = pinkoContainer.Resolve<IWebRoleConnectManager>();
            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();
            var controller = pinkoContainer.Resolve<PinkoClientPingController>();

            var result = controller.GetPing(    "marketenvid",
                                                "clientctx",
                                                "clientid",
                                                "signalr",
                                                "webroleid",
                                                "subscriptionid"
                                                );

            Assert.IsTrue(result.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 1);

            var outEnvelop = outboundMessages.OutboundMessages.First();
            Assert.IsTrue(outEnvelop.QueueName.Equals(pinkoConfiguration.PinkoMessageBusToWorkerSubscriptionManagerAllTopic));
            Assert.IsTrue(outEnvelop.ReplyTo.Equals(string.Empty));
            Assert.IsTrue(outEnvelop.ReplyTo.Equals(string.Empty));
            Assert.IsTrue(outEnvelop.PinkoProperties[PinkoMessagePropTag.RoleId].Equals(webRoleConnectManager.WebRoleId));

            var outMsg = outboundMessages.OutboundMessages.First().Message as PinkoMsgClientPing;
            Assert.IsNotNull(outMsg);
            Assert.IsTrue(outMsg.DataFeedIdentifier.MaketEnvId.Equals("marketenvid"));
            Assert.IsTrue(outMsg.DataFeedIdentifier.ClientCtx.Equals("clientctx"));
            Assert.IsTrue(outMsg.DataFeedIdentifier.ClientId.Equals("clientid"));
            Assert.IsTrue(outMsg.DataFeedIdentifier.SignalRId.Equals("signalr"));
            Assert.IsTrue(outMsg.DataFeedIdentifier.WebRoleId.Equals("webroleid"));
            Assert.IsTrue(outMsg.DataFeedIdentifier.SubscribtionId.Equals("subscriptionid"));
        }
    }
}
