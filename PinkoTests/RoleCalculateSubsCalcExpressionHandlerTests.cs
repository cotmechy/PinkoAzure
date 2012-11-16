using System;
using System.Linq;
using System.Threading;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoCommon;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.Interface;
using PinkoCommon.Utility;
using PinkoExpressionCommon;
using PinkoMocks;
using PinkoWebRoleCommon.Interface;
using PinkoWorkerCommon.RoleHandlers;

namespace PinkoTests
{
    [TestClass]
    public class RoleCalculateSubsCalcExpressionHandlerTests
    {
        /// <summary>
        /// Test parsing exception in subscribtion. Error message shoudl be sent.
        /// </summary>
        [TestMethod]
        public void TestRoleCalculateExpressionSnapshotHandlerIsSubscribtionFail()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            //var webRoleConnectManager = pinkoContainer.Resolve<IWebRoleConnectManager>();
            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();
            var handler = pinkoContainer.Resolve<RoleCalculateSubsCalcExpressionHandler>().Register();
            var msgHandler = pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpression>>();
            var pinkoExpressionEngine = pinkoContainer.Resolve<IPinkoExpressionEngine>() as PinkoExpressionEngineMock;

            // set to throw exception when parsing
            pinkoExpressionEngine.ExceptionParseAction = () =>
            {
                throw new Exception("MockParsingException");
            };

            var expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            expression.MsgAction = PinkoMessageAction.ManagerSubscription;
            expression.ResultType = PinkoCalculateExpressionDaoExtensions.ResultDouble;
            
            // Process request
            msgHandler.HandlerAction(new PinkoServiceMessageEnvelop() { Message = expression, ReplyTo = "UnitTestReplyTo"}, expression);

            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 1);

            var outMsg = outboundMessages.OutboundMessages.First();
            Assert.IsTrue(outMsg.QueueName.Equals("UnitTestReplyTo"));
            Assert.IsTrue(string.IsNullOrEmpty(outMsg.ReplyTo));

