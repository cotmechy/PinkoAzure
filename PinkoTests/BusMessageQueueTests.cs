using System;
using System.Diagnostics;
using System.Messaging;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoMsMqServiceBus;
using PinkoCommon;
using PinkoCommon.Interface;
using PinkoMocks;
using Microsoft.Practices.Unity;

namespace PinkoTests
{
    /// <summary>
    /// Summary description for BusMessageQueueTests
    /// </summary>
    [TestClass]
    public class BusMessageQueueTests
    {

        //
        // http://stackoverflow.com/questions/8005892/using-reactive-extension-rx-for-msmq-message-receive-using-async-pattern-queu
        //
        //var queue = new System.Messaging.MessageQueue("test");
        //var fun = Observable.FromAsyncPattern((cb, obj) => queue.BeginReceive(TimeSpan.FromMinutes(10), obj, cb), a => queue.EndReceive(a));
        //var obs = fun();

        /// <summary>
        /// Test Send message in queue - All types
        /// </summary>
        [TestMethod]
        public void TestMsMqServerSendReceivePinkoRoleHeartbeat()
        {
            // Each type needs to be added to 
            TestMsMqServerSendReceiveObject<PinkoRoleHeartbeat>(new PinkoRoleHeartbeat());
        }

        /// <summary>
        /// Test Send message in queue - All types
        /// </summary>
        [TestMethod]
        public void TestMsMqServerSendReceivePinkoPingMessage()
        {
            TestMsMqServerSendReceiveObject<PinkoPingMessage>(new PinkoPingMessage());
        }

        /// <summary>
        /// Test Send message in queue - All types
        /// </summary>
        [TestMethod]
        public void TestMsMqServerSendReceivePinkoCalculateExpression()
        {
            // Each type needs to be added to 
            TestMsMqServerSendReceiveObject<PinkoCalculateExpression>(new PinkoCalculateExpression());
        }

        /// <summary>
        /// Test Send message in queue - All types
        /// </summary>
        [TestMethod]
        public void TestMsMqServerSendReceivePinkoCalculateExpressionResult()
        {
            TestMsMqServerSendReceiveObject<PinkoCalculateExpressionResult>(new PinkoCalculateExpressionResult());
        }

        /// <summary>
        /// Test Send message in queue - Object
        /// </summary>
        public void TestMsMqServerSendReceiveObject<T>(object serializableObj)
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer(false);
            var pinkoApplication = pinkoContainer.Resolve<IPinkoApplication>();
            var ev = new ManualResetEvent(false);
            var messageServer = pinkoContainer.Resolve<MsMqBusMessageServer>().Initialize();

            pinkoContainer.RegisterInstance<IBusMessageServer>(messageServer);
            var config = pinkoContainer.Resolve<IPinkoConfiguration>();

            var incomingTopic = messageServer.GetTopic(config.PinkoMessageBusToWebAllRolesTopic);

            Task.Factory.StartNew(incomingTopic.Listen); // async listening

            ev.WaitOne(3000);

            Tuple<IBusMessageInbound, T> msg = null;
            incomingTopic.GetIncomingSubscriber<T>().Subscribe(x =>
                {
                    Debug.WriteLine("Received: " + x.ToString());
                    msg = x;
                    ev.Set();
                });

            var outboutMsg = new PinkoServiceMessageEnvelop(pinkoApplication)
            {
                Message = serializableObj,
                QueueName = config.PinkoMessageBusToWebAllRolesTopic
            };

            pinkoContainer
                .Resolve<IPinkoApplication>()
                .GetBus<IBusMessageOutbound>()
                .Publish(outboutMsg);

            pinkoApplication.ApplicationRunningEvent.Set();

            if (ev.WaitOne(20000))
                Debug.WriteLine("Test Signaled ...");
            else
                Debug.WriteLine("Test Timeout ...");

            incomingTopic.GetIncomingSubscriber<T>().Subscribe().Dispose();

            Assert.IsNotNull(msg);
            Assert.IsNotNull(msg.Item1.PinkoProperties);
            Assert.IsTrue(msg.Item1.PinkoProperties.Count > 0);
            Assert.IsInstanceOfType(msg.Item2, serializableObj.GetType());
        }


