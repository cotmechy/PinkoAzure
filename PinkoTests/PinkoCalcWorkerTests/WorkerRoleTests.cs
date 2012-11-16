using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoCommon;
using PinkoCommon.Interface;
using PinkoMocks;
using PinkoWorkerCommon;
using PinkoWorkerCommon.Interface;

namespace PinkoTests.PinkoCalcWorkerTests
{
    /// <summary>
    /// Summary description for WorkerRoleTests
    /// </summary>
    [TestClass]
    public class WorkerRoleTests
    {
        /// <summary>
        /// Test Heartbeat
        /// </summary>
        [TestMethod]
        public void HeartBeatTest()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var pinkoApplication = pinkoContainer.Resolve<IPinkoApplication>();
            var busMessageServer = pinkoContainer.Resolve<IBusMessageServer>();
            var pinkoConfiguration = pinkoContainer.Resolve<IPinkoConfiguration>();
            busMessageServer.Initialize();

            //
            // Factor new worker and replace container with mock.
            // Mimic Azure factor the worker role host
            var workerRole = pinkoContainer.Resolve<WorkerRoleFrame>();
            workerRole.PinkoContainer = pinkoContainer;

            // start heartbeat
            ((PinkoConfiguration)pinkoConfiguration).HeartbeatIntervalSec = 1.0;
            pinkoContainer.RegisterInstance<IWorkerRoleHeartBeat>(pinkoContainer.Resolve<WorkerRoleHeartBeat>().Initialize());
            pinkoApplication.ApplicationRunningEvent.WaitOne(6000);

            var outboudMessages = busMessageServer.GetTopic(pinkoConfiguration.PinkoMessageBusToWebRolesAllTopic).OutboudMessages;
            Assert.IsTrue(outboudMessages >= 5);
        }


    //    /// <summary>
    //    /// Factor WorkerRole with InMemoryBusMessageServer for message flow test
    //    /// </summary>
    //    [TestMethod]
    //    public void FactorWorkerRoleWithInMemoryBusMessageServer()
    //    {
    //        var pinkoContainer = PinkoContainerMock.GetMockContainer();
    //        var pinkoApplication = pinkoContainer.Resolve<IPinkoApplication>();
    //        var ev = new ManualResetEvent(false);

    //        // Register real in memory message bus
    //        pinkoContainer.RegisterInstance<IBusMessageServer>(pinkoContainer.Resolve<InMemoryBusMessageServer>());
    //        pinkoContainer.Resolve<IBusMessageServer>().Initialize();

    //        //
    //        // Factor new worker and replace container with mock.
    //        // Mimic Azure factor the worker role host
    //        var workerRole = pinkoContainer.Resolve<WorkerRoleFrame>();
    //        Task.Factory.StartNew(workerRole.Run);       // Start the work role.
            
    //        ev.WaitOne(2000);

    //        // Monitor Inbound message. simulating incoming message.
    //        Tuple<IBusMessageInbound, PinkoPingMessage> receiveMessageInbound = null;
    //        pinkoApplication
    //            .GetSubscriber<Tuple<IBusMessageInbound, PinkoPingMessage>>()
    //            .Subscribe(x =>
    //                           {
    //                               receiveMessageInbound = x;
    //                               ev.Set();
    //                           });

    //        var pm = new PinkoPingMessage { SenderMachine = "ClientMachine", ResponderMachine = "ServerMachine" };

    //        // Send message
    //        pinkoApplication
    //            .GetBus<IBusMessageOutbound>()
    //            .Publish(new PinkoServiceMessageEnvelop()
    //                         {
    //                             //ContentType = typeof(PinkoPingMessage).ToString(),
    //                             Message = pm,
    //                             QueueName = pinkoContainer.Resolve<IPinkoConfiguration>().PinkoMessageBusToWorkerAllRolesTopic
    //                         });
    //        ev.WaitOne(2000);

    //        // Assure the worker role is setup properly and incoming/outgoing messages are flowing properly
    //        Assert.IsNotNull(receiveMessageInbound);
    //        Assert.IsInstanceOfType(receiveMessageInbound.Item2, typeof(PinkoPingMessage));
    //        Assert.IsTrue(receiveMessageInbound.Item2.SenderMachine.Equals("ClientMachine"));
    //        Assert.IsFalse(string.IsNullOrEmpty(receiveMessageInbound.Item1.ContentType));
    //        Assert.IsTrue(receiveMessageInbound.Item1.QueueName == pinkoContainer.Resolve<IPinkoConfiguration>().PinkoMessageBusToWorkerAllRolesTopic);
    //    }
   }
}
