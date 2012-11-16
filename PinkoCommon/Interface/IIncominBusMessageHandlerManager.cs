using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkoCommon.Interface
{
    /// <summary>
    /// Message Handlers - All messages from message bus are push into RxExtension
    /// </summary>
    public interface IIncominBusMessageHandlerManager
    {
        /// <summary>
        /// Initialize
        /// </summary>
        IIncominBusMessageHandlerManager Initialize();

        /// <summary>
        /// Publish into proper typed bus. Here we look the proper typed bus.
        /// </summary>
        /// <param name="busMessageInbound"></param>
        void SendToHandler(IBusMessageInbound busMessageInbound);

        /// <summary>
        /// Add message type so they can be routed to RxBus
        /// </summary>
        /// <returns></returns>
        void AddBusTypeHandler<T>();

        /// <summary>
        /// Get Handler
        /// </summary>
        /// <returns></returns>
        IObservable<Tuple<IBusMessageInbound, T>> GetSubscriber<T>();
    }
}
