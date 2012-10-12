﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoCommon;
using PinkoCommon.Extension;
using PinkoCommon.Interface;
using PinkoExpressionCommon;
using PinkoMocks;
using Microsoft.Practices.Unity;
using PinkoWebRoleCommon.Interface;
using PinkoWorkerCommon.Handler;
using PinkoWorkerCommon.Extensions;

namespace PinkoTests
{
    /// <summary>
    /// Summary description for HandleCalculateExpressionTests
    /// </summary>
    [TestClass]
    public class HandleCalculateExpressionTests
    {



        /// <summary>
        /// check IsSubscribtion filter - fail
        /// </summary>
        [TestMethod]
        public void TestBusListenerCalculateExpressionSnapshotHandlerIsSubscribtionFail()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var webRoleConnectManager = pinkoContainer.Resolve<IWebRoleConnectManager>();
            var outboutBus = pinkoContainer.Resolve<IRxMemoryBus<IBusMessageOutbound>>();
            var handler = pinkoContainer.Resolve<BusListenerCalculateExpressionSnapshotHandler>().Register();

            var msgObj = SampleMockData.GetPinkoMsgCalculateExpression()[0];

            msgObj.MsgAction = PinkoMessageAction.UserSubscription;  // Set to subscription

            IBusMessageOutbound busMessageOutbound = null;
            outboutBus.Subscriber.Subscribe(x => busMessageOutbound = x);

            // Test formula
            var envelop = new PinkoServiceMessageEnvelop() { Message = msgObj, ReplyTo = "UniteTestReplyQueue" };
            envelop.PinkoProperties[PinkoMessagePropTag.RoleId] = webRoleConnectManager.WebRoleId;

            handler.HandlerPublisher.Publish(new Tuple<IBusMessageInbound, PinkoMsgCalculateExpression>(envelop, msgObj));

            Assert.IsNull(busMessageOutbound);
        }

        /// <summary>
        /// check IsSubscribtion filter - succeed
        /// </summary>
        [TestMethod]
        public void TestBusListenerCalculateExpressionSnapshotHandlerIsSubscribtion()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var webRoleConnectManager = pinkoContainer.Resolve<IWebRoleConnectManager>();
            var outboutBus = pinkoContainer.Resolve<IRxMemoryBus<IBusMessageOutbound>>();
            var handler = pinkoContainer.Resolve<BusListenerCalculateExpressionSnapshotHandler>().Register();

            var msgObj = SampleMockData.GetPinkoMsgCalculateExpression()[0];

            IBusMessageOutbound busMessageOutbound = null;
            outboutBus.Subscriber.Subscribe(x => busMessageOutbound = x);

            // Test formula
            var envelop = new PinkoServiceMessageEnvelop() { Message = msgObj, ReplyTo = "UniteTestReplyQueue" };
            envelop.PinkoProperties[PinkoMessagePropTag.RoleId] = webRoleConnectManager.WebRoleId;

            handler.HandlerPublisher.Publish(new Tuple<IBusMessageInbound, PinkoMsgCalculateExpression>(envelop, msgObj));

