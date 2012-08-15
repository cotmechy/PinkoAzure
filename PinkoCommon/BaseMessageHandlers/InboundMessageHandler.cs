using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using PinkoCommon.Interface;

namespace PinkoCommon.BaseMessageHandlers
{
    /// <summary>
    /// Handler Ping message response
    /// </summary>
    public abstract class InboundMessageHandler<T>
    {
        /// <summary>
        /// Register Handler with RxMesagebus. 
        /// </summary>
        public InboundMessageHandler<T> Register()
        {
            Trace.TraceInformation("Registering message handler: {0}", GetType());

            PinkoApplication
                .GetSubscriber<Tuple<IBusMessageInbound, T>>()
                .Do(x => Trace.TraceInformation("{0}: {1} - {2}", GetType(), x.Item1.Verbose(), x.Item2.ToString()))
                .Subscribe(x => HandlerAction(x.Item1, x.Item2));

            ReplyQueue = PinkoApplication.GetBus<IBusMessageOutbound>();

            return this;
        }

        /// <summary>
        /// Set handler 
        /// </summary>
        public abstract void HandlerAction(IBusMessageInbound msg, T type);

        /// <summary>
        /// REply queue publisher
        /// </summary>
        public IRxMemoryBus<IBusMessageOutbound> ReplyQueue;

        /// <summary>
        /// IPinkoApplication
        /// </summary>
        [Dependency]
        public IPinkoApplication PinkoApplication { get; set; }
    }
}
