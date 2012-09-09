using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        /// Basic Caclulate hitory spread
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
