using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoCalcEngineWorker.Handlers;
using PinkoCommon;
using PinkoCommon.Interface;
using PinkoExpressionCommon;
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
            Assert.IsTrue(outboundMsg.ErrorDescription == PinkoMessagesText.FormulasSupported);
        }


        /// <summary>
        /// success simple formula
        /// </summary>
        [TestMethod]
        public void TestSubscribeRequest()
        {
            var pinkoContainer = PinkoContainerMock.GetMokContainer();
            var handler = pinkoContainer.Resolve<HandleCalculateExpression>().Register() as HandleCalculateExpression;

            var msgObj = new PinkoCalculateExpressionDao
                             {
                                 ResultType = PinkoCalculateExpressionDaoExtensions.ResultDouble,
                                 ExpressionFormula = "ExpressionFormula",
                                 MaketEnvId = PinkoMarketEnvironmentMock.MockMarketEnvId,
                                 ResultValue = double.MinValue
                             };

            // Test formula
            var outboundMsg = handler.ProcessRequest(new PinkoServiceMessageEnvelop() { Message = msgObj }, msgObj);

            Assert.IsTrue(outboundMsg.ErrorCode == PinkoErrorCode.Success);
            Assert.IsInstanceOfType(msgObj.ResultValue, typeof(double));
            Assert.IsTrue(((double)msgObj.ResultValue) == 0.0);
        }

        /// <summary>
        /// Exception in handler
        /// </summary>
        [TestMethod]
        public void TestSubscribeException()
        {
            var pinkoContainer = PinkoContainerMock.GetMokContainer();
            var handler = pinkoContainer.Resolve<HandleCalculateExpression>().Register() as HandleCalculateExpression;
            var expEngine = pinkoContainer.Resolve<IPinkoExpressionEngine>() as PinkoExpressionEngineMock;

            var msgObj = new PinkoCalculateExpressionDao
                             {
                                 ResultType = PinkoCalculateExpressionDaoExtensions.ResultDouble,
                                 ExpressionFormula = "ExpressionFormula",
                                 MaketEnvId = PinkoMarketEnvironmentMock.MockMarketEnvId,
                                 ResultValue = double.MinValue
                             };

            // Test formula - should exption
            expEngine.ExceptionCall = () =>
                                          {
                                              throw new Exception("MockException");
                                          };
            var outboundMsg = handler.ProcessRequest(new PinkoServiceMessageEnvelop() { Message = msgObj }, msgObj);

            Assert.IsTrue(outboundMsg.ErrorCode == PinkoErrorCode.UnexpectedException);
            Assert.IsTrue(((string)msgObj.ResultValue) == PinkoMessagesText.Error);
        }

    }
}
