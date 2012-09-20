using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoAzureService.AzureMessageBus;
using PinkoCommon;
using PinkoCommon.Interface;
using PinkoMocks;
using Microsoft.Practices.Unity;
using PinkoWorkerCommon.Utility;

namespace PinkoTests.AzureTests
{
    /// <summary>
    /// Summary description for RealAzureConnectivityTests
    /// </summary>
    //[TestClass]
    public class RealAzureConnectivityTests
    {
        /// <summary>
        /// Test Connecting to Topic
        /// </summary>
        //[TestMethod]
        public void ConnectingTopicTest()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var pinkoConfiguration = pinkoContainer.Resolve<IPinkoConfiguration>();

            // Connect to service
            var server = pinkoContainer.Resolve<AzureBusMessageServer>();
            pinkoContainer.RegisterInstance<IBusMessageServer>(server);
            server.Initialize();

            // Connect to queue
            const string queueName = "UnitTestTopicName";
            pinkoConfiguration.QueueConfiguration[queueName] = new Tuple<string, bool>(queueName, false /* Topic */);
            var queue = server.ConnectToQueue(queueName);

            queue.Close();
            server.Deinitialize();
        }

        /// <summary>
        /// Test Connecting to Queue
        /// </summary>
        //[TestMethod]
        public void ConnectingQueueTest()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var pinkoConfiguration = pinkoContainer.Resolve<IPinkoConfiguration>();

            // Connect to service
            var server = pinkoContainer.Resolve<AzureBusMessageServer>();
            pinkoContainer.RegisterInstance<IBusMessageServer>(server);
            server.Initialize();

            // Connect to queue
            const string queuName = "UnitTestQueueName";
            pinkoConfiguration.QueueConfiguration[queuName] = new Tuple<string, bool>(queuName, true);
            var queue = server.ConnectToQueue(queuName);

            const string queuName2 = "UnitTestQueueName2";
            pinkoConfiguration.QueueConfiguration[queuName2] = new Tuple<string, bool>(queuName2, true);
            var queue2 = server.ConnectToQueue(queuName2);

            queue.Close();
            server.Deinitialize();
        }

        /// <summary>
        /// Connect to Azure Server amd semd message
        /// </summary>
        //[TestMethod]
        public void RealAzureMessageLoop()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var pinkoApplication = pinkoContainer.Resolve<IPinkoApplication>();
            var pinkoConfiguration = pinkoContainer.Resolve<IPinkoConfiguration>();
            var ev = new ManualResetEvent(false);
            Tuple<IBusMessageInbound, PinkoMsgPing> receiveMessageInbound = null;

            // Connect to service
            var server = pinkoContainer.Resolve<AzureBusMessageServer>();
            pinkoContainer.RegisterInstance<IBusMessageServer>(server);
            server.Initialize();

            // Connect to queue
            const string queuName = "UnitTestQueueName";
            pinkoConfiguration.QueueConfiguration[queuName] = new Tuple<string, bool>(queuName, true);
            var queue = server.ConnectToQueue(queuName);

            // Listen to messages
            Task.Factory.StartNew(queue.Listen);
            ev.WaitOne(1000);

            // hook  listener (no natureal C3 event). use RReactive Extensions (Rx)
            var listener = pinkoApplication.GetSubscriber<Tuple<IBusMessageInbound, PinkoMsgPing>>();
            listener.Subscribe(x =>
            {
                receiveMessageInbound = x;
                ev.Set();
            });

            // Create sample message
            var outboundMsg = new PinkoServiceMessageEnvelop()
            {
                Message = new PinkoMsgPing { SenderMachine = "ClientMachine", ResponderMachine = "ServerMachine" },
                QueueName = queuName
            };
            outboundMsg.PinkoProperties["key1"] = "key1Value1";
            outboundMsg.PinkoProperties["key2"] = "key1Value2";

            // simulate incoming message to teh WebRole
            //pinkoApplication
            //    .GetBus<Tuple<IBusMessageInbound, PinkoPingMessage>>()
            //    .Publish(new Tuple<IBusMessageInbound, PinkoPingMessage>(outboundMsg, outboundMsg.Message as PinkoPingMessage));
            pinkoApplication
                .GetBus<IBusMessageOutbound>()
                .Publish(outboundMsg);
            ev.WaitOne(20000);

            server.Deinitialize();
            queue.Close();

            Assert.IsNotNull(receiveMessageInbound);
            Assert.IsTrue(receiveMessageInbound.Item2.GetType() == typeof(PinkoMsgPing));

            // Assure properties were serialize by Azure layer
            Assert.IsTrue(receiveMessageInbound.Item1.PinkoProperties.ContainsKey("key1"));
            Assert.IsTrue(receiveMessageInbound.Item1.PinkoProperties.ContainsKey("key2"));
            Assert.IsTrue(receiveMessageInbound.Item1.PinkoProperties["key1"].Equals("key1Value1"));
            Assert.IsTrue(receiveMessageInbound.Item1.PinkoProperties["key2"].Equals("key1Value2"));
        }

    }
}
