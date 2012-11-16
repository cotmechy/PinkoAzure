using System;
using Microsoft.Practices.Unity;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.Interface;

namespace PinkoMocks
{
    public class BusMessageQueueMock : IBusMessageQueue
    {
        /// <summary>
        /// QueueName
        /// </summary>
        public string QueueName { get; set; }


        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize(string azureServerConnectionString, string selector)
        {
        
        }

        /// <summary>
        /// Start listening to incoming queues. We are using Task space to allowed OS to manage threads
        /// </summary>
        public void Listen()
        {
        }

        /// <summary>
        /// Close
        /// </summary>
        public void Close()
        {
            // Nothing to do
        }

        /// <summary>
        /// Send message into queue or topic
        /// </summary>
        /// <param name="message"></param>
        public void Send(IBusMessageOutbound message)
        {
            OutboudMessages++;
            LastMessageSent = message;
            IncominBusMessageHandlerManager.SendToHandler((IBusMessageInbound) message);
        }

        /// <summary>
        /// OutboudMessages 
        /// </summary>
        public long OutboudMessages { get; private set; }

        /// <summary>
        /// Keep last message sent through this mock
        /// </summary>
        public IBusMessageOutbound LastMessageSent;

        /// <summary>
        /// get Rx Subscriber for incoming message type
        /// </summary>
        /// <returns></returns>
        public IObservable<Tuple<IBusMessageInbound, T>> GetIncomingSubscriber<T>()
        {
            return PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, T>>().MessageBus.Subscriber;
        }


        /// <summary>
        /// IUnityContainer
        /// </summary>
        [Dependency]
        public IUnityContainer PinkoContainer { get; set; }

        [Dependency]
        public IIncominBusMessageHandlerManager IncominBusMessageHandlerManager { get; set; }
    }
}
