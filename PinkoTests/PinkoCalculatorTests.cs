using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoCommon;
using PinkoCommon.ExceptionTypes;
using PinkoCommon.Extension;
using PinkoCommon.Interface;
using PinkoExpressionCommon;
using PinkoExpressionEngine;
using PinkoMocks;
using PinkoTests.Utility;
using PinkoWorkerCommon;
using PinkoWorkerCommon.Utility;

namespace PinkoTests
{
    [TestClass]
    public class PinkoCalculatorTests
    {
        /// <summary>
        /// Test parallel calculations - Symbol not in dictionary
        /// </summary>
        [TestMethod]
        public void TestCalculatePreparedListMissingSymbolIndictionary()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var expEngine = PinkoExpressionEngineFactory.GetNewEngine();
            pinkoContainer.RegisterInstance(expEngine);
            var subscribers = pinkoContainer.Resolve<PinkoExpressionSubscribers<double[][]>>();
            var calculator = pinkoContainer.Resolve<PinkoCalculator<double[][]>>();
            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();
            var outboundSubscriptions = pinkoContainer.Resolve<OutbountListener<PinkoExceptionDataNotSubscribed>>();


            // Load formulas
            var inputResult = SampleMockData.GetPinkoMsgCalculateExpressionResult(1)[0];

            inputResult.ExpressionFormulas = new PinkoUserExpressionFormula[]
                {
                    new PinkoUserExpressionFormula()
                        {
                            ExpressionFormula = " RHist(\"Symbol\", \"Invalid\", \"Price.Bid\", \"Reuters\", \"Hour\", 360) ",
                            ExpressionLabel = " A ",
                            FormulaId = "fomulaid",
                            RuntimeId = 1000001
                        }
                };

            subscribers.UpdateSubscriber(inputResult, () => expEngine.ParseAndCompile<double[][]>(inputResult.ExpressionFormulas.GetExpression()));

            // Calculate here
            calculator.Caculate(subscribers);

            // Assure not messages are sent to client. Silently subscribe to data
            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 0);  

