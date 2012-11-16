using System;
using System.Linq;
using System.Threading;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoCommon;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.Extension;
using PinkoCommon.Interface;
using PinkoCommon.Interface.Storage;
using PinkoMocks;
using PinkoWebRoleCommon.Interface;
using PinkoWorkerCommon.RoleHandlers;

namespace PinkoTests
{
    [TestClass]
    public class RoleUserSubscriptionHandlerTests
    {
        /// <summary>
        /// Process PinkoMsgClientPing - success
        /// </summary>
        [TestMethod]
        public void TestPinkoMsgClientPing()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<RoleUserSubscriptionHandler>().Register();

            // Add subscription
            var expression = SampleMockData.GetPinkoMsgCalculateExpression(1)[0];
            handler.ProcessSubscribe(new PinkoServiceMessageEnvelop() { Message = expression }, expression);
            var origDate = handler.ClientSubscriptions.GetEnumerator().First().LastPing; 

            Assert.IsTrue(handler.ClientSubscriptions.Count() == 1);
            Assert.IsTrue(handler.ClientSubscriptions.GetEnumerator().First().LastPing <= DateTime.Now);
            Assert.AreSame(handler.ClientSubscriptions.GetEnumerator().First().CalcExpression, expression);


            // Setup ping
            var pingdate = DateTime.Now.AddHours(1);
            var pingMsg = SampleMockData.GetPinkoMsgClientPing()[0];
            pingMsg.PingTime = pingdate;
            handler.ProcessPinkoMsgClientPing(new PinkoServiceMessageEnvelop() { Message = pingMsg }, pingMsg);

            Assert.IsTrue(handler.ClientSubscriptions.GetEnumerator().First().LastPing == pingdate);
            Assert.IsTrue(handler.ClientSubscriptions.GetEnumerator().First().LastPing != origDate);
            Assert.AreSame(handler.ClientSubscriptions.GetEnumerator().First().CalcExpression, expression);
        }

        /// <summary>
        /// Process PinkoMsgClientPing - failed - late
        /// </summary>
        [TestMethod]
        public void TestPinkoMsgClientPingFailedLate()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<RoleUserSubscriptionHandler>().Register();

            // Add subscription
            var expression = SampleMockData.GetPinkoMsgCalculateExpression(1)[0];
            handler.ProcessSubscribe(new PinkoServiceMessageEnvelop() { Message = expression }, expression);
            var origDate = handler.ClientSubscriptions.GetEnumerator().First().LastPing; 

            Assert.IsTrue(handler.ClientSubscriptions.Count() == 1);
            Assert.IsTrue(handler.ClientSubscriptions.GetEnumerator().First().LastPing <= DateTime.Now);
            Assert.AreSame(handler.ClientSubscriptions.GetEnumerator().First().CalcExpression, expression);


            // Setup ping
            var pingdate = DateTime.Now.AddHours(-1);
            var pingMsg = SampleMockData.GetPinkoMsgClientPing()[0];
            pingMsg.PingTime = pingdate;
            handler.ProcessPinkoMsgClientPing(new PinkoServiceMessageEnvelop() { Message = pingMsg }, pingMsg);

