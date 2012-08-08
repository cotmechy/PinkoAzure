using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkoExpressionCommon;
using PinkoExpressionCommon.Interface;
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
        [TestMethod]
        public void TestBasicDoubleExpression()
        {
            var pinkoContainer = PinkoContainerMock.GetMokContainer();
            var marketEnv = pinkoContainer.Resolve<IPinkoMarketEnvironment>();
            var expEngine = PinkoExpressionEngineFactory.GetNewEngine(marketEnv.PinkoDataAccessLayer);

            var complExp = expEngine.ParseAndCompile("{ RForm(\"Symbol\", \"IBM\", \"Price.Bid\", \"Reuters\") } ");
            var result = expEngine.CalcExp(marketEnv, complExp);

            Assert.IsTrue(9.3765322 == result);
        }
    }
}
