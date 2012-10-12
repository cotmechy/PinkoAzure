using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkoCommon.Extension;
using PinkoCommon.Extensions;

namespace PinkoTests
{
    [TestClass]
    public class OtherTests
    {
        [TestMethod]
        public void TestIsNull()
        {
            object obj = null;
            Assert.IsTrue(obj.IsNull());
        }
    }
}