            Assert.IsTrue(handler.ClientSubscriptions.GetEnumerator().First().LastPing != pingdate);
            Assert.IsTrue(handler.ClientSubscriptions.GetEnumerator().First().LastPing == origDate);
            Assert.AreSame(handler.ClientSubscriptions.GetEnumerator().First().CalcExpression, expression);
        }



        /// <summary>
        /// test time out check
        /// </summary>
        [TestMethod]
        public void TestRoutingsExists()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<RoleUserSubscriptionHandler>().Register();

            Assert.IsTrue(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpression>>().Subscribers.Count == 1);
            Assert.IsTrue(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgClientConnect>>().Subscribers.Count == 1);
        }


        /// <summary>
        /// test time out check
        /// </summary>
        [TestMethod]
        public void TestTimer()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var pinkoConfiguration = pinkoContainer.Resolve<IPinkoConfiguration>() as PinkoConfiguration;
            pinkoConfiguration._clientTimeoutThresholdMs = 3000;

            var pinkoApplication = pinkoContainer.RegisterInstance<IPinkoApplication>(pinkoContainer.Resolve<PinkoApplication>()) as PinkoApplication;
            var handler = pinkoContainer.Resolve<RoleUserSubscriptionHandler>().Register();


            // Add subscription
            var expression = SampleMockData.GetPinkoMsgCalculateExpression(1)[0];
            var envelop = new PinkoServiceMessageEnvelop() { Message = expression };
            handler.ProcessSubscribe(envelop, expression);

            Assert.IsTrue(handler.ClientSubscriptions.Count() == 1);
            var expressiontoTimeout = handler.ClientSubscriptions.GetEnumerator().ToList()[0];      // Get the record that should be deleted
            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();

            // Mock a new timeout on one item
            expressiontoTimeout.LastPing = expressiontoTimeout.LastPing.AddMilliseconds(pinkoConfiguration.ClientTimeoutThresholdMs * -2);

            // Wait for worker to remove record
            Thread.Sleep(pinkoConfiguration._clientTimeoutThresholdMs + 1000);

            // REcord should have been removed
            Assert.IsTrue(handler.ClientSubscriptions.Count() == 0);

            // Assure message was sent out
            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 1);
            Assert.IsTrue(outboundMessages.OutboundMessages.First().QueueName.Equals(pinkoConfiguration.PinkoMessageBusToWorkerCalcEngineAllTopic));
        }

        /// <summary>
        /// test time out check
        /// </summary>
        [TestMethod]
        public void TestClienttimeout()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var webRoleConnectManager = pinkoContainer.Resolve<IWebRoleConnectManager>();
            var pinkoConfiguration = pinkoContainer.Resolve<IPinkoConfiguration>();
            var handler = pinkoContainer.Resolve<RoleUserSubscriptionHandler>().Register();

            // Add subscribers
            SampleMockData
                .GetPinkoMsgCalculateExpression(10)
                .ForEach(msgObj =>
                    {
                        msgObj.MsgAction = PinkoMessageAction.UserSubscription; // Set to subscription

                        // Test formula
                        var envelop = new PinkoServiceMessageEnvelop() {Message = msgObj};
                        envelop.PinkoProperties[PinkoMessagePropTag.RoleId] = webRoleConnectManager.WebRoleId;

                        // Add subscription
                        handler.ProcessSubscribe(envelop, msgObj);
                    });

            Assert.IsTrue(handler.ClientSubscriptions.Count() == 10);

            // Mock a new timeout on one item
            var now = handler.ClientSubscriptions.GetEnumerator().ToList()[0].LastPing;             // pick a now date 
            var expressiontoTimeout = handler.ClientSubscriptions.GetEnumerator().ToList()[3];      // Get the record that should be deleted

            // Manually set timeout to before
            expressiontoTimeout.LastPing = expressiontoTimeout.LastPing.AddMilliseconds(pinkoConfiguration.ClientTimeoutThresholdMs * -2);

            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();

            // Remove expressiontoTimeout
            handler.CheckClientTimeout(now);

            Assert.IsTrue(handler.ClientSubscriptions.Count() == 9);
            Assert.IsNull(handler.ClientSubscriptions[expressiontoTimeout.CalcExpression.DataFeedIdentifier.SubscribtionId]);

            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 1);

            Assert.IsTrue(outboundMessages.OutboundMessages.First().QueueName.Equals(pinkoConfiguration.PinkoMessageBusToWorkerCalcEngineAllTopic));

            // Web role Id
            Assert.IsFalse(string.IsNullOrEmpty(outboundMessages.OutboundMessages.First().WebRoleId));
            Assert.IsTrue(outboundMessages.OutboundMessages.First().WebRoleId.Equals(expressiontoTimeout.CalcExpression.DataFeedIdentifier.WebRoleId));

            // Check identifiers
            var outMsg = (PinkoMsgClientTimeout)outboundMessages.OutboundMessages.First().Message;
            Assert.IsTrue(outMsg.DataFeedIdentifier.IsEqual(expressiontoTimeout.CalcExpression.DataFeedIdentifier));
        }


        /// <summary>
        /// Process reconnection change - PinkoMsgClientConnect - Not the same
        /// </summary>
        [TestMethod]
        public void TestPinkoMsgClientConnectNotheSame()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var webRoleConnectManager = pinkoContainer.Resolve<IWebRoleConnectManager>();
            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();
            var handler = pinkoContainer.Resolve<RoleUserSubscriptionHandler>().Register();

            var msgObj = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            msgObj.MsgAction = PinkoMessageAction.UserSubscription;  // Set to subscription

            // Test formula
            var envelop = new PinkoServiceMessageEnvelop() { Message = msgObj };
            envelop.PinkoProperties[PinkoMessagePropTag.RoleId] = webRoleConnectManager.WebRoleId;

            var originalIdentifier = msgObj.DataFeedIdentifier.DeepClone();

            // Add subscription
            pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpression>>().HandlerAction(envelop, msgObj);

            var outMsg = outboundMessages.OutboundMessages.First().Message as PinkoMsgCalculateExpression;
            Assert.IsTrue(outMsg.MsgAction == PinkoMessageAction.ManagerSubscription);
            Assert.IsTrue(outMsg.DataFeedIdentifier.IsEqual(originalIdentifier));
            Assert.IsTrue(handler.ClientSubscriptions.Count() == 1);
            Assert.IsTrue(handler.ClientSubscriptions[originalIdentifier.SubscribtionId].CalcExpression.DataFeedIdentifier.IsEqual(originalIdentifier));

            // Send change
            var clientConn = SampleMockData.GetPinkoMsgClientConnect(2)[1];
            var envelopClientConn = new PinkoServiceMessageEnvelop() { Message = clientConn };
            outboundMessages.Clear();

            // Send Id change
            pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgClientConnect>>().HandlerAction(envelopClientConn, clientConn);

            Assert.IsTrue(outMsg.MsgAction == PinkoMessageAction.ManagerSubscription);
            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 0);  // No outbound messages
            Assert.IsTrue(handler.ClientSubscriptions.Count() == 1);
            Assert.IsFalse(handler.ClientSubscriptions[originalIdentifier.SubscribtionId].CalcExpression.DataFeedIdentifier.IsEqual(clientConn.DataFeedIdentifier));
            Assert.IsTrue(handler.ClientSubscriptions[originalIdentifier.SubscribtionId].CalcExpression.DataFeedIdentifier.IsEqual(originalIdentifier));
        }


        /// <summary>
        /// Process reconnection change - PinkoMsgClientConnect
        /// </summary>
        [TestMethod]
        public void TestPinkoMsgClientConnectSuccess()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var webRoleConnectManager = pinkoContainer.Resolve<IWebRoleConnectManager>();
            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();
            var handler = pinkoContainer.Resolve<RoleUserSubscriptionHandler>().Register();

            var msgObj = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            msgObj.MsgAction = PinkoMessageAction.UserSubscription;  // Set to subscription

            // Test formula
            var envelop = new PinkoServiceMessageEnvelop() { Message = msgObj };
            envelop.PinkoProperties[PinkoMessagePropTag.RoleId] = webRoleConnectManager.WebRoleId;

            var originalIdentifier = msgObj.DataFeedIdentifier.DeepClone();

            // Add subscription
            pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpression>>().HandlerAction(envelop, msgObj);

            var outMsg = outboundMessages.OutboundMessages.First().Message as PinkoMsgCalculateExpression;
            Assert.IsTrue(outMsg.MsgAction == PinkoMessageAction.ManagerSubscription);
            Assert.IsTrue(outMsg.DataFeedIdentifier.IsEqual(originalIdentifier));
            Assert.IsTrue(handler.ClientSubscriptions.Count() == 1);
            Assert.IsTrue(handler.ClientSubscriptions[originalIdentifier.SubscribtionId].CalcExpression.DataFeedIdentifier.IsEqual(originalIdentifier));

            // Send change
            var clientConn = SampleMockData.GetPinkoMsgClientConnect(2)[1];
            var envelopClientConn = new PinkoServiceMessageEnvelop() { Message = clientConn };
            outboundMessages.Clear();

            var changedIdentifier = clientConn.DataFeedIdentifier;
            changedIdentifier.SubscribtionId = originalIdentifier.SubscribtionId;  // AMke it the same as original
            var newCloned = originalIdentifier.PartialClone(changedIdentifier);

            // Send Id change
            pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgClientConnect>>().HandlerAction(envelopClientConn, clientConn);

            Assert.IsTrue(outMsg.MsgAction == PinkoMessageAction.ManagerSubscription);
            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 0);  // No outbound messages
            Assert.IsTrue(handler.ClientSubscriptions.Count() == 1);
            Assert.IsTrue(handler.ClientSubscriptions[originalIdentifier.SubscribtionId].CalcExpression.DataFeedIdentifier.IsEqual(newCloned));
            Assert.IsFalse(handler.ClientSubscriptions[originalIdentifier.SubscribtionId].CalcExpression.DataFeedIdentifier.IsEqual(originalIdentifier));
        }




        /// <summary>
        /// PinkoMessageAction.UserSnapshot filter - fail
        /// </summary>
        [TestMethod]
        public void TestBusListenerCalculateExpressionSnapshotHandlerIsSubscribtionFail()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var webRoleConnectManager = pinkoContainer.Resolve<IWebRoleConnectManager>();
            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();
            var handler = pinkoContainer.Resolve<RoleUserSubscriptionHandler>().Register();

            var msgObj = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            msgObj.MsgAction = PinkoMessageAction.UserSnapshot;  // Set to snapshot

            // Test formula
            var envelop = new PinkoServiceMessageEnvelop() { Message = msgObj, ReplyTo = "UniteTestReplyQueue" };
            envelop.PinkoProperties[PinkoMessagePropTag.RoleId] = webRoleConnectManager.WebRoleId;

            // Process request
            pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpression>>().HandlerAction(envelop, msgObj);

            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 0);
        }


        /// <summary>
        /// Publisher - Update current subscription
        /// </summary>
        [TestMethod]
        public void TestUpdateSubscription()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();
            var handler = pinkoContainer.Resolve<RoleUserSubscriptionHandler>().Register();
            var pinkoConfiguration = pinkoContainer.Resolve<IPinkoConfiguration>();
            var pinkoStorage = pinkoContainer.Resolve<IPinkoStorage>() as PinkoStorageMock;

            //
            // process request #1
            //
            var expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            expression.MsgAction = PinkoMessageAction.UserSubscription;  // Set to subscription
            var envelop = new PinkoServiceMessageEnvelop() { Message = expression, ReplyTo = "UniteTestReplyQueue" };
            
            // Process request
            pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpression>>().HandlerAction(envelop, expression);

            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 1);
            Assert.IsTrue(outboundMessages.OutboundMessages.ElementAt(0).Message is PinkoMsgCalculateExpression);
            Assert.IsTrue(outboundMessages.OutboundMessages.ElementAt(0).QueueName.Equals(pinkoConfiguration.PinkoMessageBusToCalcEngineQueue));
            //Assert.IsTrue(outboundMessages.OutboundMessages.ElementAt(1).Message is PinkoMsgCalculateExpression);

            var outToCalcEnv = outboundMessages.OutboundMessages.ElementAt(0);
            var outToCalcEng = (PinkoMsgCalculateExpression)outToCalcEnv.Message;

            // subscription message sent out to calc engine
            Assert.IsTrue(outToCalcEnv.QueueName.Equals(pinkoConfiguration.PinkoMessageBusToCalcEngineQueue));
            Assert.IsTrue(outToCalcEnv.ReplyTo.Equals("UniteTestReplyQueue"));
            Assert.IsTrue(handler.ClientSubscriptions[expression.DataFeedIdentifier.SubscribtionId].CalcExpression.IsEqual(outToCalcEng));

            // Assure storage was updated
            Assert.IsTrue(pinkoStorage.MockStorage.Count == 1);
            Assert.IsTrue(pinkoStorage.MockStorage.Values.First().Equals(outToCalcEng));
            Assert.IsTrue(pinkoStorage.MockStorage.Values.First().MsgAction == PinkoMessageAction.ManagerSubscription);


            //
            // process request #2 - update to same. A wash.
            //
            expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            expression.MsgAction = PinkoMessageAction.UserSubscription;  // Set to subscription
            envelop = new PinkoServiceMessageEnvelop() { Message = expression, ReplyTo = "UniteTestReplyQueue" };
            outboundMessages.Clear();

            // Process request
            pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpression>>().HandlerAction(envelop, expression);

            //var outToCalcEnv2 = outboundMessages.OutboundMessages.ElementAt(0);
            //var outToCalcEng2 = (PinkoMsgCalculateExpression)outToCalcEnv.Message;

            // subscription message sent out to calc engine
            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 1);
            Assert.IsTrue(outboundMessages.OutboundMessages.First().Message is PinkoMsgCalculateExpressionResult);
            Assert.IsTrue(handler.ClientSubscriptions[expression.DataFeedIdentifier.SubscribtionId].CalcExpression.IsEqual(outToCalcEng));

            // Must be equal

            // Assure storage was updated
            Assert.IsTrue(pinkoStorage.MockStorage.Count == 1); // Should stay the same
            Assert.IsTrue(pinkoStorage.MockStorage.Values.First().Equals(outToCalcEng));
            Assert.IsTrue(pinkoStorage.MockStorage.Values.First().MsgAction == PinkoMessageAction.ManagerSubscription);


            //
            // process request update #3
            //
            expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            expression.DataFeedIdentifier.SignalRId = "new signal id";
            expression.MsgAction = PinkoMessageAction.UserSubscription;  // Set to subscription
            envelop = new PinkoServiceMessageEnvelop() { Message = expression, ReplyTo = "UniteTestReplyQueue" };
            outboundMessages.Clear();

            // Process request
            pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpression>>().HandlerAction(envelop, expression);

            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 1);

            var outToCalcEnv3 = outboundMessages.OutboundMessages.ElementAt(0);
            Assert.IsTrue(outToCalcEnv3.Message is PinkoMsgCalculateExpression);

            var outToCalcEng3 = (PinkoMsgCalculateExpression)outToCalcEnv3.Message;

            // subscription message sent out to calc engine
            Assert.IsTrue(outToCalcEnv3.QueueName.Equals(pinkoConfiguration.PinkoMessageBusToWorkerCalcEngineTopic));
            Assert.IsTrue(outToCalcEnv3.ReplyTo.Equals("UniteTestReplyQueue"));
            Assert.IsFalse(string.IsNullOrEmpty(outToCalcEnv3.WebRoleId));
            Assert.IsTrue(outToCalcEnv3.WebRoleId.Equals(outToCalcEng3.DataFeedIdentifier.WebRoleId));
            Assert.IsTrue(handler.ClientSubscriptions[outToCalcEng3.DataFeedIdentifier.SubscribtionId].CalcExpression.IsEqual(outToCalcEng3));

            // Assure storage was updated
            Assert.IsTrue(pinkoStorage.MockStorage.Count == 1);
            Assert.IsTrue(pinkoStorage.MockStorage.Values.First().Equals(outToCalcEng3));
            Assert.IsTrue(pinkoStorage.MockStorage.Values.First().MsgAction == PinkoMessageAction.ManagerSubscription);
        }



        /// <summary>
        /// New expression changed to dictionary
        /// </summary>
        [TestMethod]
        public void TestCahngeSusbcribtionNew()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<RoleUserSubscriptionHandler>().Register();

            var expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            expression.MsgAction = PinkoMessageAction.UserSubscription;  // Set to subscription
            var envelop = new PinkoServiceMessageEnvelop() { Message = expression, ReplyTo = "UniteTestReplyQueue" };

            // Change expression
            var expressionNew = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            expressionNew.MsgAction = PinkoMessageAction.UserSubscription;  // Set to subscription
            expressionNew.ExpressionFormulasStr = "new formula";

            // process request
            pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpression>>().HandlerAction(envelop, expression);
            pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpression>>().HandlerAction(envelop, expressionNew);

            Assert.IsTrue(handler.ClientSubscriptions.Count() == 1);

            Assert.IsFalse(handler.ClientSubscriptions[expressionNew.DataFeedIdentifier.SubscribtionId].CalcExpression.IsEqual(expression));  // Must not be equal
            Assert.IsTrue(handler.ClientSubscriptions[expressionNew.DataFeedIdentifier.SubscribtionId].CalcExpression.IsEqual(expressionNew));         // Must be equal
            Assert.AreSame(handler.ClientSubscriptions[expressionNew.DataFeedIdentifier.SubscribtionId].CalcExpression, expressionNew);         // Must be equal
        }

        /// <summary>
        /// New expression added to dictionary
        /// </summary>
        [TestMethod]
        public void TestAddSusbcribtionNew()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<RoleUserSubscriptionHandler>().Register();
            var expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            expression.MsgAction = PinkoMessageAction.UserSubscription;  // Set to subscription
            var envelop = new PinkoServiceMessageEnvelop() { Message = expression, ReplyTo = "UniteTestReplyQueue" };

            // process request
            pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpression>>().HandlerAction(envelop, expression);

            Assert.IsTrue(handler.ClientSubscriptions.Count() == 1);

            // Make sure they have the same content
            var chekExp = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            Assert.IsFalse(string.IsNullOrEmpty(handler.ClientSubscriptions[chekExp.DataFeedIdentifier.SubscribtionId].CalcExpression.DataFeedIdentifier.SubscribtionId));
            Assert.IsFalse(string.IsNullOrEmpty(handler.ClientSubscriptions[chekExp.DataFeedIdentifier.SubscribtionId].CalcExpression.DataFeedIdentifier.SignalRId));
            Assert.IsFalse(string.IsNullOrEmpty(handler.ClientSubscriptions[chekExp.DataFeedIdentifier.SubscribtionId].CalcExpression.DataFeedIdentifier.MaketEnvId));
            Assert.IsFalse(string.IsNullOrEmpty(handler.ClientSubscriptions[chekExp.DataFeedIdentifier.SubscribtionId].CalcExpression.DataFeedIdentifier.ClientId));
            Assert.IsFalse(string.IsNullOrEmpty(handler.ClientSubscriptions[chekExp.DataFeedIdentifier.SubscribtionId].CalcExpression.DataFeedIdentifier.ClientCtx));
            Assert.IsTrue(handler.ClientSubscriptions[chekExp.DataFeedIdentifier.SubscribtionId].CalcExpression.IsEqual(chekExp));

            // Assure Runtime Id is assigned properly
            var formulaId = handler.RuntimeIdStart + 1;
            handler.ClientSubscriptions[chekExp.DataFeedIdentifier.SubscribtionId]
                .CalcExpression
                .ExpressionFormulas
                .ForEach(x =>
                    {
                        Assert.IsTrue(x.RuntimeId == formulaId);
                        formulaId++;
                    });
        }

        /// <summary>
        /// check IsSubscribtion filter - success
        /// </summary>
        [TestMethod]
        public void TestBusListenerSubscribeExpressionHandlerSuccess()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var webRoleConnectManager = pinkoContainer.Resolve<IWebRoleConnectManager>();
            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();
            var pinkoConfiguration = pinkoContainer.Resolve<IPinkoConfiguration>();
            var handler = pinkoContainer.Resolve<RoleUserSubscriptionHandler>().Register();

            var msgObj = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            msgObj.MsgAction = PinkoMessageAction.UserSubscription;  // Set to subscription

            // Test formula
            var envelop = new PinkoServiceMessageEnvelop() { Message = msgObj };
            envelop.PinkoProperties[PinkoMessagePropTag.RoleId] = webRoleConnectManager.WebRoleId;

            pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpression>>().HandlerAction(envelop, msgObj);

            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 1);  
            Assert.IsTrue(outboundMessages.OutboundMessages.First().ErrorCode == PinkoErrorCode.Success);
            Assert.IsTrue(outboundMessages.OutboundMessages.First().QueueName.Equals(pinkoConfiguration.PinkoMessageBusToCalcEngineQueue));  
        }


        /// <summary>
        /// check IsSubscribtion filter - fail
        /// </summary>
        [TestMethod]
        public void TestBusListenerSubscribeExpressionHandlerFail()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var webRoleConnectManager = pinkoContainer.Resolve<IWebRoleConnectManager>();
            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();
            var handler = pinkoContainer.Resolve<RoleUserSubscriptionHandler>().Register();

            var msgObj = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            msgObj.MsgAction = PinkoMessageAction.MaxActions;  // Invalid

            // Test formula
            var envelop = new PinkoServiceMessageEnvelop() { Message = msgObj };
            envelop.PinkoProperties[PinkoMessagePropTag.RoleId] = webRoleConnectManager.WebRoleId;

            pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpression>>().HandlerAction(envelop, msgObj);

            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 0);  
        }

    }
}
