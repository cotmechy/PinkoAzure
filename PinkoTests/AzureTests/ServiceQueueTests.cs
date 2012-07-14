using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoAzureService.AzureMessageBus;
using PinkoMocks;
using Microsoft.Practices.Unity;
using PinkoWorkerCommon.ExceptionTypes;
using PinkoWorkerCommon.Interface;
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
            var pinkoContainer = PinkoContainerMock.GetMokContainer();

            var client = pinkoContainer.Resolve<AzureQueueClient>();

            Assert.IsInstanceOfType(
                TryCatch.RunInTry( () => client.Send(new PinkoServiceMessageEnvelop()
                                {
                                    Message = new SerializableTestType(),
                                    ContentType = typeof (SerializableTestType).ToString()
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
            var pm = new PinkoPingMessage {SenderMachine = "ClientMachine", ResponderMachine = "ServerMachine"};
            var bm = AzureQueueClient.FactorNewOutboundMessage(new PinkoServiceMessageEnvelop() {Message = pm});

            // check for proepr deserialization
            var abm = new AzureBrokeredMessageEnvelopInbound(bm);
            Assert.IsNotNull(abm.Message);
            Assert.IsTrue(abm.Message.GetType() == typeof(PinkoPingMessage));
            Assert.IsTrue(bm.ContentType.Equals(typeof(PinkoPingMessage).ToString()));
        }

        /// <summary>
        /// Serializer/Deserialized inot Azure broker message - Error
        /// </summary>
        [TestMethod]
        public void BrokeredMessageSerializationError()
        {
            var pm = new SerializableTestType { ClientMachine = "ClientMachine", ServerMachine = "ServerMachine" };
            var bm = AzureQueueClient.FactorNewOutboundMessage(new PinkoServiceMessageEnvelop() { Message = pm });

            // check for proepr deserialization
            var abm = new AzureBrokeredMessageEnvelopInbound(bm);
            Assert.IsNull(abm.Message);
            Assert.IsTrue(bm.ContentType.Equals(typeof(SerializableTestType).ToString()));
        }



        /// <summary>
        /// Unreachable Azure server
        /// </summary>
        [TestMethod]
        public void RealAzureNotFound()
        {
            var pinkoContainer = PinkoContainerMock.GetMokContainer();

            // Connect to service
            var server = pinkoContainer.Resolve<AzureBusMessageServer>();
            server.Initialize();

            // Connect to queue
            //server.ConnectToQueue("NonExistingUnitTestQueueName");

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
