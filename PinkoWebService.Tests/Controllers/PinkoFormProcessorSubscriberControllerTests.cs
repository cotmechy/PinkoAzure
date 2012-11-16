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
    public class PinkoFormProcessorSubscriberControllerTests
    {
        /// <summary>
        /// Successful subscribe
        /// </summary>
        [TestMethod]
        public void TestSubscribe()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();
            var controller = pinkoContainer.Resolve<PinkoFormProcessorSubscriberController>();
            var webRoleConnectManager = pinkoContainer.Resolve<IWebRoleConnectManager>();

            var result = controller.GetSubscribe(" formulaId1 : Label1 : 1 + 1; formulaId2 : Label2: 2 + 2; formulaId3 :Label3: 3 + 3; ",
                                                    "marketenvid",
                                                    "clientctx",
                                                    "clientid",
                                                    "signalr",
                                                    "webroleid",
                                                    "subscribtionid"
                                                    );

            var outMsg = outboundMessages.OutboundMessages.First().Message as PinkoMsgCalculateExpression;
            Assert.IsTrue(result.StatusCode == HttpStatusCode.OK);
            Assert.IsNotNull(outMsg);

            Assert.IsTrue(outMsg.DataFeedIdentifier.MaketEnvId.Equals("marketenvid"));
            Assert.IsTrue(outMsg.DataFeedIdentifier.ClientCtx.Equals("clientctx"));
            Assert.IsTrue(outMsg.DataFeedIdentifier.ClientId.Equals("clientid"));
            Assert.IsTrue(outMsg.DataFeedIdentifier.SignalRId.Equals("signalr"));
            Assert.IsTrue(outMsg.DataFeedIdentifier.WebRoleId.Equals("webroleid"));

            // web role id required for snapshot calculation
            Assert.IsFalse(string.IsNullOrEmpty(webRoleConnectManager.WebRoleId));
            Assert.IsTrue(outboundMessages.OutboundMessages.First().PinkoProperties[PinkoMessagePropTag.RoleId] == webRoleConnectManager.WebRoleId);
        }
    }
}
