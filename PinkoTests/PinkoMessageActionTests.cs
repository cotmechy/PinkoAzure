using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;

namespace PinkoTests
{
    [TestClass]
    public class PinkoMessageActionTests
    {
        [TestMethod]
        public void TestGoodString()
        {
            Assert.IsTrue(PinkoMessageAction.InvalidMessageAction == "".ToPinkoMessageAction());
            Assert.IsTrue(PinkoMessageAction.InvalidMessageAction == "CarlosString".ToPinkoMessageAction());
            Assert.IsTrue(PinkoMessageAction.InvalidMessageAction == PinkoMessageActionExtensions._maxActions.ToString().ToPinkoMessageAction());
            Assert.IsTrue(PinkoMessageAction.InvalidMessageAction == "100".ToPinkoMessageAction());

            Assert.IsTrue(PinkoMessageAction.UserSnapshot == "0".ToPinkoMessageAction());
            Assert.IsTrue(PinkoMessageAction.UserSubscription == "1".ToPinkoMessageAction());

            Assert.IsTrue("-1" == PinkoMessageAction.InvalidMessageAction.ToSerialString());
            Assert.IsTrue("0" == PinkoMessageAction.UserSnapshot.ToSerialString());
            Assert.IsTrue("1" == PinkoMessageAction.UserSubscription.ToSerialString());
        }
    }
}
