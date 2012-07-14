using System;
using PinkoWorkerCommon.Interface;

namespace PinkoWorkerCommon.BaseMessageHandlers
{
    /// <summary>
    /// Inbouned typed publisher
    /// </summary>
    class InboundTypedPublisher<T1, T2> : IInBoundTypedPublisherBase
    {
        /// <summary>
        /// Constructor - InboundTypedPublisher 
        /// </summary>
        public InboundTypedPublisher(IPinkoApplication pinkoApplication)
        {
            MessageBus = pinkoApplication.GetBus<Tuple<T1, T2>>();
        }

        public IRxMemoryBus<Tuple<T1, T2>> MessageBus;

        /// <summary>
        /// Publish message into bus
        /// </summary>
        /// <returns></returns>
        public void Publish(IBusMessageInbound busMessageInbound)
        {
            MessageBus.Publish(new Tuple<T1, T2>((T1) busMessageInbound, (T2) busMessageInbound.Message));
        }
    }
}