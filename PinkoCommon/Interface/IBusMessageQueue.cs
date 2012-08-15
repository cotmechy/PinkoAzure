using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkoCommon.Interface
{
    /// <summary>
    /// single Queue
    /// </summary>
    public interface IBusMessageQueue
    {
        /// <summary>
        /// QueueName
        /// </summary>
        string QueueName { get; }

        /// <summary>
        /// Initialize
        /// </summary>
        void Initialize(string azureServerConnectionString);

        /// <summary>
        /// Start listening to incoming queues. We are usiing Task space to allowed OS to manage threads
        /// </summary>
        void Listen();

        /// <summary>
        /// Close
        /// </summary>
        void Close();

        /// <summary>
        /// Send message into queue or topic
        /// </summary>
        /// <param name="message"></param>
        void Send(IBusMessageOutbound message);

        /// <summary>
        /// OutboudMessages 
        /// </summary>
        long OutboudMessages { get; }

        /// <summary>
        /// Add extra handlers
        /// </summary>
        /// <returns></returns>
        void AddHandler<T>();

        /// <summary>
        /// get Rx Subscriber for incoming message type
        /// </summary>
        /// <returns></returns>
        IObservable<Tuple<IBusMessageInbound, T>> GetIncomingSubscriber<T>();
    }

    /// <summary>
    /// Extensions
    /// </summary>
    public static class MessageQueueExtensions
    {
        /// <summary>
        /// IIMessageQueue
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Verbose(this IBusMessageQueue obj)
        {
            return string.Format("IBusMessageQueue: QueueName: {0}", obj.QueueName);
        }
    }

}
