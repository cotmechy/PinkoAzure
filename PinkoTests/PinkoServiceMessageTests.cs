using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkoMocks;
using PinkoWorkerCommon.Interface;
using PinkoWorkerCommon.Utility;
using Microsoft.Practices.Unity;

namespace PinkoTests
{
    /// <summary>
    /// Summary description for PinkoServiceMessageTests
    /// </summary>
    [TestClass]
    public class PinkoServiceMessageTests
    {
        ///// <summary>
        ///// test AzureQueueClient
        ///// </summary>
        //[TestMethod]
        //public void testAzureQueueClient()
        //{
        //    var pinkoContainer = PinkoContainerMock.GetMokContainer();
        //    var pinkoApplication = pinkoContainer.Resolve<IPinkoApplication>();

        //    var client = pinkoContainer.Resolve<AzureQueueClient>();
        //    client.Initialize();

        //    Task.Factory.StartNew(client.Listen);

        //    pinkoApplication.ApplicationRunningEvent.WaitOne(2000);

        //}
    }
}
