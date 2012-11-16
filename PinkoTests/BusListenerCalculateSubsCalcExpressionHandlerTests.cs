//using System.Linq;
//using System.Threading;
//using Microsoft.Practices.Unity;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using PinkDao;
//using PinkoCommon;
//using PinkoCommon.Interface;
//using PinkoExpressionCommon;
//using PinkoMocks;

//namespace PinkoTests
//{
//    [TestClass]
//    public class BusListenerCalculateSubsCalcExpressionHandlerTests
//    {

//        /// <summary>
//        /// Test timer kicks calculation - real IPinkoApplication parallel loop
//        /// </summary>
//        [TestMethod]
//        public void TestTimerRunCalculation()
//        {
//            var pinkoContainer = PinkoContainerMock.GetMockContainer();
//            pinkoContainer.RegisterInstance<IPinkoApplication>(pinkoContainer.Resolve<PinkoApplication>());
//            var expEngine = pinkoContainer.Resolve<IPinkoExpressionEngine>() as PinkoExpressionEngineMock;
//            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();
//            var handler = pinkoContainer.Resolve<BusListenerCalculateSubsCalcExpressionHandler>().Register() as BusListenerCalculateSubsCalcExpressionHandler;

//            // double[]
//            SampleMockData
//                .GetPinkoMsgCalculateExpressionResult(10)
//                .ForEach(expression => handler.SubscribersDouble.UpdateSubscriber(expression, expEngine.ParseAndCompile<double[]>(expression.ExpressionFormulas.GetExpression())));

//            // double[][]
//            SampleMockData
//                .GetPinkoMsgCalculateExpressionResult(10)
//                .ForEach(expression => handler.SubscribersDoubleDouble.UpdateSubscriber(expression, expEngine.ParseAndCompile<double[][]>(expression.ExpressionFormulas.GetExpression())));

//            //handler.CalculatingMutex.Lock(15000);
//            Thread.Sleep(3000);

//            Assert.IsTrue(outboundMessages.OutboundMessages.Count >= 20);
//            Assert.IsTrue(outboundMessages.OutboundMessages.All(x => x.ErrorCode == PinkoErrorCode.Success));
//        }

        
//        /// <summary>
//        /// test kickoff calculation
//        /// </summary>
//        [TestMethod]
//        public void TestRunCalculation()
//        {
//            var pinkoContainer = PinkoContainerMock.GetMockContainer();
//            var expEngine = pinkoContainer.Resolve<IPinkoExpressionEngine>() as PinkoExpressionEngineMock;
//            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();
//            var handler = pinkoContainer.Resolve<BusListenerCalculateSubsCalcExpressionHandler>().Register() as BusListenerCalculateSubsCalcExpressionHandler;

//            // double[]
//            SampleMockData
//                .GetPinkoMsgCalculateExpressionResult(10)
//                .ForEach(expression => handler.SubscribersDouble.UpdateSubscriber(expression, expEngine.ParseAndCompile<double[]>(expression.ExpressionFormulas.GetExpression())));

//            // double[][]
//            SampleMockData
//                .GetPinkoMsgCalculateExpressionResult(10)
//                .ForEach(expression => handler.SubscribersDoubleDouble.UpdateSubscriber(expression, expEngine.ParseAndCompile<double[][]>(expression.ExpressionFormulas.GetExpression())));

//            handler.RunParallelCalculation();

//            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 20);
//            Assert.IsTrue(outboundMessages.OutboundMessages.All(x => x.ErrorCode == PinkoErrorCode.Success));
//        }

        
//        /// <summary>
//        /// Success double [][]
//        /// </summary>
//        [TestMethod]
//        public void TestSusbcribeDoubleDouble()
//        {
//            var pinkoContainer = PinkoContainerMock.GetMockContainer();
//            var handler = pinkoContainer.Resolve<BusListenerCalculateSubsCalcExpressionHandler>().Register() as BusListenerCalculateSubsCalcExpressionHandler;

//            var expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
//            expression.MsgAction = PinkoMessageAction.ManagerSubscription;
//            expression.ResultType = PinkoCalculateExpressionDaoExtensions.ResultDoubleSeries;

//            var response = handler.ProcessRequest(new PinkoServiceMessageEnvelop() { Message = expression }, expression);
//            var responseMsg = response.Message as PinkoMsgCalculateExpressionResult;

