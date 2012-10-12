using System;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkoMocks;
using PinkoWorkerCommon.Handler;

namespace PinkoTests
{
    [TestClass]
    public class BusListenerCalculateSubsCalcExpressionHandlerTests
    {
        [TestMethod]
        public void TestSusbcribeType()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<BusListenerCalculateSubsCalcExpressionHandler>().Register() as BusListenerCalculateSubsCalcExpressionHandler;

            var results = SampleMockData.GetResultTuple();

            //handler.ProcessSubscribe(results1.Item1, results1.Item2, results1.Item3);

        }
    }
}
