using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinkoCommon.Interface;

namespace PinkoCommon.BaseMessageHandlers
{
    /// <summary>
    /// Inbounded typed publisher
    /// </summary>
    public class InboundTypedPublisher<T1, T2> : IInBoundTypedPublisherBase
    {
        /// <summary>
        /// Constructor - InboundTypedPublisher - pushes messages into RxBus
        /// </summary>
        public InboundTypedPublisher(IPinkoApplication pinkoApplication)
        {
            MessageBus = pinkoApplication.GetBus<Tuple<T1, T2>>();
        }

        /// <summary>
        /// Publish message into bus
        /// </summary>
        /// <returns></returns>
        public void Publish(IBusMessageInbound busMessageInbound)
        {
            MessageBus.Publish(new Tuple<T1, T2>((T1)busMessageInbound, (T2)busMessageInbound.Message));
        }       

        /// <summary>
        /// Message bus for this message type
        /// </summary>
        public IRxMemoryBus<Tuple<T1, T2>> MessageBus { get; private set; }
    }
}
