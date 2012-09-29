using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoExpressionCommon;
using PinkoExpressionEngine;
using PinkoMocks;
using Microsoft.Practices.Unity;

namespace PinkoTests
{
    /// <summary>
    /// Summary description for PinkoExpressionTests
    /// </summary>
    [TestClass]
    public class PinkoExpressionTests
    {

        /// <summary>
        /// PinkoDataFeedIdentifier - CopyTo()
        /// </summary>
        [TestMethod]
        public void TestPinkoDataFeedIdentifierCopyTo()
        {
            var expression = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            var newone = expression.CopyTo(new PinkoMsgCalculateExpression());

            Assert.IsTrue(expression.IsEqual(newone));
        }


        /// <summary>
        /// AreEqual
        /// </summary>
        [TestMethod]
        public void TestAreEqual()
        {
            var val1 = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            Assert.IsTrue(val1.IsEqual(val1));

            var val2 = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            Assert.IsTrue(val1.IsEqual(val2));

            val2 = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            val2.DataFeedIdentifier.SubscribtionId = "--- Different ----";
            Assert.IsFalse(val1.IsEqual(val2));

            val2 = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            val2.DataFeedIdentifier.SignalRId = "--- Different ----";
            Assert.IsFalse(val1.IsEqual(val2));

            val2 = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            val2.DataFeedIdentifier.WebRoleId = "--- Different ----";
            Assert.IsFalse(val1.IsEqual(val2));

            val2 = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            val2.DataFeedIdentifier.ClientId = "--- Different ----";
            Assert.IsFalse(val1.IsEqual(val2));

            val2 = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            val2.DataFeedIdentifier.MaketEnvId = "--- Different ----";
            Assert.IsFalse(val1.IsEqual(val2));
        }

        /// <summary>
        /// expression syntax
        /// </summary>
        [TestMethod]
        public void TestExpressionBuild()
        {
            var msgObj = new PinkoMsgCalculateExpression
            {
                ExpressionFormulas = SampleMockData.GetPinkoUserExpressionFormula(3),
            };

            var expression = msgObj.GetExpression();
            Assert.IsTrue(expression.Replace(" ", string.Empty) == "{ Lbl_1000 = 1000.1 * 1000.2; Lbl_1001 = 1001.1 * 1001.2; Lbl_1002 = 1002.1 * 1002.2; [  Lbl_1000, Lbl_1001, Lbl_1002 ]  }".Replace(" ", string.Empty));
        }

        /// <summary>
        /// Basic Calculate history spread for same instrument multiple results
        /// </summary>
        [TestMethod]
        public void TestGetHistorySpreadZeroMultipleReults()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var marketEnv = pinkoContainer.Resolve<IPinkoMarketEnvironment>();
            var dal = marketEnv.PinkoDataAccessLayer as PinkoDataAccessLayerMock;
            var expEngine = PinkoExpressionEngineFactory.GetNewEngine();

            var complExp = expEngine.ParseAndCompile<double[][]>("{ " +
                                                               "A = RHist(\"Symbol\", \"IBM\", \"Price.Bid\", \"Reuters\", \"Hour\", 360) - RHist(\"Symbol\", \"MSFT\", \"Price.Bid\", \"Reuters\", \"Hour\", 360);" +
                                                               "B = RHist(\"Symbol\", \"IBM\", \"Price.Bid\", \"Reuters\", \"Hour\", 360) - RHist(\"Symbol\", \"MSFT\", \"Price.Bid\", \"Reuters\", \"Hour\", 360);" +
                                                               "C = A + B;" +
                                                               " [A, B, C]} ");
            var result = expEngine.Invoke(marketEnv, complExp);
            Assert.IsTrue(result[0].All(x => x != 0d));
            Assert.IsTrue(result[1].All(x => x != 0d));
            Assert.IsTrue(result[2].All(x => x != 0d));

