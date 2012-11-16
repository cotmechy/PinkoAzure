using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using PinkoCommon.Interface;

namespace PinkoMocks
{
    /// <summary>
    /// Unit test class contaner to intercep outbound messages for testing
    /// </summary>
    public class OutbountListener<T>
    {
        /// <summary>
        /// Constructor - OutbountListener 
        /// </summary>
        public OutbountListener(IUnityContainer container)
        {
            OutboutBus = container.Resolve<IRxMemoryBus<T>>();

            OutboutBus.Subscriber.Subscribe(x => OutboundMessages.Add(x));
        }

        public void Clear()
        {
            OutboundMessages = new ConcurrentBag<T>();
        }


        public ConcurrentBag<T> OutboundMessages = new ConcurrentBag<T>();

        private readonly IRxMemoryBus<T> OutboutBus;

    }
}
