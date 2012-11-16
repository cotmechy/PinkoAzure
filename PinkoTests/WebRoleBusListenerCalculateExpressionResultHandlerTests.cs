using System;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoCommon;
using PinkoCommon.Interface;
using PinkoMocks;
using PinkoWebRoleCommon.Interface;
using PinkoWebRoleCommon.RoleHandler;

namespace PinkoTests
{
    [TestClass]
    public class WebRoleBusListenerCalculateExpressionResultHandlerTests
    {

        /// <summary>
        /// check IsSubscribtion filter - success
        /// </summary>
        [TestMethod]
        public void TestRoleCalculateExpressionSnapshotHandlerIsSubscribtionFail()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();
            var handler = pinkoContainer.Resolve<WebRoleBusListenerCalculateExpressionResultHandler>().Register();

            var msgObj = SampleMockData.GetPinkoMsgCalculateExpressionResult(1)[0];

            // Test formula
            var envelop = new PinkoServiceMessageEnvelop() { Message = msgObj, ReplyTo = "UniteTestReplyQueue" };

            // Process request
            handler.PinkoMsgCalculateExpressionResultRouter.HandlerAction(envelop, msgObj);

            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 1);
        }
    }
}
