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
        /// Check Fields
        /// </summary>
        [TestMethod]
        public void TestCheckFields()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<BusListenerCalculateExpressionSnapshot>().Register() as BusListenerCalculateExpressionSnapshot;

            var msgObj = new PinkoMsgCalculateExpression
            {
                ResultType = PinkoCalculateExpressionDaoExtensions.ResultDouble,
                ExpressionFormula = "ExpressionFormula",
                DataFeedIdentifier =
                {
                    MaketEnvId = PinkoMarketEnvironmentMock.MockMarketEnvId,
                    SignalRId = "SignalRId",
                    ClientCtx = "ClientCtx",
                    WebRoleId = "WebRoleId",
                    ClientId = "ClientId"
                }
            };

            // Test formula
            var outboundMsg = handler.ProcessRequest(new PinkoServiceMessageEnvelop() { Message = msgObj }, msgObj);

            Assert.IsTrue(outboundMsg.ErrorCode == PinkoErrorCode.Success);
            Assert.IsTrue(outboundMsg.WebRoleId == msgObj.DataFeedIdentifier.WebRoleId);
            Assert.IsFalse(string.IsNullOrEmpty(outboundMsg.WebRoleId));

            var resultMsg = (PinkoMsgCalculateExpressionResult)outboundMsg.Message;
            Assert.IsTrue(resultMsg.ExpressionFormula.Equals("ExpressionFormula"));
            Assert.IsTrue(resultMsg.DataFeedIdentifier.SignalRId.Equals("SignalRId"));
            Assert.IsTrue(resultMsg.DataFeedIdentifier.ClientCtx.Equals("ClientCtx"));
            Assert.IsTrue(resultMsg.DataFeedIdentifier.WebRoleId.Equals("WebRoleId"));
            Assert.IsTrue(resultMsg.DataFeedIdentifier.ClientId.Equals("ClientId"));
            Assert.IsTrue(resultMsg.DataFeedIdentifier.MaketEnvId.Equals(PinkoMarketEnvironmentMock.MockMarketEnvId));
        }

        /// <summary>
        /// Bad formula type
        /// </summary>
        [TestMethod]
        public void TestBadType()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<BusListenerCalculateExpressionSnapshot>().Register() as BusListenerCalculateExpressionSnapshot;

            var msgObj = new PinkoMsgCalculateExpression
            {
                ResultType = -99,
                ExpressionFormula = "ExpressionFormula",
                DataFeedIdentifier = { MaketEnvId = PinkoMarketEnvironmentMock.MockMarketEnvId }
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
            var handler = pinkoContainer.Resolve<BusListenerCalculateExpressionSnapshot>().Register() as BusListenerCalculateExpressionSnapshot;

             var msgObj = new PinkoMsgCalculateExpression
                             {
                                 ResultType = PinkoCalculateExpressionDaoExtensions.ResultDouble,
                                 ExpressionFormula = "ExpressionFormula",
                                 DataFeedIdentifier = { MaketEnvId = PinkoMarketEnvironmentMock.MockMarketEnvId }
                             };

            // Test formula
            var outboundMsg = handler.ProcessRequest(new PinkoServiceMessageEnvelop() { Message = msgObj }, msgObj);

            Assert.IsTrue(outboundMsg.ErrorCode == PinkoErrorCode.Success);
            var resultMsg = (PinkoMsgCalculateExpressionResult) outboundMsg.Message;

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
            var handler = pinkoContainer.Resolve<BusListenerCalculateExpressionSnapshot>().Register() as BusListenerCalculateExpressionSnapshot;
            var expEngine = pinkoContainer.Resolve<IPinkoExpressionEngine>() as PinkoExpressionEngineMock;

            var msgObj = new PinkoMsgCalculateExpression
                             {
                                 ResultType = PinkoCalculateExpressionDaoExtensions.ResultDouble,
                                 ExpressionFormula = "ExpressionFormula",
                                 DataFeedIdentifier = { MaketEnvId = PinkoMarketEnvironmentMock.MockMarketEnvId }
                             };

            // Test formula - should exption
            expEngine.ExceptionCall = () =>
                                          {
                                              throw new Exception("MockException");
                                          };
            var outboundMsg = handler.ProcessRequest(new PinkoServiceMessageEnvelop() { Message = msgObj }, msgObj);
            var resultMsg = (PinkoMsgCalculateExpressionResult)outboundMsg.Message;

            Assert.IsTrue(outboundMsg.ErrorCode == PinkoErrorCode.UnexpectedException);
            Assert.IsTrue((string) resultMsg.ResultValue == PinkoMessagesText.Error);
        }

    }
}
