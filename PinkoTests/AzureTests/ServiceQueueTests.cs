using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoAzureService.AzureMessageBus;
using PinkoCommon;
using PinkoCommon.Interface;
using PinkoCommon.Utility;
using PinkoMocks;
using Microsoft.Practices.Unity;
using PinkoWorkerCommon.ExceptionTypes;
using PinkoWorkerCommon.Utility;

namespace PinkoTests.AzureTests
{
    /// <summary>
    /// Summary description for ServiceQueueTests
    /// </summary>
    [TestClass]
    public class ServiceQueueTests
    {
        /// <summary>
        /// Test Exception to cover non existing serializer in AzureQueueClient
        /// </summary>
        [TestMethod]
        public void AzureQueueClientMissingSerilalizer()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();

            var client = pinkoContainer.Resolve<AzureQueueClient>();

            Assert.IsInstanceOfType(
                TryCatch.RunInTry( () => client.Send(new PinkoServiceMessageEnvelop()
                                {
                                    Message = new SerializableTestType()
                                    //, ContentType = typeof (SerializableTestType).ToString()
                                })),
                typeof(PinkoExceptionAzureDeserializerNotFound));
        }

        //}
        /// <summary>
        /// Serializer/Deserialized inot Azure broker message
        /// </summary>
        [TestMethod]
        public void BrokeredMessageSerialization()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var pm = new PinkoMsgPing { SenderMachine = "ClientMachine", ResponderMachine = "ServerMachine" };
            var bm = AzureQueueClient.FactorNewOutboundMessage(new PinkoServiceMessageEnvelop() {Message = pm});

            // check for proepr deserialization
            var abm = new AzureBrokeredMessageEnvelopInbound(pinkoContainer.Resolve<IPinkoApplication>(), bm);
            Assert.IsNotNull(abm.Message);
            Assert.IsTrue(abm.Message.GetType() == typeof(PinkoMsgPing));
            Assert.IsTrue(bm.ContentType.Equals(typeof(PinkoMsgPing).ToString()));

            Assert.IsTrue(abm.PinkoProperties.ContainsKey(PinkoMessagePropTag.MachineName));
            Assert.IsTrue(abm.PinkoProperties.ContainsKey(PinkoMessagePropTag.SenderName));

        }

        /// <summary>
        /// Serializer/Deserialized inot Azure broker message - Error
        /// </summary>
        [TestMethod]
        public void BrokeredMessageSerializationError()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var pm = new SerializableTestType { ClientMachine = "ClientMachine", ServerMachine = "ServerMachine" };
            var bm = AzureQueueClient.FactorNewOutboundMessage(new PinkoServiceMessageEnvelop() { Message = pm });

            // check for proepr deserialization
            var abm = new AzureBrokeredMessageEnvelopInbound(pinkoContainer.Resolve<IPinkoApplication>(), bm);
            Assert.IsNull(abm.Message);
            Assert.IsTrue(bm.ContentType.Equals(typeof(SerializableTestType).ToString()));
        }



        /// <summary>
        /// Unreachable Azure server
        /// </summary>
        [TestMethod]
        public void RealAzureNotFound()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();

            // Connect to service
            var server = pinkoContainer.Resolve<AzureBusMessageServer>();
            server.Initialize();

            Assert.IsInstanceOfType( TryCatch.RunInTry(() => server.ConnectToQueue("NonExistingUnitTestQueueName")), typeof(PinkoExceptionQueueNotConfigured));

        }



        /// <summary>
        /// Internal test type
        /// </summary>
        public class SerializableTestType
        {
            public string ClientMachine = string.Empty;
            public string ServerMachine = string.Empty;
            public DateTime Created = DateTime.Now;
        }

    }
}