            Assert.IsTrue(result[0].Select(x => Math.Round(x, 4)).SequenceEqual((new[] { -0.99, 2.177, -0.3491, 1.9579, 0.516, 0.0606999999999989, -0.6835, -0.503400000000001, -0.388, -0.416270000000001, 0.811400000000001, 0.202299999999999, -0.3093, -0.223429000000001 }).Select(x => Math.Round(x, 4))));
            Assert.IsTrue(result[1].Select(x => Math.Round(x, 4)).SequenceEqual((new[] { -0.99, 2.177, -0.3491, 1.9579, 0.516, 0.0606999999999989, -0.6835, -0.503400000000001, -0.388, -0.416270000000001, 0.811400000000001, 0.202299999999999, -0.3093, -0.223429000000001 }).Select(x => Math.Round(x, 4))));
            Assert.IsTrue(result[2].Select(x => Math.Round(x, 4)).SequenceEqual((new[] { -1.98, 4.354, -0.6982, 3.9158, 1.032, 0.121399999999998, -1.367, -1.0068, -0.776, -0.832540000000002, 1.6228, 0.404599999999999, -0.618600000000001, -0.446858000000002 }).Select(x => Math.Round(x, 4))));
        }

        /// <summary>
        /// Double array
        /// </summary>
        [TestMethod]
        public void TestMultipledoubles()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var marketEnv = pinkoContainer.Resolve<IPinkoMarketEnvironment>();
            var expEngine = PinkoExpressionEngineFactory.GetNewEngine();

            var complExp = expEngine.ParseAndCompile<double[]>("{ A = 3.1 + 1.1; B = A + 4.5; C = 2.4 + 3.2; [A,B,C]; }");
            //var complExp = expEngine.ParseAndCompile<double[]>("{ Lbl_1000 = 1000.1 * 1000.2; Lbl_1001 = 1001.1 * 1001.2; Lbl_1002 = 1002.1 * 1002.2; Lbl_1003 = 1003.1 * 1003.2; [  Lbl_1000, Lbl_1001, Lbl_1002, Lbl_1003 ]  }");

