using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinkoCommon.Interface;
using PinkoWorkerCommon.Interface;

namespace PinkoWorkerCommon.Utility
{
    /// <summary>
    /// PinkoServiceMessageEnvelop Message
    /// </summary>
    public class PinkoServiceMessageEnvelop : IBusMessageOutbound, IBusMessageInbound
    {
        /// <summary>
        /// Constructor - PinkoServiceMessage 
        /// </summary>
        public PinkoServiceMessageEnvelop()
        {
            PinkoProperties = new Dictionary<string, object>(); 
            ReplyTo = QueueName = string.Empty;
        }

        /// <summary>
        /// Create PinkoServiceMessageEnvelop for sending out from WorkerRole to Clients
        /// </summary>
        /// <returns></returns>
        static public PinkoServiceMessageEnvelop FactorClientMessage(string queueName, object message)
        {
            return new PinkoServiceMessageEnvelop()
                       {
                           QueueName = queueName,
                           ReplyTo = queueName,
                           ContentType = message.GetType().ToString(),
                           Message = message
                       };
        }

        /// <summary>
        /// Serializable message 
        /// </summary>
        public object Message { set; get; }

        /// <summary>
        /// Message ContentType
        /// </summary>
        public string ContentType { set; get; }

        /// <summary>
        /// ClientId
        /// </summary>
        public string ClientId { set; get; }

        /// <summary>
        /// Queue Name
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// ReplyTo
        /// </summary>
        public string ReplyTo { set; get; }

        /// <summary>
        /// Value pairs
        /// </summary>
        public IDictionary<string, object> PinkoProperties { get; set; }
    }
}
