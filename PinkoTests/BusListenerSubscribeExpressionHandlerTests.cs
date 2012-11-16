//using System;
//using System.Linq;
//using Microsoft.Practices.Unity;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using PinkDao;
//using PinkoCommon;
//using PinkoCommon.Extension;
//using PinkoCommon.Interface;
//using PinkoCommon.Interface.Storage;
//using PinkoMocks;
//using PinkoWebRoleCommon.Interface;
//using PinkoWorkerCommon.Handler;

//namespace PinkoTests
//{
//    [TestClass]
//    public class BusListenerSubscribeExpressionHandlerTests
//    {
//        /// <summary>
//        /// Publisher - Update current subscription
//        /// </summary>
//        [TestMethod]
//        public void TestUpdateSubscription()
//        {
//            var pinkoContainer = PinkoContainerMock.GetMockContainer();
//            var handler = pinkoContainer.Resolve<BusListenerUserSubscriberExpressionHandler>().Register() as BusListenerUserSubscriberExpressionHandler;
//            var outboutBus = pinkoContainer.Resolve<IRxMemoryBus<IBusMessageOutbound>>();
//            var pinkoConfiguration = pinkoContainer.Resolve<IPinkoConfiguration>();
//            var pinkoStorage = pinkoContainer.Resolve<IPinkoStorage>() as PinkoStorageMock;

//            IBusMessageOutbound busMessageOutbound = null;
//            outboutBus.Subscriber.Subscribe(x => busMessageOutbound = x);

//            //
//            // process request #1
//            //
//            var results1 = SampleMockData.GetResultTuple();
//            handler.ProcessSubscribe(results1.Item1, results1.Item2, results1.Item3);
//            var outMsg = (PinkoMsgCalculateExpression)busMessageOutbound.Message;

//            // subscription message sent out to calc engine
//            Assert.IsNotNull(busMessageOutbound);
//            Assert.IsTrue(busMessageOutbound.QueueName.Equals(pinkoConfiguration.PinkoMessageBusToCalcEngineQueue));
//            Assert.IsTrue(busMessageOutbound.ReplyTo.Equals(string.Empty));
//            Assert.IsTrue(handler.ClientSubscriptions[results1.Item3.DataFeedIdentifier.SubscribtionId].IsEqual(outMsg));         // Must be equal

//            // Assure storage was updated
//            Assert.IsTrue(pinkoStorage.MockStorage.Count == 1);
//            Assert.IsTrue(pinkoStorage.MockStorage.Values.First().Equals(outMsg));
//            Assert.IsTrue(pinkoStorage.MockStorage.Values.First().MsgAction == PinkoMessageAction.ManagerSubscription);


//            //
//            // process request #2 - update to same. A wash.
//            //
//            var results2 = SampleMockData.GetResultTuple();
//            busMessageOutbound = null;
//            handler.ProcessSubscribe(results2.Item1, results2.Item2, results2.Item3);

//            // subscription message sent out to calc engine
//            Assert.IsNull(busMessageOutbound);  //  a wash message
//            Assert.IsTrue(handler.ClientSubscriptions[results1.Item3.DataFeedIdentifier.SubscribtionId].IsEqual(outMsg));         // Must be equal

//            // Assure storage was updated
//            Assert.IsTrue(pinkoStorage.MockStorage.Count == 1); // Should stay the same
//            Assert.IsTrue(pinkoStorage.MockStorage.Values.First().Equals(outMsg));
//            Assert.IsTrue(pinkoStorage.MockStorage.Values.First().MsgAction == PinkoMessageAction.ManagerSubscription);


//            //
//            // process request update #3
//            //
//            var results3 = SampleMockData.GetResultTuple();
//            busMessageOutbound = null;
//            results3.Item3.DataFeedIdentifier.SignalRId = "new signal id";
//            handler.ProcessSubscribe(results3.Item1, results3.Item2, results3.Item3);
//            var outMsg3 = (PinkoMsgCalculateExpression)busMessageOutbound.Message;

//            // subscription message sent out to calc engine
//            Assert.IsNotNull(busMessageOutbound);
//            Assert.IsTrue(busMessageOutbound.QueueName.Equals(pinkoConfiguration.PinkoMessageBusToWorkerCalcEngineTopic));
//            Assert.IsTrue(busMessageOutbound.ReplyTo.Equals(string.Empty));
//            Assert.IsTrue(handler.ClientSubscriptions[results3.Item3.DataFeedIdentifier.SubscribtionId].IsEqual(outMsg3));         // Must be equal