            var result = expEngine.Invoke(marketEnv, complExp);
            Assert.IsTrue(result.Count() == 3);
            Assert.IsTrue(result[0] == 4.2);
            Assert.IsTrue(result[1] == 8.7);
            Assert.IsTrue(result[2] == 5.6);
        }


        /// <summary>
        /// Simple add
        /// </summary>
        [TestMethod]
        public void TestBasicOperation()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var marketEnv = pinkoContainer.Resolve<IPinkoMarketEnvironment>();
            var expEngine = PinkoExpressionEngineFactory.GetNewEngine();
            //var dal = marketEnv.PinkoDataAccessLayer as PinkoDataAccessLayerMock;

            var complExp = expEngine.ParseAndCompile<double>(" { 3.1 + 1.1 } ");

            var result = expEngine.Invoke(marketEnv, complExp);
            Assert.IsTrue(4.2 == Math.Round(result, 4));
        }


        
        
        /// <summary>
        /// test constants
        /// </summary>
        [TestMethod]
        public void TestBasicConst()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var marketEnv = pinkoContainer.Resolve<IPinkoMarketEnvironment>();
            var expEngine = PinkoExpressionEngineFactory.GetNewEngine();
            var dal = marketEnv.PinkoDataAccessLayer as PinkoDataAccessLayerMock;

            var complExp = expEngine.ParseAndCompile<double>(" { 3.1 } ");

            var result = expEngine.Invoke<double>(marketEnv, complExp);
            Assert.IsTrue(3.1 == Math.Round(result, 4));
        }

        /// <summary>
        /// Run same formula with 2 diff data access layers
        /// </summary>
        [TestMethod]
        public void TestDataLayerChange()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var marketEnv = pinkoContainer.Resolve<IPinkoMarketEnvironment>();
            var expEngine = PinkoExpressionEngineFactory.GetNewEngine();
            var dal = marketEnv.PinkoDataAccessLayer as PinkoDataAccessLayerMock;

            var complExp = expEngine.ParseAndCompile<double>("{ RForm(\"Symbol\", \"IBM\", \"Price.Bid\", \"Reuters\") } ");

            var result = expEngine.Invoke(marketEnv, complExp);
            Assert.IsTrue(9.5679 == Math.Round(result, 4));

            // Change IPinkoDataAccessLayer value 
            dal.IbmPrice = 3.99;
            result = expEngine.Invoke(marketEnv, complExp);
            Assert.IsTrue(3.99 == Math.Round(result, 4));
        }

        /// <summary>
        /// Basic double expression
        /// </summary>
        [TestMethod]
        public void TestBasicDoubleExpression()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var marketEnv = pinkoContainer.Resolve<IPinkoMarketEnvironment>();
            var expEngine = PinkoExpressionEngineFactory.GetNewEngine();

            var complExp = expEngine.ParseAndCompile<double>("{ RForm(\"Symbol\", \"IBM\", \"Price.Bid\", \"Reuters\") } ");
            var result = expEngine.Invoke(marketEnv, complExp);

            Assert.IsTrue(9.5679 == Math.Round(result, 4));
        }

        /// <summary>
        /// Basic Calculate history spread for same instrument
        /// </summary>
        [TestMethod]
        public void TestGetHistorySpreadZero()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var marketEnv = pinkoContainer.Resolve<IPinkoMarketEnvironment>();
            var dal = marketEnv.PinkoDataAccessLayer as PinkoDataAccessLayerMock;
            var expEngine = PinkoExpressionEngineFactory.GetNewEngine();

            // ibm - msft
            var complExp = expEngine.ParseAndCompile<double[]>("{ RHist(\"Symbol\", \"IBM\", \"Price.Bid\", \"Reuters\", \"Hour\", 360) - RHist(\"Symbol\", \"IBM\", \"Price.Ask\", \"Reuters\", \"Hour\", 360); } ");
            var result = expEngine.Invoke(marketEnv, complExp);

            Assert.IsTrue(result.All(x => 0 == x));
        }


        /// <summary>
        /// Basic Calculate history spread
        /// </summary>
        [TestMethod]
        public void TestGetHistorySpread()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var marketEnv = pinkoContainer.Resolve<IPinkoMarketEnvironment>();
            var dal = marketEnv.PinkoDataAccessLayer as PinkoDataAccessLayerMock;
            var expEngine = PinkoExpressionEngineFactory.GetNewEngine();

            // ibm - msft
            var complExp = expEngine.ParseAndCompile<double[]>("{ RHist(\"Symbol\", \"IBM\", \"Price.Bid\", \"Reuters\", \"Hour\", 360) - RHist(\"Symbol\", \"MSFT\", \"Price.Ask\", \"Reuters\", \"Hour\", 360); } ");
            var result = expEngine.Invoke(marketEnv, complExp);

            int idx = 0;
            var expectedResult = dal.IbmSeries.Select(x => Math.Round(x - dal.MsftSeries[idx++], 4)).ToList();
            Assert.IsTrue(result.Select(x => Math.Round(x,4)) .SequenceEqual(expectedResult));
        }


        /// <summary>
        /// Basic get history
        /// </summary>
        [TestMethod]
        public void TestGetHistory()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var marketEnv = pinkoContainer.Resolve<IPinkoMarketEnvironment>();
            var dal = marketEnv.PinkoDataAccessLayer as PinkoDataAccessLayerMock;
            var expEngine = PinkoExpressionEngineFactory.GetNewEngine();

            // ibm
            var complExp = expEngine.ParseAndCompile<double[]>("{ RHist(\"Symbol\", \"IBM\", \"Price.Bid\", \"Reuters\", \"Hour\", 360) } ");
            var result = expEngine.Invoke(marketEnv, complExp);
            Assert.IsTrue(dal.IbmSeries.SequenceEqual(result));
            Assert.IsFalse(dal.MsftSeries.SequenceEqual(result));
            Assert.IsFalse(dal.InvalidSeries.SequenceEqual(result));

            // msft
            complExp = expEngine.ParseAndCompile<double[]>("{ RHist(\"Symbol\", \"MSFT\", \"Price.Bid\", \"Reuters\", \"Hour\", 360) } ");
            result = expEngine.Invoke(marketEnv, complExp);
            Assert.IsFalse(dal.IbmSeries.SequenceEqual(result));
            Assert.IsTrue(dal.MsftSeries.SequenceEqual(result));
            Assert.IsFalse(dal.InvalidSeries.SequenceEqual(result));

            // Invalid
            complExp = expEngine.ParseAndCompile<double[]>("{ RHist(\"Symbol\", \"Invalid\", \"Price.Bid\", \"Reuters\", \"Hour\", 360) } ");
            result = expEngine.Invoke(marketEnv, complExp);
            Assert.IsFalse(dal.IbmSeries.SequenceEqual(result));
            Assert.IsFalse(dal.MsftSeries.SequenceEqual(result));
            Assert.IsTrue(dal.InvalidSeries.SequenceEqual(result));
        }


    }
}
