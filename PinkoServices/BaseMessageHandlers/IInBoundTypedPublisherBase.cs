using PinkoWorkerCommon.Interface;

namespace PinkoServices.BaseMessageHandlers
{
    /// <summary>
    /// Generig
    /// </summary>
    public interface IInBoundTypedPublisherBase
    {
        /// <summary>
        /// Publish message into bus
        /// </summary>
        /// <returns></returns>
        void Publish(IBusMessageInbound message);
    }
}