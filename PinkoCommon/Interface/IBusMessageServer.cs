using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkoCommon.Interface
{
    /// <summary>
    /// Message Bus
    /// </summary>
    public interface IBusMessageServer
    {
        /// <summary>
        /// Server connection string
        /// </summary>
        string AzureServerConnectionString { get; }

        /// <summary>
        /// Connect queue
        /// </summary>
        IBusMessageQueue GetTopic(string queueName, string selector = "");

        /// <summary>
        /// Initialize message bus
        /// </summary>
        IBusMessageServer Initialize();

        /// <summary>
        /// Deinitialize message bus
        /// </summary>
        void Deinitialize();

        ///// <summary>
        ///// Queues
        ///// </summary>
        //ConcurrentDictionary<string, InMemoryBusMessageQueue> Queues { get; }
    }

    /// <summary>
    /// Extensions
    /// </summary>
    public static class MessageBusExtensions
    {

        /// <summary>
        /// Verbose()
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Verbose(this IBusMessageServer obj)
        {
            return string.Format("IBusMessageServer: AzureServerConnectionString: {0}", obj.AzureServerConnectionString);
        }
    }
}
