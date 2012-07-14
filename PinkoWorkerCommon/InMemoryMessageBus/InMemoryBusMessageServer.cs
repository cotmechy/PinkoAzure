using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using PinkoWorkerCommon.Interface;

namespace PinkoWorkerCommon.InMemoryMessageBus
{
    /// <summary>
    /// In Memory bus. Us ein offline development.
    /// </summary>
    public class InMemoryBusMessageServer : IBusMessageServer
    {
        /// <summary>
        /// Constructor - InMemoryBusMessageServer 
        /// </summary>
        public InMemoryBusMessageServer()
        {
            AzureServerConnectionString = "InMemoryBusMessageServer";
        }

        /// <summary>
        /// Connect to single Queue
        /// </summary>
        public IBusMessageQueue ConnectToQueue(string queueName)
        {
            // Initialize the connection to Service Bus Queue
            var client = _queues.GetOrAdd(queueName, x =>
                                                         {
                                                             Trace.TraceInformation("Creating new InMemoryBusMessageQueue: {0} in {1}...", queueName, AzureServerConnectionString);

                                                             var q = PinkoContainer.Resolve<InMemoryBusMessageQueue>();
                                                             q.QueueName = queueName;
                                                             q.Initialize(string.Empty);
                                                             return q;
                                                         });
            return client;
        }


        /// <summary>
        /// Get existing or new queue
        /// </summary>
        public IBusMessageQueue GetQueue(string queueName)
        {
            //Trace.WriteLine(string.Format("GetQueue: {0} in {1}...", queueName, AzureServerConnectionString));

            // Get queue
            return ConnectToQueue(queueName);
        }


        /// <summary>
        /// Initialize message bus
        /// </summary>
        public void Initialize()
        {
            //_applicationBusMessageSend = PinkoApplication.GetSubscriber<IBusMessageOutbound>();

            // Set listener for outboundmessages 
            PinkoApplication.GetSubscriber<IBusMessageOutbound>()
                .ObserveOn(Scheduler.ThreadPool)
                .Do(x => Trace.TraceInformation("(InMemoryBusMessageServer) Sending: {0}", x.Verbose()))
                .Subscribe(x => GetQueue(x.QueueName).Send(x));
        }


        /// <summary>
        /// Deinitialize message bus
        /// </summary>
        public void Deinitialize()
        {
        }

        /// <summary>
        /// IUnityContainer
        /// </summary>
        [Dependency]
        public IUnityContainer PinkoContainer { get; set; }

        /// <summary>
        /// IPinkoApplication
        /// </summary>
        [Dependency]
        public IPinkoApplication PinkoApplication { get; set; }

        /// <summary>
        /// Server connection string
        /// </summary>
        public string AzureServerConnectionString { get; set; }

        ///// <summary>
        ///// Receive all message vis this Rxmemory bus
        ///// </summary>
        //private IObservable<IBusMessageOutbound> _applicationBusMessageSend;


        /// <summary>
        /// Queus 
        /// </summary>
        public ConcurrentDictionary<string, InMemoryBusMessageQueue> Queues { get { return _queues; } }

        /// <summary>
        /// Internal queue cache
        /// </summary>
        readonly ConcurrentDictionary<string, InMemoryBusMessageQueue> _queues = new ConcurrentDictionary<string, InMemoryBusMessageQueue>();
    }
}
