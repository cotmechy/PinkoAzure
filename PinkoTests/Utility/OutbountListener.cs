using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using PinkoCommon.Interface;

namespace PinkoTests.Utility
{
    /// <summary>
    /// Unit test class contaner to intercep outbound messages for testing
    /// </summary>
    internal class OutbountListener
    {
        /// <summary>
        /// Constructor - OutbountListener 
        /// </summary>
        public OutbountListener(IUnityContainer container)
        {
            OutboutBus = container.Resolve<IRxMemoryBus<IBusMessageOutbound>>();

            OutboutBus.Subscriber.Subscribe(x => OutboundMessages.Add(x));
        }

        public ConcurrentBag<IBusMessageOutbound> OutboundMessages = new ConcurrentBag<IBusMessageOutbound>();  

        private readonly IRxMemoryBus<IBusMessageOutbound> OutboutBus = null;

    }
}
