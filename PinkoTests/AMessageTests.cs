using System;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoAzureService.AzureMessageBus;
using PinkoCommon;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.Extension;
using PinkoCommon.Interface;
using PinkoMocks;
using PinkoMsMqServiceBus;

namespace PinkoTests
{
    [TestClass]
    public class AMessageTests
    {
        /// <summary>
        /// Assure message types have proper serializer in Azure bus
        /// </summary>
        [TestMethod]
        public void TestAzureSerializer()
        {
            var serializers = AzureQueueClientExtensions.GetDeserializer();

            // Missing serializer
            PinkoMessageTypes
                .SupportedTypes
                .ForEach(x => Assert.IsTrue(serializers.ContainsKey(x.ToString())));
        }

        [TestMethod]
        public void TestMqSerializerInbound()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();

            var q = pinkoContainer.Resolve<MsMqBusMessageQueue>();
            q.QueueName = "UnitTest_TestMqSerializerInbound";
            q.Initialize(string.Empty, "selector");

            // Missing serializer
            PinkoMessageTypes
                .SupportedTypes
                .ForEach(x => Assert.IsTrue(q.MsmqFormatter.TargetTypes.Count(y => y == x) == 1));
        }


        [TestMethod]
        public void TestIIncominBusMessageHandlerManagerTypeHandler()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();

            var handler = pinkoContainer.Resolve<IIncominBusMessageHandlerManager>() as IncominBusMessageHandlerManager;

            // Missing serializer
            PinkoMessageTypes
                .SupportedTypes
                .ForEach(x => Assert.IsTrue(handler.CachedBusPublisher.ContainsKey(x.ToString())));

            // Add to container in mock
            //pinkoContainer.Resolve<IIncominBusMessageHandlerManager>().AddBusTypeHandler<PinkoMsgClientTimeout>();
        }

        //[TestMethod]
        //public void TestConainerListener()
        //{
        //    var pinkoContainer = PinkoContainerMock.GetMockContainer();

        //    var handler = pinkoContainer.Resolve<IIncominBusMessageHandlerManager>() as IncominBusMessageHandlerManager;

        //    // Missing serializer
        //    PinkoMessageTypes
        //        .SupportedTypes
        //        .ForEach(x => Assert.IsTrue(pinkoContainer.Registrations.Count(y => y.RegisteredType == x) == 1));
        //}

    }
}