//            Assert.IsTrue(handler.SubscribersDouble.Subscribers.Count == 0);
//            Assert.IsTrue(handler.SubscribersDoubleDouble.Subscribers.Count == 1);

//            Assert.IsTrue(response.ErrorCode == PinkoErrorCode.Success);
//            Assert.IsTrue(response.ErrorDescription == string.Empty);

//            Assert.IsTrue(responseMsg.ResultType == PinkoCalculateExpressionDaoExtensions.ResultDoubleSeries);
//            Assert.IsTrue(responseMsg.ResultsTupples[0].PointSeries.Count() == 1);
//            Assert.IsTrue(double.IsNaN(responseMsg.ResultsTupples[0].PointSeries[0].PointTime));
//        }

//        /// <summary>
//        /// Success double []
//        /// </summary>
//        [TestMethod]
//        public void TestSusbcribeDouble()
//        {
//            var pinkoContainer = PinkoContainerMock.GetMockContainer();
//            var handler = pinkoContainer.Resolve<BusListenerCalculateSubsCalcExpressionHandler>().Register() as BusListenerCalculateSubsCalcExpressionHandler;

//            var expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
//            expression.MsgAction = PinkoMessageAction.ManagerSubscription;
//            expression.ResultType = PinkoCalculateExpressionDaoExtensions.ResultDouble;

//            var response = handler.ProcessRequest(new PinkoServiceMessageEnvelop() { Message = expression }, expression);
//            var responseMsg = response.Message as PinkoMsgCalculateExpressionResult;

//            Assert.IsTrue(handler.SubscribersDouble.Subscribers.Count == 1);
//            Assert.IsTrue(handler.SubscribersDoubleDouble.Subscribers.Count == 0);

//            Assert.IsTrue(response.ErrorCode == PinkoErrorCode.Success);
//            Assert.IsTrue(response.ErrorDescription == string.Empty);

//            Assert.IsTrue(responseMsg.ResultType == PinkoCalculateExpressionDaoExtensions.ResultDouble);
//            Assert.IsTrue(responseMsg.ResultsTupples[0].PointSeries.Count() == 1);
//            Assert.IsTrue(double.IsNaN(responseMsg.ResultsTupples[0].PointSeries[0].PointTime));
//        }


//        /// <summary>
//        /// Invalid Result type passed
//        /// </summary>
//        [TestMethod]
//        public void TestSusbcribeInvalidResultType()
//        {
//            var pinkoContainer = PinkoContainerMock.GetMockContainer();
//            var handler = pinkoContainer.Resolve<BusListenerCalculateSubsCalcExpressionHandler>().Register() as BusListenerCalculateSubsCalcExpressionHandler;

//            var expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
//            expression.ResultType = 100001;
//            expression.MsgAction = PinkoMessageAction.ManagerSubscription;

//            var response = handler.ProcessRequest(new PinkoServiceMessageEnvelop() { Message = expression }, expression);
//            var responseMsg = response.Message as PinkoMsgCalculateExpressionResult;

//            Assert.IsTrue(response.ErrorCode == PinkoErrorCode.FormulaTypeNotSupported);
//            Assert.IsTrue(response.ErrorDescription == PinkoMessagesText.FormulaNotSupported);
//            Assert.IsTrue(responseMsg.ResultType == PinkoErrorCode.FormulaTypeNotSupported);
//        }


//        /// <summary>
//        /// invalid msg action
//        /// </summary>
//        [TestMethod]
//        public void TestSusbcribeInvalid()
//        {
//            var pinkoContainer = PinkoContainerMock.GetMockContainer();
//            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();
//            var handler = pinkoContainer.Resolve<BusListenerCalculateSubsCalcExpressionHandler>().Register() as BusListenerCalculateSubsCalcExpressionHandler;

//            var expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
//            expression.MsgAction = PinkoMessageAction.MaxActions;
            
//            var response = handler.ProcessRequest(new PinkoServiceMessageEnvelop() { Message = expression }, expression);
//            var responseMsg = response.Message as PinkoMsgCalculateExpressionResult;

//            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 0);

//            Assert.IsTrue(response.ErrorCode == PinkoErrorCode.ActionNotSupported);
//            Assert.IsTrue(response.ErrorDescription == PinkoMessagesText.ActionNotSupported);
//            Assert.IsTrue(responseMsg.ResultType == PinkoErrorCode.ActionNotSupported);
//        }
//    }
//}
