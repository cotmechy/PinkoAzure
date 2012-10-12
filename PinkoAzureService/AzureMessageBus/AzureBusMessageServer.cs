using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Practices.Unity;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using PinkoCommon.Interface;
using PinkoCommon.Utility;
using PinkoWorkerCommon.ExceptionTypes;

namespace PinkoAzureService.AzureMessageBus
{
    /// <summary>
    /// Specific implementation for Azure message bus cloud service
    /// </summary>
    public class AzureBusMessageServer : IBusMessageServer
    {
        /// <summary>
        /// Connect to single Queue
        /// </summary>
        public IBusMessageQueue ConnectToQueue(string queueName, string selector = "")
        {
            //Trace.TraceInformation("Connecting to Queue/Topic: {0} - selector: {2}  in {1}...", queueName, BusMessageServerConnectionString, selector);
            AzureQueueClient client = null;

            // create queue/topic
            var ex = TryCatch.RunInTry(() =>
                                  {
                                      // Create Queue
                                      if (!PinkoConfiguration.QueueConfiguration[queueName].Item2 && !_azureNamespaceManager.QueueExists(queueName))
                                      {
                                          Trace.TraceInformation("Connecting to Queue: {0} - selector: {2}  in {1}...", queueName, BusMessageServerConnectionString, selector);
                                          var td = new QueueDescription(queueName) { MaxSizeInMegabytes = 5120, DefaultMessageTimeToLive = new TimeSpan(0, 0, 30) };
                                          _azureNamespaceManager.CreateQueue(td);
                                      }

                                      // Create Topic
                                      if (PinkoConfiguration.QueueConfiguration[queueName].Item2)
                                      {
                                          Trace.TraceInformation("Connecting to Topic: {0} - selector: {2}  in {1}...", queueName, BusMessageServerConnectionString, selector);
                                          var td = new TopicDescription(queueName)
                                                       {
                                                           MaxSizeInMegabytes = 5120,
                                                           DefaultMessageTimeToLive = new TimeSpan(0, 0, 20)
                                                       };

                                          if (!_azureNamespaceManager.TopicExists(queueName))
                                              _azureNamespaceManager.CreateTopic(td);
                                      }
                                  });
            if (null != ex)
            {
                var newExce = new PinkoExceptionQueueNotConfigured(queueName);
                newExce.Data["OriginalException"] = ex;
                throw newExce;
            }

            // Initialize the connection to Service Bus Queue
            client = PinkoContainer.Resolve<AzureQueueClient>();
            client.QueueName = queueName;
            client.AzureNamespaceManager = _azureNamespaceManager;
            client.Initialize(BusMessageServerConnectionString, selector);
            return client;
        }

        /// <summary>
        /// Get existing or new queue
        /// </summary>
        public IBusMessageQueue GetTopic(string queueName, string selector = "")
        {
            //Trace.WriteLine(string.Format("GetQueue: {0} in {1}...", queueName, AzureServerConnectionString));

            // Get queue
            return _queues.GetOrAdd(queueName, x => ConnectToQueue(queueName, selector));
        }

        /// <summary>
        /// Initialize message bus
        /// </summary>
        public IBusMessageServer Initialize()
        {
            TryCatch.RunInTryThrow(() =>
                {
                    // Set the maximum number of concurrent connections 
                    ServicePointManager.DefaultConnectionLimit = 12;

                    // Create the queue if it does not exist already
                    BusMessageServerConnectionString = PinkoConfiguration.GetSetting("Microsoft.ServiceBus.ConnectionString");

                    Trace.TraceInformation("Creating AzureNamespaceManager: {0}...", BusMessageServerConnectionString);
                    _azureNamespaceManager = NamespaceManager.CreateFromConnectionString(BusMessageServerConnectionString);

                    // Set listener for outbound messages 
                    PinkoApplication.GetSubscriber<IBusMessageOutbound>()
                        .ObserveOn(PinkoApplication.ThreadPoolScheduler)
                        .Do(x => Trace.TraceInformation("AzureBusMessageServer Sending: {0}", x.Verbose()))
                        .Subscribe(x => GetTopic(x.QueueName).Send(x));
                });

            return this;
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
        /// IPinkoConfiguration
        /// </summary>
        [Dependency]
        public IPinkoConfiguration PinkoConfiguration { private get; set; }

        /// <summary>
        /// Internal queue cache
        /// </summary>
        readonly ConcurrentDictionary<string, IBusMessageQueue> _queues = new ConcurrentDictionary<string, IBusMessageQueue>();

        /// <summary>
        /// Server connection string
        /// </summary>
        public string BusMessageServerConnectionString { get; set; }

        /// <summary>
        /// Azure messaging server Namespace
        /// </summary>
        private NamespaceManager _azureNamespaceManager;
    }
}
