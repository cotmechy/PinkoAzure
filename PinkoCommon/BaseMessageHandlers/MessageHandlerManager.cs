using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon.Interface;

namespace PinkoCommon.BaseMessageHandlers
{
    /// <summary>
    /// Manager to handler specifc message serialization, mappring, etc
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
            _cachedBusPublisher[busMessageInbound.ContentType].Publish(busMessageInbound);
        }


        /// <summary>
        /// Add extra handlers
        /// </summary>
        /// <returns></returns>
        public void AddHandler<T>()
        {
            _cachedBusPublisher[typeof(T).ToString()] = PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, T>>();
        }

        /// <summary>
        /// Get Handler
        /// </summary>
        /// <returns></returns>
        public IObservable<Tuple<IBusMessageInbound, T>> GetSubscriber<T>()
        {
            return (_cachedBusPublisher[typeof(T).ToString()] as InboundTypedPublisher<IBusMessageInbound, T>).MessageBus.Subscriber;
        }

        /// <summary>
        /// Pre-cache msg busses
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, IInBoundTypedPublisherBase> GetTypeBuses()
        {
            var inboundPublisher = new Dictionary<string, IInBoundTypedPublisherBase>();

            inboundPublisher[typeof(string).ToString()] = PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, string>>();
            inboundPublisher[typeof(PinkoPingMessage).ToString()] = PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, PinkoPingMessage>>();
            inboundPublisher[typeof(PinkoRoleHeartbeat).ToString()] = PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, PinkoRoleHeartbeat>>();
            inboundPublisher[typeof(PinkoCalcSubsAction).ToString()] = PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, PinkoCalcSubsAction>>();

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
