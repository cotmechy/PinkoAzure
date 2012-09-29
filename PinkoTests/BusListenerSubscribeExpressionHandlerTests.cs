using System;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoCommon.Interface;
using PinkoMocks;
using PinkoWebRoleCommon.Interface;
using PinkoWorkerCommon.Handler;

namespace PinkoTests
{
    [TestClass]
    public class BusListenerSubscribeExpressionHandlerTests
    {
        [TestMethod]
        public void TestSubscribeSuccess()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var outboutBus = pinkoContainer.Resolve<IRxMemoryBus<IBusMessageOutbound>>();
            var handler = pinkoContainer.Resolve<BusListenerSubscribeExpressionHandler>().Register();

            var msgObj = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            msgObj.MsgAction = PinkoMessageAction.Subscription;




        }
    }
}
