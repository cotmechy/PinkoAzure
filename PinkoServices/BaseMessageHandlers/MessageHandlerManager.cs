using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoServices.AzureMessageBus;
using PinkoWorkerCommon.Interface;

namespace PinkoServices.BaseMessageHandlers
{
    /// <summary>
    /// Manager to handler specifc message serialization, mappring, etc
    /// </summary>
    public class MessageHandlerManager
    {
        /// <summary>
        /// Initialize
        /// </summary>
        public MessageHandlerManager Initialize()
        {
            _cachedBusPublisher = GetTypeBuses();

            return this;
        }

        /// <summary>
        /// Publish into proper typed bus. Here we look the proper typed bus.
        /// </summary>
        /// <param name="busMessageInbound"></param>
        public void Publish(IBusMessageInbound busMessageInbound)
        {
            _cachedBusPublisher[busMessageInbound.ContentType].Publish(busMessageInbound);
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