            Assert.IsTrue(outMsg.ErrorCode == PinkoErrorCode.UnexpectedException);
        }

        
        /// <summary>
        /// Bad formula update - double [] - fail
        /// </summary>
        [TestMethod]
        public void TestSusbcribeDoubleFaileUpdate()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var pinkoExpressionEngine = pinkoContainer.Resolve<IPinkoExpressionEngine>() as PinkoExpressionEngineMock;
            var handler = pinkoContainer.Resolve<RoleCalculateSubsCalcExpressionHandler>().Register();

            //
            // Success new formula
            //
            var expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            expression.MsgAction = PinkoMessageAction.ManagerSubscription;
            expression.ResultType = PinkoCalculateExpressionDaoExtensions.ResultDouble;

            var response = handler.ProcessSubscribe(new PinkoServiceMessageEnvelop() { Message = expression }, expression);
            Assert.IsTrue(handler.SubscribersDouble.Subscribers.Count == 1);


            //
            // Fail update formula
            //
            expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            expression.MsgAction = PinkoMessageAction.ManagerSubscription;
            expression.ResultType = PinkoCalculateExpressionDaoExtensions.ResultDouble;

            // set to throw exception when parsing
            pinkoExpressionEngine.ExceptionParseAction = () =>
                                          {
                                              throw new Exception("MockException");
                                          };

            var excompile = TryCatch.RunInTry( () => handler.ProcessSubscribe(new PinkoServiceMessageEnvelop() { Message = expression }, expression));

            // Assure subscriber is removed when there is an exception
            Assert.IsNotNull(excompile);
            Assert.IsTrue(handler.SubscribersDouble.Subscribers.Count == 0);
        }



        /// <summary>
        /// assure routings are registered
        /// </summary>
        [TestMethod]
        public void TestRoutingsExists()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<RoleCalculateSubsCalcExpressionHandler>().Register();

            Assert.IsTrue(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpression>>().Subscribers.Count == 1);
            Assert.IsTrue(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgClientConnect>>().Subscribers.Count == 1);
            Assert.IsTrue(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgClientTimeout>>().Subscribers.Count == 1);
        }

        /// <summary>
        /// Client timeout processing
        /// </summary>
        [TestMethod]
        public void TestSusbcribeTimeout()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<RoleCalculateSubsCalcExpressionHandler>().Register();

            var expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            expression.MsgAction = PinkoMessageAction.ManagerSubscription;
            expression.ResultType = PinkoCalculateExpressionDaoExtensions.ResultDouble;

            var response = handler.ProcessSubscribe(new PinkoServiceMessageEnvelop() { Message = expression }, expression);
            var responseMsg = response.Message as PinkoMsgCalculateExpressionResult;

            Assert.IsTrue(handler.SubscribersDouble.Subscribers.Count == 1);
            Assert.IsTrue(handler.SubscribersDoubleDouble.Subscribers.Count == 0);

            // Different id to delete, so it should stay the same
            Assert.IsNull(handler.ProcessTimedOutClients(null, SampleMockData.GetPinkoMsgClientTimeout(2)[1]));
            Assert.IsTrue(handler.SubscribersDouble.Subscribers.Count == 1);
            Assert.IsTrue(handler.SubscribersDoubleDouble.Subscribers.Count == 0);

            // Delete it
            Assert.IsNull(handler.ProcessTimedOutClients(null, SampleMockData.GetPinkoMsgClientTimeout()[0]));
            Assert.IsTrue(handler.SubscribersDouble.Subscribers.Count == 0);
            Assert.IsTrue(handler.SubscribersDoubleDouble.Subscribers.Count == 0);

            // Delete it again but nothing should happen
            Assert.IsNull(handler.ProcessTimedOutClients(null, SampleMockData.GetPinkoMsgClientTimeout()[0]));
            Assert.IsTrue(handler.SubscribersDouble.Subscribers.Count == 0);
            Assert.IsTrue(handler.SubscribersDoubleDouble.Subscribers.Count == 0);
        }


        /// <summary>
        /// PinkoMsgClientConnect - Subscription Id change to existing subscription - Not found. collection stays the same.
        /// </summary>
        [TestMethod]
        public void TestSusbcribePinkoMsgClientConnectChangeNotFound()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<RoleCalculateSubsCalcExpressionHandler>().Register();

            // Add valid feed subscription
            SampleMockData
                .GetPinkoMsgCalculateExpression(10)
                .ForEach(expression =>
                {
                    expression.MsgAction = PinkoMessageAction.ManagerSubscription;
                    expression.ResultType = PinkoCalculateExpressionDaoExtensions.ResultDouble;

                    handler.ProcessSubscribe(new PinkoServiceMessageEnvelop() { Message = expression }, expression);
                });

            Assert.IsTrue(handler.SubscribersDouble.Subscribers.Count == 10);

            //
            // Make a subscription change
            //
            var clientConn = SampleMockData.GetPinkoMsgClientConnect(11)[10];

            handler.ProcessConnectionChange(new PinkoServiceMessageEnvelop() { Message = clientConn }, clientConn);

            Assert.IsTrue(handler.SubscribersDouble.Subscribers.Count == 10);

            // Assure array stays the same
            SampleMockData
                            .GetPinkoMsgCalculateExpression(10)
                            .ForEach(expression => Assert.IsTrue(handler.SubscribersDouble.Subscribers[expression.DataFeedIdentifier.SubscribtionId].Subcribers.First().Item2.IsEqual(new PinkoMsgCalculateExpressionResult().FromRequest(expression))));
        }


        /// <summary>
        /// PinkoMsgClientConnect - Subscription Id change to existing subscription
        /// </summary>
        [TestMethod]
        public void TestSusbcribePinkoMsgClientConnectChangeSuccess()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<RoleCalculateSubsCalcExpressionHandler>().Register();

            // Add valid feed subscription
            SampleMockData
                .GetPinkoMsgCalculateExpression(10)
                .ForEach(expression =>
                    {
                        expression.MsgAction = PinkoMessageAction.ManagerSubscription;
                        expression.ResultType = PinkoCalculateExpressionDaoExtensions.ResultDouble;

                        handler.ProcessSubscribe(new PinkoServiceMessageEnvelop() {Message = expression}, expression);
                    });

            Assert.IsTrue(handler.SubscribersDouble.Subscribers.Count == 10);

            //
            // Make a subscription change
            //
            var clientConn = SampleMockData.GetPinkoMsgClientConnect(11)[10];
            clientConn.DataFeedIdentifier.SubscribtionId = SampleMockData.GetPinkoMsgClientConnect(2)[1].DataFeedIdentifier.SubscribtionId;
            clientConn.DataFeedIdentifier.ClientCtx = SampleMockData.GetPinkoMsgClientConnect(2)[1].DataFeedIdentifier.ClientCtx;
            clientConn.DataFeedIdentifier.ClientId = SampleMockData.GetPinkoMsgClientConnect(2)[1].DataFeedIdentifier.ClientId;

            handler.ProcessConnectionChange(new PinkoServiceMessageEnvelop() { Message = clientConn }, clientConn);

            Assert.IsTrue(handler.SubscribersDouble.Subscribers.Count == 10);

            var checkIdentifier = SampleMockData.GetPinkoMsgCalculateExpression(10)[0].DataFeedIdentifier;
            Assert.IsTrue(handler.SubscribersDouble.Subscribers[checkIdentifier.SubscribtionId].Subcribers.First().Item2.DataFeedIdentifier.IsEqual(checkIdentifier));

            checkIdentifier = clientConn.DataFeedIdentifier;
            Assert.IsTrue(handler.SubscribersDouble.Subscribers[checkIdentifier.SubscribtionId].Subcribers.First().Item2.DataFeedIdentifier.IsEqual(checkIdentifier));

            checkIdentifier = SampleMockData.GetPinkoMsgCalculateExpression(10)[1].DataFeedIdentifier;
            Assert.IsFalse(handler.SubscribersDouble.Subscribers[checkIdentifier.SubscribtionId].Subcribers.First().Item2.DataFeedIdentifier.IsEqual(checkIdentifier));

            checkIdentifier = SampleMockData.GetPinkoMsgCalculateExpression(10)[2].DataFeedIdentifier;
            Assert.IsTrue(handler.SubscribersDouble.Subscribers[checkIdentifier.SubscribtionId].Subcribers.First().Item2.DataFeedIdentifier.IsEqual(checkIdentifier));
        }


        /// <summary>
        /// Test timer kicks calculation - real IPinkoApplication parallel loop
        /// </summary>
        [TestMethod]
        public void TestTimerRunCalculation()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            pinkoContainer.RegisterInstance<IPinkoApplication>(pinkoContainer.Resolve<PinkoApplication>());
            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();
            var expEngine = pinkoContainer.Resolve<IPinkoExpressionEngine>() as PinkoExpressionEngineMock;
            var handler = pinkoContainer.Resolve<RoleCalculateSubsCalcExpressionHandler>().Register();

            // double[]
            SampleMockData
                .GetPinkoMsgCalculateExpressionResult(10)
                .ForEach(expression => handler.SubscribersDouble.UpdateSubscriber(expression, () => expEngine.ParseAndCompile<double[]>(expression.ExpressionFormulas.GetExpression())));

            // double[][]
            SampleMockData
                .GetPinkoMsgCalculateExpressionResult(10)
                .ForEach(expression => handler.SubscribersDoubleDouble.UpdateSubscriber(expression, () => expEngine.ParseAndCompile<double[][]>(expression.ExpressionFormulas.GetExpression())));

            //handler.CalculatingMutex.Lock(15000);
            Thread.Sleep(3000);

            Assert.IsTrue(outboundMessages.OutboundMessages.Count >= 20);
            Assert.IsTrue(outboundMessages.OutboundMessages.All(x => x.ErrorCode == PinkoErrorCode.Success));
        }


        /// <summary>
        /// test kickoff calculation
        /// </summary>
        [TestMethod]
        public void TestRunCalculation()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();
            var expEngine = pinkoContainer.Resolve<IPinkoExpressionEngine>() as PinkoExpressionEngineMock;
            var handler = pinkoContainer.Resolve<RoleCalculateSubsCalcExpressionHandler>().Register();

            // double[]
            SampleMockData
                .GetPinkoMsgCalculateExpressionResult(10)
                .ForEach(expression => handler.SubscribersDouble.UpdateSubscriber(expression, () => expEngine.ParseAndCompile<double[]>(expression.ExpressionFormulas.GetExpression())));

            // double[][]
            SampleMockData
                .GetPinkoMsgCalculateExpressionResult(10)
                .ForEach(expression => handler.SubscribersDoubleDouble.UpdateSubscriber(expression, () => expEngine.ParseAndCompile<double[][]>(expression.ExpressionFormulas.GetExpression())));

            handler.RunParallelCalculation();

            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 20);
            Assert.IsTrue(outboundMessages.OutboundMessages.All(x => x.ErrorCode == PinkoErrorCode.Success));
        }


        /// <summary>
        /// Success double [][]
        /// </summary>
        [TestMethod]
        public void TestSusbcribeDoubleDouble()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<RoleCalculateSubsCalcExpressionHandler>().Register();

            var expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            expression.MsgAction = PinkoMessageAction.ManagerSubscription;
            expression.ResultType = PinkoCalculateExpressionDaoExtensions.ResultDoubleSeries;

            var response = handler.ProcessSubscribe(new PinkoServiceMessageEnvelop() { Message = expression }, expression);
            var responseMsg = response.Message as PinkoMsgCalculateExpressionResult;

            Assert.IsTrue(handler.SubscribersDouble.Subscribers.Count == 0);
            Assert.IsTrue(handler.SubscribersDoubleDouble.Subscribers.Count == 1);

            Assert.IsTrue(response.ErrorCode == PinkoErrorCode.Success);
            Assert.IsTrue(response.ErrorDescription == string.Empty);

            Assert.IsTrue(responseMsg.ResultType == PinkoCalculateExpressionDaoExtensions.ResultDoubleSeries);
            Assert.IsTrue(responseMsg.ResultsTupples[0].PointSeries.Count() == 1);
            Assert.IsTrue(double.IsNaN(responseMsg.ResultsTupples[0].PointSeries[0].PointTime));
        }

        /// <summary>
        /// Success double []
        /// </summary>
        [TestMethod]
        public void TestSusbcribeDouble()
        {
            //var pinkoContainer = PinkoContainerMock.GetMockContainer();
            //var handler = pinkoContainer.Resolve<BusListenerCalculateSubsCalcExpressionHandler>().Register() as BusListenerCalculateSubsCalcExpressionHandler;
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<RoleCalculateSubsCalcExpressionHandler>().Register();

            var expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            expression.MsgAction = PinkoMessageAction.ManagerSubscription;
            expression.ResultType = PinkoCalculateExpressionDaoExtensions.ResultDouble;

            var response = handler.ProcessSubscribe(new PinkoServiceMessageEnvelop() { Message = expression }, expression);
            var responseMsg = response.Message as PinkoMsgCalculateExpressionResult;

            Assert.IsTrue(handler.SubscribersDouble.Subscribers.Count == 1);
            Assert.IsTrue(handler.SubscribersDoubleDouble.Subscribers.Count == 0);

            Assert.IsTrue(response.ErrorCode == PinkoErrorCode.Success);
            Assert.IsTrue(response.ErrorDescription == string.Empty);

            Assert.IsTrue(responseMsg.ResultType == PinkoCalculateExpressionDaoExtensions.ResultDouble);
            Assert.IsTrue(responseMsg.ResultsTupples[0].PointSeries.Count() == 1);
            Assert.IsTrue(double.IsNaN(responseMsg.ResultsTupples[0].PointSeries[0].PointTime));
        }


        /// <summary>
        /// Invalid Result type passed
        /// </summary>
        [TestMethod]
        public void TestSusbcribeInvalidResultType()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<RoleCalculateSubsCalcExpressionHandler>().Register();

            var expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            expression.ResultType = 100001;
            expression.MsgAction = PinkoMessageAction.ManagerSubscription;

            var response = handler.ProcessSubscribe(new PinkoServiceMessageEnvelop() { Message = expression }, expression);
            var responseMsg = response.Message as PinkoMsgCalculateExpressionResult;

            Assert.IsTrue(response.ErrorCode == PinkoErrorCode.FormulaTypeNotSupported);
            Assert.IsTrue(response.ErrorDescription == PinkoMessagesText.FormulaNotSupported);
            Assert.IsTrue(responseMsg.ResultType == PinkoErrorCode.FormulaTypeNotSupported);
        }


        /// <summary>
        /// invalid msg action
        /// </summary>
        [TestMethod]
        public void TestSusbcribeInvalid()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<RoleCalculateSubsCalcExpressionHandler>().Register();
            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();

            var expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            expression.MsgAction = PinkoMessageAction.MaxActions;

            var response = handler.ProcessSubscribe(new PinkoServiceMessageEnvelop() { Message = expression }, expression);
            var responseMsg = response.Message as PinkoMsgCalculateExpressionResult;

            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 0);

            Assert.IsTrue(response.ErrorCode == PinkoErrorCode.ActionNotSupported);
            Assert.IsTrue(response.ErrorDescription == PinkoMessagesText.ActionNotSupported);
            Assert.IsTrue(responseMsg.ResultType == PinkoErrorCode.ActionNotSupported);
        }
    }
}
