using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reactive.Linq;
using Microsoft.Practices.Unity;
using PinkoCommon.Interface;

namespace PinkoMsMqServiceBus
{
    /// <summary>
    /// MsMqBusMessageServer
    /// </summary>
    public class MsMqBusMessageServer : IBusMessageServer
    {
        /// <summary>
        /// Server connection string
        /// </summary>
        public string BusMessageServerConnectionString
        {
            get { return "MsMqBusMessageServerConnectionString"; }
        }

        /// <summary>
        /// Connect queue
        /// </summary>
        public IBusMessageQueue GetTopic(string queueName, string selector = "")
        {
            // Initialize the connection to Service Bus Queue
            var client = Queues.GetOrAdd(queueName, x =>
            {
                Trace.TraceInformation("Creating new MsMqBusMessageQueue: {0} - Selector: {1}...", queueName, selector);

                var q = PinkoContainer.Resolve<MsMqBusMessageQueue>();
                q.QueueName = queueName;
                q.Initialize(string.Empty, selector);
                return q;
            });

            return client;
        }

        /// <summary>
        /// Initialize message bus
        /// </summary>
        public IBusMessageServer Initialize()
        {
            // Set listener for outbound messages. 
            // this server will publish into the msmq message queue
            PinkoApplication.GetSubscriber<IBusMessageOutbound>()
                .ObserveOn(PinkoApplication.ThreadPoolScheduler)
                .Do(x => Trace.TraceInformation("(MsMqBusMessageServer) Sending: {0}", x.Verbose()))
                .Subscribe(x => GetTopic(x.QueueName).Send(x));

            return this;
        }

        /// <summary>
        /// Deinitialize message bus
        /// </summary>
        public void Deinitialize()
        {
            // nothing to do
        }

        /// <summary>
        /// Internal queue cache
        /// </summary>
        public readonly ConcurrentDictionary<string, IBusMessageQueue> Queues = new ConcurrentDictionary<string, IBusMessageQueue>();

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

    }
}
