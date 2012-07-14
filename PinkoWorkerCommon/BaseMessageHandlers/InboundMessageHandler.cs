using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Microsoft.Practices.Unity;
using PinkoWorkerCommon.Interface;

namespace PinkoWorkerCommon.BaseMessageHandlers
{
    /// <summary>
    /// Handler Ping message response
    /// </summary>
    public abstract class InboundMessageHandler<T>
    {
        /// <summary>
        /// Register Handler
        /// </summary>
        public InboundMessageHandler<T> Register()
        {
            Trace.TraceInformation("Registering message handler: {0}", GetType());

            PinkoApplication
                .GetSubscriber<Tuple<IBusMessageInbound, T>>()
                .Do(x => Trace.TraceInformation("{0}: {1} - {2}", GetType(), x.Item1.Verbose(), x.Item2.ToString()) )
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