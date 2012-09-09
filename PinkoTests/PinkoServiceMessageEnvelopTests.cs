using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkoCommon;
using PinkoCommon.Interface;
using PinkoMocks;
using PinkoWebRoleCommon.Extensions;

namespace PinkoTests
{
    /// <summary>
    /// Summary description for PinkoServiceMessageEnvelopTests
    /// </summary>
    [TestClass]
    public class PinkoServiceMessageEnvelopTests
    {
        [TestMethod]
        public void TestFactorWebEnvelop()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var pinkoApplication = pinkoContainer.Resolve<IPinkoApplication>();

            var envelop = pinkoApplication.FactorWebEnvelop("clientId", "webRoleId");

            Assert.IsTrue(envelop.PinkoProperties.ContainsKey(PinkoMessagePropTag.ClientId));
            Assert.IsTrue(envelop.PinkoProperties.ContainsKey(PinkoMessagePropTag.WebRoleId));
            Assert.IsTrue(envelop.PinkoProperties.ContainsKey(PinkoMessagePropTag.MachineName));
            Assert.IsTrue(envelop.PinkoProperties.ContainsKey(PinkoMessagePropTag.SenderName));
            Assert.IsTrue(envelop.PinkoProperties.ContainsKey(PinkoMessagePropTag.DateTimeStamp));
        }
    }
}