//            // Assure storage was updated
//            Assert.IsTrue(pinkoStorage.MockStorage.Count == 1);
//            Assert.IsTrue(pinkoStorage.MockStorage.Values.First().Equals(outMsg3));
//            Assert.IsTrue(pinkoStorage.MockStorage.Values.First().MsgAction == PinkoMessageAction.ManagerSubscription);
//        }


//        ///// <summary>
//        ///// Publisher - Storage
//        ///// </summary>
//        //[TestMethod]
//        //public void TestStorageOutBoundMessage()
//        //{
//        //    var pinkoContainer = PinkoContainerMock.GetMockContainer();
//        //    var handler = pinkoContainer.Resolve<BusListenerSubscribeExpressionHandler>().Register() as BusListenerSubscribeExpressionHandler;
//        //    var outboutBus = pinkoContainer.Resolve<IRxMemoryBus<IBusMessageOutbound>>();
//        //    var pinkoConfiguration = pinkoContainer.Resolve<IPinkoConfiguration>();
//        //    var pinkoStorage = pinkoContainer.Resolve<IPinkoStorage>() as PinkoStorageMock;

//        //    // Build messages
//        //    var expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
//        //    var resultMsg = new PinkoMsgCalculateExpressionResult().FromRequest(expression);
//        //    var envelop = new PinkoServiceMessageEnvelop() { Message = expression };

//        //    IBusMessageOutbound busMessageOutbound = null;
//        //    outboutBus.Subscriber.Subscribe(x => busMessageOutbound = x);

//        //    // process request
//        //    handler.ProcessSubscribe(resultMsg, envelop, expression);
//        //    var outMsg = (PinkoMsgCalculateExpression)busMessageOutbound.Message;

//        //    // Assure storage was updated
//        //    Assert.IsTrue(pinkoStorage.MockStorage.Count == 1);
//        //    Assert.IsTrue(pinkoStorage.MockStorage.Values.First().Equals(outMsg));
//        //    Assert.IsTrue(pinkoStorage.MockStorage.Values.First().MsgAction == PinkoMessageAction.ManagerSubscription);
//        //}



//        /// <summary>
//        /// New expression changed to dictionary
//        /// </summary>
//        [TestMethod]
//        public void TestCahngeSusbcribtionNew()
//        {
//            var pinkoContainer = PinkoContainerMock.GetMockContainer();
//            var handler = pinkoContainer.Resolve<BusListenerUserSubscriberExpressionHandler>().Register() as BusListenerUserSubscriberExpressionHandler;
//            var expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
//            var resultMsg = new PinkoMsgCalculateExpressionResult().FromRequest(expression);
//            var envelop = new PinkoServiceMessageEnvelop() { Message = expression, ReplyTo = "UniteTestReplyQueue" };

//            // Change expression
//            var expressionNew = SampleMockData.GetPinkoMsgCalculateExpression()[0];
//            expressionNew.ExpressionFormulasStr = "new formula";

//            handler.ProcessSubscribe(resultMsg, envelop, expression);
//            handler.ProcessSubscribe(resultMsg, envelop, expressionNew);

//            Assert.IsTrue(handler.ClientSubscriptions.Count() == 1);

//            Assert.IsFalse(handler.ClientSubscriptions[expressionNew.DataFeedIdentifier.SubscribtionId].IsEqual(expression));  // Must not be equal

//            Assert.IsTrue(handler.ClientSubscriptions[expressionNew.DataFeedIdentifier.SubscribtionId].IsEqual(expressionNew));         // Must be equal
//            Assert.AreSame(handler.ClientSubscriptions[expressionNew.DataFeedIdentifier.SubscribtionId], expressionNew);         // Must be equal
//        }



//        /// <summary>
//        /// New expression added to dictionary
//        /// </summary>
//        [TestMethod]
//        public void TestAddSusbcribtionNew()
//        {
//            var pinkoContainer = PinkoContainerMock.GetMockContainer();
//            var handler = pinkoContainer.Resolve<BusListenerUserSubscriberExpressionHandler>().Register() as BusListenerUserSubscriberExpressionHandler;

//            var expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
//            var resultMsg = new PinkoMsgCalculateExpressionResult().FromRequest(expression);
//            var envelop = new PinkoServiceMessageEnvelop() { Message = expression, ReplyTo = "UniteTestReplyQueue" };

//            // process request
//            handler.ProcessSubscribe(resultMsg, envelop, expression);

//            Assert.IsTrue(handler.ClientSubscriptions.Count() == 1);

