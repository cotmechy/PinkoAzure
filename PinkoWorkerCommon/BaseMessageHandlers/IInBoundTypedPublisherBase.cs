using PinkoWorkerCommon.Interface;

namespace PinkoWorkerCommon.BaseMessageHandlers
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