        ///// <summary>
        ///// Test Send message in queue - String
        ///// </summary>
        //[TestMethod]
        //public void TestMsMqServerSendReceiveString()
        //{
        //    var pinkoContainer = PinkoContainerMock.GetMockContainer(false);
        //    var ev = new ManualResetEvent(false);
        //    var messageServer = pinkoContainer.Resolve<MsMqBusMessageServer>().Initialize();
        //    pinkoContainer.RegisterInstance<IBusMessageServer>(messageServer);

        //    var config = pinkoContainer.Resolve<IPinkoConfiguration>();

        //    var incomingTopic = messageServer.GetTopic(config.PinkoMessageBusToWebAllRolesTopic);

        //    Task.Factory.StartNew(incomingTopic.Listen); // async listening

        //    ev.WaitOne(3000);

        //    Tuple<IBusMessageInbound, string> msg = null;
        //    incomingTopic.GetIncomingSubscriber<string>().Subscribe(x =>
        //        {
        //            msg = x;
        //            ev.Set();
        //        });

        //    var outboutMsg = new PinkoServiceMessageEnvelop()
        //        {
        //            Message = "StringUnitTestMessage",
        //            QueueName = config.PinkoMessageBusToWebAllRolesTopic
        //        };

        //    pinkoContainer
        //        .Resolve<IPinkoApplication>()
        //        .GetBus<IBusMessageOutbound>()
        //        .Publish(outboutMsg);

        //    ev.WaitOne(7000);

        //    Assert.IsNotNull(msg);
        //    Assert.IsTrue(msg.Item2.Equals("StringUnitTestMessage"));
        //}
        
        /// <summary>
        /// Test get queue
        /// </summary>
        [TestMethod]
        public void TestMsMqServerGetQueue()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var ev = new ManualResetEvent(false);
            var messageServer = pinkoContainer.Resolve<MsMqBusMessageServer>().Initialize() as MsMqBusMessageServer;
            pinkoContainer.RegisterInstance(messageServer);

            var config = pinkoContainer.Resolve<IPinkoConfiguration>();
            
            // Get Topic
            var incomingTopic = messageServer.GetTopic(config.PinkoMessageBusToWebAllRolesTopic);

            Assert.IsNotNull(incomingTopic);
            Assert.IsTrue(incomingTopic.QueueName.Equals(config.PinkoMessageBusToWebAllRolesTopic));
            Assert.IsNotNull(messageServer.Queues.Count == 1);
        }



        /// <summary>
        /// Basic double expression
        /// </summary>
        [TestMethod]
        public void TestMessageQueue()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var ev = new ManualResetEvent(false);

            const string testQueue = @".\private$\PinkoUbnistTestTopic";

            var recvdMessage = string.Empty;

            Task.Factory.StartNew(() =>
                {
                    // start a new queue and listen for a message
                    //MessageQueue.Create(testQueue); //".\\PinkoUbnistTestTopic");
                    var msgQueueReceive = new MessageQueue(testQueue)
                        {
                            Formatter = new XmlMessageFormatter( new Type[] {typeof (string)} )
                        };

                    //var queues = MessageQueue.GetPrivateQueuesByMachine(".");
                    //queues.ToList().ForEach(x => Debug.WriteLine(">>" + x.QueueName));

                    // Set the formatter to indicate body contains an Order.

                    var msg = msgQueueReceive.Receive();
                    recvdMessage = (string) msg.Body;
                });

            ev.WaitOne(2000);

            var msgQueue = new MessageQueue(testQueue);
            msgQueue.Send("msg");

            ev.WaitOne(2000);

            Assert.IsFalse(string.IsNullOrEmpty(recvdMessage));
        }

        /// <summary>
        /// Basic double expression
        /// </summary>
        [TestMethod]
        public void TestGetGetIncomingSubscriber()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var messageBus = pinkoContainer.Resolve<IBusMessageServer>();
            var config = pinkoContainer.Resolve<IPinkoConfiguration>();

            var incomingTopic = messageBus.GetTopic(config.PinkoMessageBusToWebAllRolesTopic);

            Tuple<IBusMessageInbound, string> msg = null;
            incomingTopic.GetIncomingSubscriber<string>().Subscribe(x => msg = x);

            // Simulate incoming topic message
            incomingTopic.Send(new PinkoServiceMessageEnvelop()
                                   {
                                       Message = "SringIncomingMessageTest"
                                   });

            Assert.IsNotNull(msg);
            Assert.IsTrue(msg.Item2.Equals("SringIncomingMessageTest"));
        }
    }
}
