using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoCalcEngineWorker.Handlers;
using PinkoCommon;
using PinkoCommon.Interface;
using PinkoMocks;
using Microsoft.Practices.Unity;
using PinkoWorkerCommon.Utility;

namespace PinkoTests
{
    /// <summary>
    /// Summary description for HandleCalculateExpressionTests
    /// </summary>
    [TestClass]
    public class HandleCalculateExpressionTests
    {
        /// <summary>
        /// Bad formula type
        /// </summary>
        [TestMethod]
        public void TestBadType()
        {
            var pinkoContainer = PinkoContainerMock.GetMokContainer();
            var handler = pinkoContainer.Resolve<HandleCalculateExpression>().Register() as HandleCalculateExpression;

            var msgObj = new PinkoCalculateExpressionDao
            {
                ResultType = -99,
                ExpressionFormula = "ExpressionFormula",
                MaketEnvId = PinkoMarketEnvironmentMock.MockMarketEnvId,
                ResultValue = double.MinValue
            };

            // Listen for outbound messages to monitor outbouns queue
            var outboundMsg = handler.ProcessRequest(new PinkoServiceMessageEnvelop() { Message = msgObj }, msgObj);

            Assert.IsTrue(outboundMsg.ErrorCode == PinkoErrorCode.FormulaTypeNotSupported);
        }


        /// <summary>
        /// Run same formula with 2 diff data access layers
        /// </summary>
        [TestMethod]
        public void TestSubscribeRequest()
        {
            var pinkoContainer = PinkoContainerMock.GetMokContainer();
            var handler = pinkoContainer.Resolve<HandleCalculateExpression>().Register() as HandleCalculateExpression;

            var msgObj = new PinkoCalculateExpressionDao
                             {
                                 ExpressionFormula = "ExpressionFormula",
                                 MaketEnvId = PinkoMarketEnvironmentMock.MockMarketEnvId,
                                 ResultValue = double.MinValue
                             };

            // Test formula
            var outboundMsg = handler.ProcessRequest(new PinkoServiceMessageEnvelop() { Message = msgObj }, msgObj);

            Assert.IsTrue(outboundMsg.ErrorCode == PinkoErrorCode.Success);
            Assert.IsTrue(((double)msgObj.ResultValue) == 0.0);
        }
    }
}
