using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Microsoft.Practices.Unity;
using PinkoCommon.Interface;

namespace PinkoMocks
{
    public class BusMessageServerMock : IBusMessageServer
    {
        public BusMessageQueueMock BusMessageQueueMock;
        
        /// <summary>
        /// Server connection string
        /// </summary>
        public string AzureServerConnectionString { get; private set; }

        /// <summary>
        /// Connect queue
        /// </summary>
        public IBusMessageQueue GetTopic(string queueName, string selector = "")
        {
            BusMessageQueueMock.QueueName = queueName;
            return BusMessageQueueMock;
        }

        /// <summary>
        /// Initialize message bus
        /// </summary>
        public IBusMessageServer Initialize()
        {
            BusMessageQueueMock = PinkoContainer.Resolve<BusMessageQueueMock>();

            // Set listener for outbound messages. 
            // this server will publish into the message queue
            PinkoApplication.GetSubscriber<IBusMessageOutbound>()
                .Do(x => Trace.TraceInformation("(BusMessageServerMock) Sending: {0}", x.Verbose()))
                .Subscribe(x => GetTopic(x.QueueName).Send(x));

            return this;
        }

        /// <summary>
        /// Deinitialize message bus
        /// </summary>
        public void Deinitialize()
        {
        }


        /// <summary>
        /// PinkoApplication
        /// </summary>
        [Dependency]
        public IPinkoApplication PinkoApplication { get; set; }

        /// <summary>
        /// IUnityContainer
        /// </summary>
        [Dependency]
        public IUnityContainer PinkoContainer { get; set; }


    }
}
