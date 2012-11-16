using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon.Interface;

namespace PinkoCommon.BaseMessageHandlers
{
    /// <summary>
    /// Manager to handler specific message serialization, mapping, etc
    /// </summary>
    public class IncominBusMessageHandlerManager : IIncominBusMessageHandlerManager
    {
        /// <summary>
        /// Initialize
        /// </summary>
        public IIncominBusMessageHandlerManager Initialize()
        {
            //CachedBusPublisher = GetTypeBuses();

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
            if (CachedBusPublisher.TryGetValue(busMessageInbound.ContentType, out typePublisher))
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
            CachedBusPublisher[typeof(T).ToString()] = PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, T>>();
        }

        /// <summary>
        /// Get Handler
        /// </summary>
        /// <returns></returns>
        public IObservable<Tuple<IBusMessageInbound, T>> GetSubscriber<T>()
        {
            return ((InboundTypedPublisher<IBusMessageInbound, T>) CachedBusPublisher[typeof(T).ToString()]).MessageBus.Subscriber;
        }

        ///// <summary>
        ///// Pre-cache message busses
        ///// </summary>
        ///// <returns></returns>
        //private Dictionary<string, IInBoundTypedPublisherBase> GetTypeBuses()
        //{
        //    //var inboundPublisher = new Dictionary<string, IInBoundTypedPublisherBase>();

        //    CachedBusPublisher[typeof(string).ToString()] = PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, string>>();
        //    CachedBusPublisher[typeof(PinkoMsgPing).ToString()] = PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, PinkoMsgPing>>();
        //    CachedBusPublisher[typeof(PinkoMsgRoleHeartbeat).ToString()] = PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, PinkoMsgRoleHeartbeat>>();
        //    CachedBusPublisher[typeof(PinkoMsgCalculateExpression).ToString()] = PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, PinkoMsgCalculateExpression>>();
        //    CachedBusPublisher[typeof(PinkoMsgCalculateExpressionResult).ToString()] = PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, PinkoMsgCalculateExpressionResult>>();
        //    CachedBusPublisher[typeof(PinkoMsgClientConnect).ToString()] = PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, PinkoMsgClientConnect>>();


        //    return CachedBusPublisher;
        //}

        /// <summary>
        /// Pre-cached typed message busses
        /// </summary>
        public Dictionary<string, IInBoundTypedPublisherBase> CachedBusPublisher = new Dictionary<string, IInBoundTypedPublisherBase>();

        /// <summary>
        /// IUnityContainer
        /// </summary>
        [Dependency]
        public IUnityContainer PinkoContainer { get; set; }
    }
}
