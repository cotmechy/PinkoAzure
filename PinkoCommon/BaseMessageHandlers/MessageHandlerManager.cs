using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon.Interface;

namespace PinkoCommon.BaseMessageHandlers
{
    /// <summary>
    /// Manager to handler specific message serialization, mapping, etc
    /// </summary>
    public class MessageHandlerManager : IMessageHandlerManager
    {
        /// <summary>
        /// Initialize
        /// </summary>
        public IMessageHandlerManager Initialize()
        {
            _cachedBusPublisher = GetTypeBuses();

            return this;
        }

        /// <summary>
        /// Publish into proper typed bus. Here we look the proper typed bus.
        /// </summary>
        /// <param name="busMessageInbound"></param>
        public void SendToHandler(IBusMessageInbound busMessageInbound)
        {
            Trace.TraceInformation("(MessageHandlerManager) Receiving: {0}", busMessageInbound.Verbose());

            IInBoundTypedPublisherBase typePublisher = null;
            if (_cachedBusPublisher.TryGetValue(busMessageInbound.ContentType, out typePublisher))
                typePublisher.Publish(busMessageInbound);
            else
                Trace.TraceInformation("Unexpected Message Type.Cannot Process message: {0}", busMessageInbound.Verbose());
        }


        /// <summary>
        /// Add extra handlers
        /// </summary>
        /// <returns></returns>
        public void AddBusTypeHandler<T>()
        {
            _cachedBusPublisher[typeof(T).ToString()] = PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, T>>();
        }

        /// <summary>
        /// Get Handler
        /// </summary>
        /// <returns></returns>
        public IObservable<Tuple<IBusMessageInbound, T>> GetSubscriber<T>()
        {
            return ((InboundTypedPublisher<IBusMessageInbound, T>) _cachedBusPublisher[typeof(T).ToString()]).MessageBus.Subscriber;
        }

        /// <summary>
        /// Pre-cache message busses
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, IInBoundTypedPublisherBase> GetTypeBuses()
        {
            var inboundPublisher = new Dictionary<string, IInBoundTypedPublisherBase>();

            inboundPublisher[typeof(string).ToString()] = PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, string>>();
            inboundPublisher[typeof(PinkoPingMessage).ToString()] = PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, PinkoPingMessage>>();
            inboundPublisher[typeof(PinkoRoleHeartbeat).ToString()] = PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, PinkoRoleHeartbeat>>();
            inboundPublisher[typeof(PinkoCalculateExpression).ToString()] = PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, PinkoCalculateExpression>>();
            inboundPublisher[typeof(PinkoCalculateExpressionResult).ToString()] = PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, PinkoCalculateExpressionResult>>();

            return inboundPublisher;
        }

        /// <summary>
        /// Pre-cached typed message busses
        /// </summary>
        private Dictionary<string, IInBoundTypedPublisherBase> _cachedBusPublisher;

        /// <summary>
        /// IUnityContainer
        /// </summary>
        [Dependency]
        public IUnityContainer PinkoContainer { get; set; }
    }
}
