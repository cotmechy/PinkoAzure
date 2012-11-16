using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoMocks;

namespace PinkoTests
{
    [TestClass]
    public class PinkoDataFeedIdentifierTests
    {
        [TestMethod]
        public void TestPartial()
        {
            var identifier = SampleMockData.GetPinkoDataFeedIdentifier(1)[0];
            var identifier2 = SampleMockData.GetPinkoDataFeedIdentifier(2)[1];

            var mixed = identifier.PartialClone(identifier2);

            Assert.IsTrue(mixed.ClientId == identifier.ClientId);
            Assert.IsTrue(mixed.ClientCtx == identifier.ClientCtx);
            Assert.IsTrue(mixed.MaketEnvId == identifier.MaketEnvId);
            Assert.IsTrue(mixed.SubscribtionId == identifier.SubscribtionId);

            Assert.IsTrue(mixed.SignalRId == identifier2.SignalRId);
            Assert.IsTrue(mixed.WebRoleId == identifier2.WebRoleId);
            Assert.IsTrue(mixed.PreviousSignalRId == identifier2.PreviousSignalRId);
            Assert.IsTrue(mixed.PreviousWebRoleId == identifier2.PreviousWebRoleId);

        }
    }
}
