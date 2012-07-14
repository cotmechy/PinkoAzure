using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Practices.Unity;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using PinkoWorkerCommon.ExceptionTypes;
using PinkoWorkerCommon.Interface;
using PinkoWorkerCommon.Utility;

namespace PinkoAzureService.AzureMessageBus
{
    /// <summary>
    /// Specific implementation for Asuze message bus cloud service
    /// </summary>
    public class AzureBusMessageServer : IBusMessageServer
    {
        /// <summary>
        /// Connect to single Queuen
        /// </summary>
        public IBusMessageQueue ConnectToQueue(string queueName)
        {
            Trace.TraceInformation("Connectting to Queue/Topic: {0} in {1}...", queueName, AzureServerConnectionString);
            AzureQueueClient client = null;

            // create queue/topic
            var ex = TryCatch.RunInTry(() =>
                                  {
                                      //Uri uri = ServiceBusEnvironment.CreateServiceUri("sb", "ingham-blog", string.Empty);
                                      //string name = "owner";
                                      //string key = "abcdefghijklmopqrstuvwxyz";

                                      //TokenProvider tokenProvider = TokenProvider.CreateSharedSecretTokenProvider(name, key);
                                      //NamespaceManager namespaceManager = new NamespaceManager(uri, tokenProvider);

                                      if (PinkoConfiguration.QueueConfiguration[queueName].Item2 && !_azureNamespaceManager.QueueExists(queueName))
                                      {
                                          var td = new QueueDescription(queueName) { MaxSizeInMegabytes = 5120, DefaultMessageTimeToLive = new TimeSpan(0, 0, 30) };
                                          _azureNamespaceManager.CreateQueue(td);
                                      }

                                      if (!PinkoConfiguration.QueueConfiguration[queueName].Item2)
                                      {
                                          var td = new TopicDescription(queueName) { MaxSizeInMegabytes = 5120, DefaultMessageTimeToLive = new TimeSpan(0, 0, 20) };
                                          if (!_azureNamespaceManager.TopicExists(queueName))
                                              _azureNamespaceManager.CreateTopic(td);
                                      }
                                  });
            if (null != ex)
            //    throw ex; // new PinkoExceptionQueueNotConfigured(queueName);
            {
                var newExce = new PinkoExceptionQueueNotConfigured(queueName);
                newExce.Data["OriginalException"] = ex;
                throw newExce;
            }

            // Initialize the connection to Service Bus Queue
            client = PinkoContainer.Resolve<AzureQueueClient>();
            //client.AzureMessageClient = PinkoConfiguration.QueueConfiguration[queueName].Item2
            //                                ? (MessageClientEntity) QueueClient.CreateFromConnectionString(AzureServerConnectionString, queueName)
            //                                : (MessageClientEntity) TopicClient.CreateFromConnectionString(AzureServerConnectionString, queueName);
            client.QueueName = queueName;
            client.AzureNamespaceManager = _azureNamespaceManager;
            client.Initialize(AzureServerConnectionString);
            return client;
        }

        /// <summary>
        /// Get existing or new queue
        /// </summary>
        public IBusMessageQueue GetQueue(string queueName)
        {
            //Trace.WriteLine(string.Format("GetQueue: {0} in {1}...", queueName, AzureServerConnectionString));

            // Get queue
            return _queues.GetOrAdd(queueName, x => ConnectToQueue(queueName));
        }

        /// <summary>
        /// Initialize message bus
        /// </summary>
        public void Initialize()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // Create the queue if it does not exist already
            AzureServerConnectionString = PinkoConfiguration.GetSetting("Microsoft.ServiceBus.ConnectionString");

            //Uri uri = ServiceBusEnvironment.CreateServiceUri("sb", "ingham-blog", string.Empty);
            //string name = "owner";
            //string key = "abcdefghijklmopqrstuvwxyz";

            //TokenProvider tokenProvider = TokenProvider.CreateSharedSecretTokenProvider("pinko-app-bus", "S6c6FYYpdWvOLscjmUyJWQDiQd01gxENzm+W/FSjOk4=");
            //NamespaceManager namespaceManager = new NamespaceManager(uri, tokenProvider);

            Trace.TraceInformation("Creating AzureNamespaceManager: {0}...", AzureServerConnectionString);
            _azureNamespaceManager = NamespaceManager.CreateFromConnectionString(AzureServerConnectionString);

            // Set listener for outbound messages 
            PinkoApplication.GetSubscriber<IBusMessageOutbound>()
                .Do(x => Trace.TraceInformation("AzureBusMessageServer Sending: {0}", x.Verbose()))
                .ObserveOn(Scheduler.ThreadPool)
                .Subscribe(x => GetQueue(string.IsNullOrEmpty(x.ReplyTo) ? x.QueueName : x.ReplyTo).Send(x));
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
        /// Receive all message vis this Rxmemory bus
        /// </summary>
        private IObservable<IBusMessageOutbound> _applicationBusMessageSend;

        /// <summary>
        /// Internal queue cache
        /// </summary>
        readonly ConcurrentDictionary<string, IBusMessageQueue> _queues = new ConcurrentDictionary<string, IBusMessageQueue>();

        /// <summary>
        /// Server connection string
        /// </summary>
        public string AzureServerConnectionString { get; set; }

        /// <summary>
        /// Azure messaging server Namespace
        /// </summary>
        private NamespaceManager _azureNamespaceManager;
    }
}