//            // Make sure they have the same content
//            var chekExp = SampleMockData.GetPinkoMsgCalculateExpression()[0];
//            Assert.IsFalse(string.IsNullOrEmpty(handler.ClientSubscriptions[chekExp.DataFeedIdentifier.SubscribtionId].DataFeedIdentifier.SubscribtionId));
//            Assert.IsFalse(string.IsNullOrEmpty(handler.ClientSubscriptions[chekExp.DataFeedIdentifier.SubscribtionId].DataFeedIdentifier.SignalRId));
//            Assert.IsFalse(string.IsNullOrEmpty(handler.ClientSubscriptions[chekExp.DataFeedIdentifier.SubscribtionId].DataFeedIdentifier.MaketEnvId));
//            Assert.IsFalse(string.IsNullOrEmpty(handler.ClientSubscriptions[chekExp.DataFeedIdentifier.SubscribtionId].DataFeedIdentifier.ClientId));
//            Assert.IsFalse(string.IsNullOrEmpty(handler.ClientSubscriptions[chekExp.DataFeedIdentifier.SubscribtionId].DataFeedIdentifier.ClientCtx));
//            Assert.IsTrue(handler.ClientSubscriptions[chekExp.DataFeedIdentifier.SubscribtionId].IsEqual(chekExp));

//            // Assure Runtime Id is assigned properly
//            var formulaId = BusListenerUserSubscriberExpressionHandler.RuntimeIdStart + 1;
//            handler.ClientSubscriptions[chekExp.DataFeedIdentifier.SubscribtionId]
//                .ExpressionFormulas
//                .ForEach(x =>
//                    {
//                        Assert.IsTrue(x.RuntimeId == formulaId);
//                        formulaId++;
//                    });
//        }


//        /// <summary>
//        /// check IsSubscribtion filter - success
//        /// </summary>
//        [TestMethod]
//        public void TestBusListenerSubscribeExpressionHandlerSuccess()
//        {
//            var pinkoContainer = PinkoContainerMock.GetMockContainer();
//            var webRoleConnectManager = pinkoContainer.Resolve<IWebRoleConnectManager>();
//            var outboutBus = pinkoContainer.Resolve<IRxMemoryBus<IBusMessageOutbound>>();
//            var handler = pinkoContainer.Resolve<BusListenerUserSubscriberExpressionHandler>().Register();

//            var msgObj = SampleMockData.GetPinkoMsgCalculateExpression()[0];

//            msgObj.MsgAction = PinkoMessageAction.UserSubscription;  // Set to subscription

//            IBusMessageOutbound busMessageOutbound = null;
//            outboutBus.Subscriber.Subscribe(x => busMessageOutbound = x);

//            // Test formula
//            var envelop = new PinkoServiceMessageEnvelop() { Message = msgObj };
//            envelop.PinkoProperties[PinkoMessagePropTag.RoleId] = webRoleConnectManager.WebRoleId;

//            handler.HandlerPublisher.Publish(new Tuple<IBusMessageInbound, PinkoMsgCalculateExpression>(envelop, msgObj));

//            Assert.IsNotNull(busMessageOutbound);  // process snapshots
//        }

//        /// <summary>
//        /// check IsSubscribtion filter - fail
//        /// </summary>
//        [TestMethod]
//        public void TestBusListenerSubscribeExpressionHandlerFail()
//        {
//            var pinkoContainer = PinkoContainerMock.GetMockContainer();
//            var webRoleConnectManager = pinkoContainer.Resolve<IWebRoleConnectManager>();
//            var outboutBus = pinkoContainer.Resolve<IRxMemoryBus<IBusMessageOutbound>>();
//            var handler = pinkoContainer.Resolve<BusListenerUserSubscriberExpressionHandler>().Register();

//            var msgObj = SampleMockData.GetPinkoMsgCalculateExpression()[0];

//            IBusMessageOutbound busMessageOutbound = null;
//            outboutBus.Subscriber.Subscribe(x => busMessageOutbound = x);

//            // Test formula
//            var envelop = new PinkoServiceMessageEnvelop() { Message = msgObj };
//            envelop.PinkoProperties[PinkoMessagePropTag.RoleId] = webRoleConnectManager.WebRoleId;

//            handler.HandlerPublisher.Publish(new Tuple<IBusMessageInbound, PinkoMsgCalculateExpression>(envelop, msgObj));

//            Assert.IsNull(busMessageOutbound);  // do not process snapshots
//        }


//    }
//}
