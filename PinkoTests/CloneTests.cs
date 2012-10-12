using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoMocks;

namespace PinkoTests
{
    [TestClass]
    public class CloneTests
    {
        [TestMethod]
        public void TestPinkoMsgCalculateExpressionToResult()
        {
            var src = SampleMockData.GetPinkoMsgCalculateExpression()[0];
            var clone = src.CopyTo(new PinkoMsgCalculateExpressionResult());

            Assert.IsFalse(src.DataFeedIdentifier.GetHashCode() == clone.DataFeedIdentifier.GetHashCode());
            Assert.IsFalse(src.ExpressionFormulas.GetHashCode() == clone.ExpressionFormulas.GetHashCode());
            Assert.IsTrue(src.ResultType == clone.ResultType);
            Assert.IsTrue(clone.ResultsTupple.GetHashCode() == PinkoMsgCalculateExpressionResultExtensions.DefaultResultTupple.GetHashCode());
        }


        [TestMethod]
        public void TestPinkoDataFeedIdentifier()
        {
            var src = SampleMockData.GetPinkoMsgCalculateExpression()[0].DataFeedIdentifier;
            var clone = src.DeepClone();

            Assert.IsFalse(src.GetHashCode() == clone.GetHashCode());
        }

        [TestMethod]
        public void TestPinkoFormPoint()
        {
            var src = SampleMockData.GetPinkoMsgCalculateExpressionResult()[0].ResultsTupple[0].PointSeries;
            var clone = src.DeepClone();

            Assert.AreNotSame(src,clone);
        }

        [TestMethod]
        public void TestResultsTuppleWrapper()
        {
            var src = SampleMockData.GetResultsTuppleWrapper().ToArray();
            var clone = src.DeepClone();

            Assert.IsFalse(AreSame(src, clone));
        }

        [TestMethod]
        public void TestPinkoUserExpressionFormula()
        {
            var src = SampleMockData.GetPinkoUserExpressionFormula();
            var clone = src.DeepClone();

            Assert.IsFalse(AreSame(src, clone));
        }


        static bool AreSame<T>(IEnumerable<T> src, IEnumerable<T> deest)
        {
            var areSame = true;

            var left = src.ToList();
            var right = deest.ToList();

            for (var idx = 0; idx < right.Count && areSame; idx++)
                if (left[idx].GetHashCode() != right[idx].GetHashCode())
                    areSame = false;

            return areSame;
        }
    }
}
