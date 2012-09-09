using System;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoCommon.Interface;
using PinkoMocks;
using PinkoWebRoleCommon.IoC;
using PinkoWebService.Controllers;

namespace PinkoWebService.Tests.Controllers
{
    [TestClass]
    public class PinkoDictionaryControllerTests
    {
        [TestMethod]
        public void PinkoDictionaryControllerGet()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var pinkoApplication = pinkoContainer.Resolve<IPinkoApplication>();
            var controller = pinkoContainer.Resolve<PinkoDictionaryController>();

            var result = controller.GetPublicDictionary().ToList();

            Assert.IsTrue(result.Count == SampleMockData.GetPinkoFormulaMocks().Count);
        }



    }
}