            Assert.IsNotNull(busMessageOutbound);
        }

        /// <summary>
        /// Test serialized incomplete
        /// </summary>
        [TestMethod]
        public void TestFromUrlParameterincomplete()
        {
            var formulas = PinkoUserExpressionFormulaCommonExtensions.FromUrlParameter(" formulaId1 : Label1 : 1 + 1; formulaId2 : Label2: 2 + 2; formulaId3 :Lab");

            Assert.IsTrue(formulas.Length == 0);
        }

        /// <summary>
        /// Test serialized string empty
        /// </summary>
        [TestMethod]
        public void TestFromUrlParameterEmpty()
        {
            var formulas = PinkoUserExpressionFormulaCommonExtensions.FromUrlParameter(string.Empty);

            Assert.IsNotNull(formulas);
            Assert.IsNotNull(formulas.Length == 0);
        }

        /// <summary>
        /// Test deserialized GET formula to typed array
        /// </summary>
        [TestMethod]
        public void TestFromUrlParameter()
        {
            var formulas = PinkoUserExpressionFormulaCommonExtensions.FromUrlParameter(" formulaId1 : Label1 : 1 + 1; formulaId2 : Label2: 2 + 2; formulaId3 :Label3: 3 + 3; ");

            Assert.IsTrue(formulas[0].FormulaId == "formulaId1");
            Assert.IsTrue(formulas[0].ExpressionLabel == "Label1");
            Assert.IsTrue(formulas[0].ExpressionFormula == "1 + 1");

            Assert.IsTrue(formulas[1].FormulaId == "formulaId2");
            Assert.IsTrue(formulas[1].ExpressionLabel == "Label2");
            Assert.IsTrue(formulas[1].ExpressionFormula == "2 + 2");

            Assert.IsTrue(formulas[2].FormulaId == "formulaId3");
            Assert.IsTrue(formulas[2].ExpressionLabel == "Label3");
            Assert.IsTrue(formulas[2].ExpressionFormula == "3 + 3");
        }

        /// <summary>
        /// Test tuple builder double[]
        /// </summary>
        [TestMethod]
        public void TestGetTupleResultDouble()
        {
            var reponseTuple = SampleMockData.GetPinkoUserExpressionFormula(3).GetTupleResult(new double[] { 1.1, 2.2, 3.3 });

            Assert.IsTrue(reponseTuple.Length == 3);

            Assert.IsTrue(reponseTuple[0].OriginalFormula.ExpressionFormula == SampleMockData.GetPinkoUserExpressionFormula(3)[0].ExpressionFormula);
            Assert.IsTrue(reponseTuple[1].OriginalFormula.ExpressionFormula == SampleMockData.GetPinkoUserExpressionFormula(3)[1].ExpressionFormula);
            Assert.IsTrue(reponseTuple[2].OriginalFormula.ExpressionFormula == SampleMockData.GetPinkoUserExpressionFormula(3)[2].ExpressionFormula);

            Assert.IsTrue(reponseTuple[0].PointSeries.Length == 1);
            Assert.IsTrue(reponseTuple[0].PointSeries[0].PointValue == 1.1);

            Assert.IsTrue(reponseTuple[1].PointSeries.Length == 1);
            Assert.IsTrue(reponseTuple[1].PointSeries[0].PointValue == 2.2);

            Assert.IsTrue(reponseTuple[2].PointSeries.Length == 1);
            Assert.IsTrue(reponseTuple[2].PointSeries[0].PointValue == 3.3);
        }

        /// <summary>
        /// Test tuple builder double[][]
        /// </summary>
        [TestMethod]
        public void TestGetTupleResultDoubleDouble()
        {
            var reponseTuple =
                SampleMockData.GetPinkoUserExpressionFormula(3).GetTupleResult(new double[][]
                                                                          {
                                                                              new[] {1.1, 2.2, 3.3},
                                                                              new[] {11.1, 22.2, 33.3},
                                                                              new[] {111.1, 222.2, 333.3}
                                                                          });

            Assert.IsTrue(reponseTuple.Length == 3);

            Assert.IsTrue(reponseTuple[0].OriginalFormula.ExpressionFormula == SampleMockData.GetPinkoUserExpressionFormula(3)[0].ExpressionFormula);
            Assert.IsTrue(reponseTuple[1].OriginalFormula.ExpressionFormula == SampleMockData.GetPinkoUserExpressionFormula(3)[1].ExpressionFormula);
            Assert.IsTrue(reponseTuple[2].OriginalFormula.ExpressionFormula == SampleMockData.GetPinkoUserExpressionFormula(3)[2].ExpressionFormula);

            Assert.IsTrue(reponseTuple[0].PointSeries.Length == 3);
            Assert.IsTrue(reponseTuple[0].PointSeries[0].PointValue == 1.1);
            Assert.IsTrue(reponseTuple[0].PointSeries[1].PointValue == 2.2);
            Assert.IsTrue(reponseTuple[0].PointSeries[2].PointValue == 3.3);

            Assert.IsTrue(reponseTuple[1].PointSeries.Length == 3);
            Assert.IsTrue(reponseTuple[1].PointSeries[0].PointValue == 11.1);
            Assert.IsTrue(reponseTuple[1].PointSeries[1].PointValue == 22.2);
            Assert.IsTrue(reponseTuple[1].PointSeries[2].PointValue == 33.3);

            Assert.IsTrue(reponseTuple[2].PointSeries.Length == 3);
            Assert.IsTrue(reponseTuple[2].PointSeries[0].PointValue == 111.1);
            Assert.IsTrue(reponseTuple[2].PointSeries[1].PointValue == 222.2);
            Assert.IsTrue(reponseTuple[2].PointSeries[2].PointValue == 333.3);
        }

        /// <summary>
        /// Check Fields
        /// </summary>
        [TestMethod]
        public void TestCheckFields()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<BusListenerCalculateExpressionSnapshotHandler>().Register() as BusListenerCalculateExpressionSnapshotHandler;
            var webRoleConnectManager = pinkoContainer.Resolve<IWebRoleConnectManager>();

            var msgObj = new PinkoMsgCalculateExpression
            {
                ResultType = PinkoCalculateExpressionDaoExtensions.ResultDouble,
                ExpressionFormulas = SampleMockData.GetPinkoUserExpressionFormula(3),
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
            var envelop = new PinkoServiceMessageEnvelop() {Message = msgObj};
            envelop.PinkoProperties[PinkoMessagePropTag.RoleId] = webRoleConnectManager.WebRoleId;
            var outboundMsg = handler.ProcessRequest(envelop, msgObj);

            Assert.IsTrue(outboundMsg.ErrorCode == PinkoErrorCode.Success);
            Assert.IsTrue(outboundMsg.WebRoleId == msgObj.DataFeedIdentifier.WebRoleId);
            Assert.IsFalse(string.IsNullOrEmpty(outboundMsg.WebRoleId));
            
            // needs a response role id to route to a specific web role
            Assert.IsFalse(string.IsNullOrEmpty(webRoleConnectManager.WebRoleId));
            Assert.IsTrue(outboundMsg.PinkoProperties[PinkoMessagePropTag.RoleId] == webRoleConnectManager.WebRoleId);

            var resultMsg = (PinkoMsgCalculateExpressionResult)outboundMsg.Message;
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
            var handler = pinkoContainer.Resolve<BusListenerCalculateExpressionSnapshotHandler>().Register() as BusListenerCalculateExpressionSnapshotHandler;

            var msgObj = new PinkoMsgCalculateExpression
            {
                ResultType = -99,
                ExpressionFormulas = SampleMockData.GetPinkoUserExpressionFormula(),
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
            var handler = pinkoContainer.Resolve<BusListenerCalculateExpressionSnapshotHandler>().Register() as BusListenerCalculateExpressionSnapshotHandler;

            var msgObj = new PinkoMsgCalculateExpression
                            {
                                ResultType = PinkoCalculateExpressionDaoExtensions.ResultDouble,
                                ExpressionFormulas = SampleMockData.GetPinkoUserExpressionFormula(3),
                                DataFeedIdentifier = { MaketEnvId = PinkoMarketEnvironmentMock.MockMarketEnvId }
                            };

            // Test formula
            var outboundMsg = handler.ProcessRequest(new PinkoServiceMessageEnvelop() { Message = msgObj }, msgObj);

            Assert.IsTrue(outboundMsg.ErrorCode == PinkoErrorCode.Success);
            var resultMsg = (PinkoMsgCalculateExpressionResult)outboundMsg.Message;

            Assert.IsTrue(resultMsg.ResultType == PinkoCalculateExpressionDaoExtensions.ResultDouble);
            Assert.IsTrue(resultMsg.ResultsTupple[0].PointSeries[0].PointValue == 0);
        }

        /// <summary>
        /// Exception in handler
        /// </summary>
        [TestMethod]
        public void TestSubscribeException()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<BusListenerCalculateExpressionSnapshotHandler>().Register() as BusListenerCalculateExpressionSnapshotHandler;
            var expEngine = pinkoContainer.Resolve<IPinkoExpressionEngine>() as PinkoExpressionEngineMock;

            var msgObj = new PinkoMsgCalculateExpression
                             {
                                 ResultType = PinkoCalculateExpressionDaoExtensions.ResultDouble,
                                 ExpressionFormulas = SampleMockData.GetPinkoUserExpressionFormula(3),
                                 DataFeedIdentifier = { MaketEnvId = PinkoMarketEnvironmentMock.MockMarketEnvId }
                             };

            // Test formula - should exception
            expEngine.ExceptionParseAction = () =>
                                          {
                                              throw new Exception("MockException");
                                          };
            var outboundMsg = handler.ProcessRequest(new PinkoServiceMessageEnvelop() { Message = msgObj }, msgObj);
            var resultMsg = (PinkoMsgCalculateExpressionResult)outboundMsg.Message;

            Assert.IsTrue(outboundMsg.ErrorCode == PinkoErrorCode.UnexpectedException);
            Assert.IsTrue(resultMsg.ResultType == PinkoErrorCode.UnexpectedException);
        }

        /// <summary>
        /// Test double[][]
        /// </summary>
        [TestMethod]
        public void TestSubscribeRequestDoubleDouble()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var handler = pinkoContainer.Resolve<BusListenerCalculateExpressionSnapshotHandler>().Register() as BusListenerCalculateExpressionSnapshotHandler;

            var msgObj = new PinkoMsgCalculateExpression
            {
                ResultType = PinkoCalculateExpressionDaoExtensions.ResultDoubleSeries,
                ExpressionFormulas = SampleMockData.GetPinkoUserExpressionFormula(3),
                DataFeedIdentifier = { MaketEnvId = PinkoMarketEnvironmentMock.MockMarketEnvId }
            };

            // Test formula
            var outboundMsg = handler.ProcessRequest(new PinkoServiceMessageEnvelop() { Message = msgObj }, msgObj);

            Assert.IsTrue(outboundMsg.ErrorCode == PinkoErrorCode.Success);
            var resultMsg = (PinkoMsgCalculateExpressionResult)outboundMsg.Message;

            Assert.IsTrue(resultMsg.ResultType == PinkoCalculateExpressionDaoExtensions.ResultDoubleSeries);
            Assert.IsTrue(resultMsg.ResultsTupple[0].PointSeries[0].PointValue == 0);
        }
    }
}