            // Assure subscription was message was triggered
            Assert.IsTrue(outboundSubscriptions.OutboundMessages.Count == 1);
        }


        /// <summary>
        /// Test parallel calculations - fail
        /// </summary>
        [TestMethod]
        public void TestCalculatePreparedListFail()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var expEngine = pinkoContainer.Resolve<IPinkoExpressionEngine>() as PinkoExpressionEngineMock;
            var subscribers = pinkoContainer.Resolve<PinkoExpressionSubscribers<double[]>>();
            var calculator = pinkoContainer.Resolve<PinkoCalculator<double[]>>();
            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();

            // Set up exception  throw
            expEngine.ExceptionInvokeAction = () => { throw new Exception("Test Invoke Exception"); };

            // Load formulas
            var inputResults = SampleMockData.GetPinkoMsgCalculateExpressionResult(10);
            inputResults.ForEach(x => subscribers.UpdateSubscriber(x, () => expEngine.ParseAndCompile<double[]>(x.ExpressionFormulas.GetExpression())));

            // Calculate here
            calculator.Caculate(subscribers);

            var iInputExpression = 9;
            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 10);

            outboundMessages
                .OutboundMessages
                .ForEach(outMsg =>
                    {
                        var msgOutbound = outMsg.Message as PinkoMsgCalculateExpressionResult;

                        Assert.IsNotNull(msgOutbound);
                        Assert.IsTrue(outMsg.QueueName.Equals( pinkoContainer.Resolve<IPinkoConfiguration>().PinkoMessageBusToWebRoleCalcResultTopic));

                        // Assure exception message was sent out to subscriber
                        Assert.IsFalse(string.IsNullOrEmpty(outMsg.ErrorSystem));
                        Assert.IsTrue(outMsg.ErrorCode == PinkoErrorCode.UnexpectedException);
                        Assert.IsTrue(outMsg.ErrorDescription == "Test Invoke Exception");

                        Assert.IsTrue(msgOutbound.DataFeedIdentifier.IsEqual(inputResults[iInputExpression].DataFeedIdentifier));
                        Assert.IsTrue(msgOutbound.TimeSequence == PinkoApplicationMock.TestSequenceValue);

                        iInputExpression--;
                    });
        }


        /// <summary>
        /// Test parallel calculations - success
        /// </summary>
        [TestMethod]
        public void TestCalculatePreparedListSuccess()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var expEngine = PinkoExpressionEngineFactory.GetNewEngine();
            pinkoContainer.RegisterInstance(expEngine);
            var subscribers = pinkoContainer.Resolve<PinkoExpressionSubscribers<double[]>>();
            var calculator = pinkoContainer.Resolve<PinkoCalculator<double[]>>();
            var outboundMessages = pinkoContainer.Resolve<OutbountListener<IBusMessageOutbound>>();

            // Load formulas
            var inputResults = SampleMockData.GetPinkoMsgCalculateExpressionResult(10);
            inputResults.ForEach(x => subscribers.UpdateSubscriber(x, () => expEngine.ParseAndCompile<double[]>(x.ExpressionFormulas.GetExpression())));

            // Calculate here
            calculator.Caculate(subscribers);

            Assert.IsTrue(outboundMessages.OutboundMessages.Count == 10);

            var envelopOutbound = outboundMessages.OutboundMessages.Last();
            var msgOutbound = envelopOutbound.Message as PinkoMsgCalculateExpressionResult;

            Assert.IsTrue(envelopOutbound.QueueName.Equals(pinkoContainer.Resolve<IPinkoConfiguration>().PinkoMessageBusToWebRoleCalcResultTopic));

            Assert.IsTrue(msgOutbound.ExpressionFormulas.SequenceEqual(inputResults.First().ExpressionFormulas, PinkoUserExpressionFormulaExtensions.Comparer));
            Assert.IsTrue(msgOutbound.TimeSequence == PinkoApplicationMock.TestSequenceValue);
            Assert.IsTrue(msgOutbound.ResultType == PinkoCalculateExpressionDaoExtensions.ResultDouble);
            Assert.IsTrue(msgOutbound.DataFeedIdentifier.IsEqual(inputResults.First().DataFeedIdentifier));

            // Check some real result
            Assert.IsTrue(msgOutbound.ResultsTupples.Count() == 10);
            Assert.IsTrue(Math.Round(msgOutbound.ResultsTupples[0].PointSeries[0].PointValue, 4) == 0.0200);
            Assert.IsTrue(Math.Round(msgOutbound.ResultsTupples[1].PointSeries[0].PointValue, 4) == 1.32);
            Assert.IsTrue(Math.Round(msgOutbound.ResultsTupples[2].PointSeries[0].PointValue, 4) == 4.62);
            Assert.IsTrue(Math.Round(msgOutbound.ResultsTupples[3].PointSeries[0].PointValue, 4) == 9.92);
            Assert.IsTrue(Math.Round(msgOutbound.ResultsTupples[4].PointSeries[0].PointValue, 4) == 17.22);
            Assert.IsTrue(Math.Round(msgOutbound.ResultsTupples[5].PointSeries[0].PointValue, 4) == 26.52);
            Assert.IsTrue(Math.Round(msgOutbound.ResultsTupples[6].PointSeries[0].PointValue, 4) == 37.82);
            Assert.IsTrue(Math.Round(msgOutbound.ResultsTupples[7].PointSeries[0].PointValue, 4) == 51.12);
            Assert.IsTrue(Math.Round(msgOutbound.ResultsTupples[8].PointSeries[0].PointValue, 4) == 66.42);
            Assert.IsTrue(Math.Round(msgOutbound.ResultsTupples[9].PointSeries[0].PointValue, 4) == 83.72);
        }


        [TestMethod]
        public void TestSetupSubscribersSetMarketEnv()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();

            // Setup collection
            var subscribers = pinkoContainer.Resolve<PinkoExpressionSubscribers<double[]>>();
            SampleMockData.GetPinkoMsgCalculateExpressionResult().Take(10).ForEach(x => subscribers.UpdateSubscriber(x, () => null));

            var calculator = pinkoContainer.Resolve<PinkoCalculator<double[]>>();

            var results = calculator.SetupSubscribers(subscribers);

            Assert.IsTrue(results.Count == 10);
            Assert.IsFalse(results.Any(x => x.PinkoMarketEnvironment.IsNull()));
        }
    }
}
