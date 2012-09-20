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
    public class InboundMessageHandler<T>
    {
        /// <summary>
        /// Register Handler with RxMesagebus. Subscribe for incoming messages to process in the Rx Bus 
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
        /// Handle adhoc request
        /// </summary>
        private void HandlerAction(IBusMessageInbound msg, T expression)
        {
            //Trace.TraceInformation("HandlerAction: {0}", expression.);
            var response = ProcessRequestHandler(msg, expression);

            // No response required
            if (response == null) return;

            response.PinkoProperties[PinkoMessagePropTag.MessageHandlerId] = _handlerId;
            response.PinkoProperties[PinkoMessagePropTag.SenderName] = PinkoApplication.MachineName;

            if (string.IsNullOrEmpty(response.ReplyTo))
            {
                Trace.TraceInformation("Missing ReplyTo. Not sending response for: {0}", msg.Verbose() );
            }
            else
            {
                response.QueueName = response.ReplyTo;
                response.ReplyTo = string.Empty;
                ReplyQueue.Publish(response);
            }
        }

        /// <summary>
        /// HandlerId
        /// </summary>
        private readonly string _handlerId = Guid.NewGuid().ToString();

        /// <summary>
        /// Set handler 
        /// </summary>
        public Func<IBusMessageInbound, T, IBusMessageOutbound> ProcessRequestHandler = null;

        /// <summary>
        /// REply queue publisher
        /// </summary>
        protected IRxMemoryBus<IBusMessageOutbound> ReplyQueue;

        /// <summary>
        /// IUnityContainer
        /// </summary>
        [Dependency]
        public IUnityContainer PinkoContainer { get; set; }

        /// <summary>
        /// IPinkoApplication
        /// </summary>
        [Dependency]
        public IPinkoApplication PinkoApplication { get; set; }
    }
}
