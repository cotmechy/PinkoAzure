using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Microsoft.Practices.Unity;
using PinkoCommon.Interface;
using PinkoCommon.Utility;

namespace PinkoCommon.BaseMessageHandlers
{
    /// <summary>
    /// Handler Ping message response
    /// </summary>
    public abstract class InboundMessageHandler<T>
    {
        /// <summary>
        /// Register Handler with RxMesagebus. Subscribe for incoming messages to process in the Rx Bus 
        /// </summary>
        public InboundMessageHandler<T> Register()
        {
            Trace.TraceInformation("Registering message handler: {0}", GetType());

            HandlerPublisher = PinkoApplication.GetBus<Tuple<IBusMessageInbound, T>>();

            PinkoApplication
                .GetSubscriber<Tuple<IBusMessageInbound, T>>()
                .Where(FilterIncomingMsg)
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
            IBusMessageOutbound response = null;

            TryTimer.RunInTimer( () => expression.ToString(),  () => response = ProcessRequest(msg, expression));

            // No response required
            if (response == null) return;

            response.PinkoProperties[PinkoMessagePropTag.MessageHandlerId] = _handlerId;
            response.PinkoProperties[PinkoMessagePropTag.SenderName] = PinkoApplication.MachineName;

            if (string.IsNullOrEmpty(response.ReplyTo))
            {
                Trace.TraceInformation("Missing ReplyTo. Message processed but not sending a response for: {0}", msg.Verbose() );
            }
            else
            {
                response.QueueName = response.ReplyTo;
                response.ReplyTo = string.Empty;
                ReplyQueue.Publish(response);
            }
        }


        /// <summary>
        /// Handler
        /// </summary>
        public abstract IBusMessageOutbound ProcessRequest(IBusMessageInbound msg, T expression);


        /// <summary>
        /// Derived type can override to filter Rx Where().  All message are included by default.
        /// </summary>
        protected virtual bool FilterIncomingMsg(Tuple<IBusMessageInbound, T> msg)
        {
            return true; // include all (default)
        }
        
        /// <summary>
        /// HandlerId
        /// </summary>
        private readonly string _handlerId = Guid.NewGuid().ToString();

        ///// <summary>
        ///// Set handler 
        ///// </summary>
        //public Func<IBusMessageInbound, T, IBusMessageOutbound> ProcessRequestHandler = null;

        /// <summary>
        /// REply queue publisher
        /// </summary>
        protected IRxMemoryBus<IBusMessageOutbound> ReplyQueue;

        /// <summary>
        /// Rx Susbcriber for this handler
        /// </summary>
        public IRxMemoryBus<Tuple<IBusMessageInbound, T>> HandlerPublisher;

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
