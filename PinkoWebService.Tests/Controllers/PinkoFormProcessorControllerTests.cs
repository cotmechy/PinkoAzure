using System;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoCommon.Interface;
using PinkoMocks;
using PinkoWebRoleCommon.IoC;
using PinkoWebService.Controllers;

namespace PinkoWebService.Tests.Controllers
{
    [TestClass]
    public class PinkoFormProcessorControllerTests
    {
        /// <summary>
        /// Test formula request is sent via the bus to the server
        /// </summary>
        [TestMethod]
        public void TestFormulaRequest()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var pinkoApplication = pinkoContainer.Resolve<IPinkoApplication>();
            var controller = pinkoContainer.Resolve<PinkoFormProcessorController>();
            pinkoContainer.Resolve<IMessageHandlerManager>().AddBusTypeHandler<PinkoCalculateExpression>();

            // Listen for outbound traffic to 
            IBusMessageOutbound outboundMsg = null;
            pinkoApplication
                .GetSubscriber<IBusMessageOutbound>()
                .Subscribe(x => outboundMsg = x);

            controller.GetCalc("Expression formula", "marketenvid", "clientctx");
 
            Assert.IsNotNull(outboundMsg);
            Assert.IsNotNull(outboundMsg.Message is PinkoCalculateExpression);
        }
    }
}
