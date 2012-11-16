using System.Messaging;
using System.Xml.Serialization;
using PinkDao;
using PinkoCommon.Interface;

namespace PinkoMsMqServiceBus
{
    /// <summary>
    /// MsMQ Wrapper 
    /// </summary>
    public class MsMqWrapper<T>
    {

        //[XmlInclude(typeof(PinkoRoleHeartbeat))]
        public T Message { set; get; }

        /// <summary>
        /// Queue Name
        /// </summary>
        public string QueueName { set; get; }

        /// <summary>
        /// ReplyTo
        /// </summary>
        public string ReplyTo { set; get; }

        /// <summary>
        /// Value pairs
        /// </summary>
        public string PinkoProperties { get; set; }

        /// <summary>
        /// Error code. Non 0 is an error
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Error Description. User friendly. 
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Full technical error
        /// </summary>
        public string ErrorSystem { get; set; }
    }


    /// <summary>
    /// MsMqWrapperExtensions
    /// </summary>
    public static class MsMqWrapperExtensions
    {
        /// <summary>
        /// convert outbound message to MsMq message type
        /// </summary>
        static public MsMqWrapper<T> ToMsMqWrapper<T>(this IBusMessageOutbound inbound)
        {
            var wrapper = new MsMqWrapper<T>()
                {
                    Message = (T) inbound.Message,
                    QueueName = inbound.QueueName,
                    ReplyTo = inbound.ReplyTo,
                    PinkoProperties = DictionaryDataItem.Serialize(inbound.PinkoProperties),
                    ErrorCode = inbound.ErrorCode,
                    ErrorDescription = inbound.ErrorDescription,
                    ErrorSystem = inbound.ErrorSystem
                };

            return wrapper;
        }

        /// <summary>
        /// Convert msg from MsMq to Pinko Type
        /// </summary>
        static public MsMqMessageEnvelopInbound FromMsMqWrapper<T>(this MsMqMessageEnvelopInbound inbound, Message msg)
        {

            var msMqWrapper = (MsMqWrapper<T>)msg.Body;  // If exception, you are missing MSMq formatter in _msmqFormatter

            inbound.Message = msMqWrapper.Message;
            inbound.QueueName = msMqWrapper.QueueName;
            inbound.ReplyTo = msMqWrapper.ReplyTo;
            inbound.PinkoProperties = DictionaryDataItem.Deserialize(msMqWrapper.PinkoProperties);
            inbound.ErrorCode = msMqWrapper.ErrorCode;
            inbound.ErrorDescription = msMqWrapper.ErrorDescription;
            inbound.ErrorSystem = msMqWrapper.ErrorSystem;

            return inbound;
        }

    }

}