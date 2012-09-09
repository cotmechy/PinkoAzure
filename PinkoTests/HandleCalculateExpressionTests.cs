using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoCommon;
using PinkoExpressionCommon;
using PinkoMocks;
using Microsoft.Practices.Unity;
using PinkoWorkerCommon.Handler;

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
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<BusListenerCalculateExpression>().Register() as BusListenerCalculateExpression;

            var msgObj = new PinkoCalculateExpression
            {
                ResultType = -99,
                ExpressionFormula = "ExpressionFormula",
                MaketEnvId = PinkoMarketEnvironmentMock.MockMarketEnvId,
                //ResultValue = double.MinValue
            };

            // Listen for outbound messages to monitor outbound queue
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
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<BusListenerCalculateExpression>().Register() as BusListenerCalculateExpression;

             var msgObj = new PinkoCalculateExpression
                             {
                                 ResultType = PinkoCalculateExpressionDaoExtensions.ResultDouble,
                                 ExpressionFormula = "ExpressionFormula",
                                 MaketEnvId = PinkoMarketEnvironmentMock.MockMarketEnvId,
                                 //ResultValue = double.MinValue
                             };

            // Test formula
            var outboundMsg = handler.ProcessRequest(new PinkoServiceMessageEnvelop() { Message = msgObj }, msgObj);

            Assert.IsTrue(outboundMsg.ErrorCode == PinkoErrorCode.Success);
            var resultMsg = (PinkoCalculateExpressionResult) outboundMsg.Message;

            Assert.IsTrue(resultMsg.ResultType == PinkoCalculateExpressionDaoExtensions.ResultDouble);
            Assert.IsInstanceOfType(resultMsg.ResultValue, typeof(double));
            Assert.IsTrue((double)resultMsg.ResultValue == 0.0);
        }

        /// <summary>
        /// Exception in handler
        /// </summary>
        [TestMethod]
        public void TestSubscribeException()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<BusListenerCalculateExpression>().Register() as BusListenerCalculateExpression;
            var expEngine = pinkoContainer.Resolve<IPinkoExpressionEngine>() as PinkoExpressionEngineMock;

            var msgObj = new PinkoCalculateExpression
                             {
                                 ResultType = PinkoCalculateExpressionDaoExtensions.ResultDouble,
                                 ExpressionFormula = "ExpressionFormula",
                                 MaketEnvId = PinkoMarketEnvironmentMock.MockMarketEnvId,
                                 //ResultValue = double.MinValue
                             };

            // Test formula - should exption
            expEngine.ExceptionCall = () =>
                                          {
                                              throw new Exception("MockException");
                                          };
            var outboundMsg = handler.ProcessRequest(new PinkoServiceMessageEnvelop() { Message = msgObj }, msgObj);
            var resultMsg = (PinkoCalculateExpressionResult)outboundMsg.Message;

            Assert.IsTrue(outboundMsg.ErrorCode == PinkoErrorCode.UnexpectedException);
            Assert.IsTrue((string) resultMsg.ResultValue == PinkoMessagesText.Error);
        }

    }
}
