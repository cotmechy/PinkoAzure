using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinkoCommon.Interface;

namespace PinkoCommon
{
    /// <summary>
    /// PinkoServiceMessageEnvelop Message
    /// </summary>
    public class PinkoServiceMessageEnvelop : IBusMessageOutbound, IBusMessageInbound
    {
        /// <summary>
        /// Constructor - PinkoServiceMessageEnvelop 
        /// </summary>
        public PinkoServiceMessageEnvelop()
        {
            PinkoProperties = new Dictionary<string, string>();
            ReplyTo = QueueName = string.Empty;

            PinkoProperties[PinkoMessagePropTag.DateTimeStamp] = DateTime.Now.ToUniversalTime().ToLongTimeString();
            PinkoProperties[PinkoMessagePropTag.MachineName] = string.Empty;
            PinkoProperties[PinkoMessagePropTag.SenderName] = string.Empty;

            ErrorCode = PinkoErrorCode.Success;
            ErrorSystem = string.Empty;
            ErrorDescription = string.Empty;
        }

        /// <summary>
        /// Constructor - PinkoServiceMessage 
        /// </summary>
        public PinkoServiceMessageEnvelop(IPinkoApplication application)
            : this()
        {
            PinkoProperties[PinkoMessagePropTag.MachineName] = application.MachineName;
            PinkoProperties[PinkoMessagePropTag.SenderName] = application.UserName;
        }

        /// <summary>
        /// Serialize message 
        /// </summary>
        public object Message
        {
            set
            {
                _message = value;
                _contentType = _message != null ? _message.GetType().ToString() : string.Empty;
            }
            get { return _message; }
        }
        private object _message;


        /// <summary>
        /// Message ContentType
        /// </summary>
        public string ContentType
        {
            private set { _contentType = value; }
            get { return _contentType; }
        }
        private string _contentType = string.Empty;

        /// <summary>
        /// Queue Name
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// ReplyTo
        /// </summary>
        public string ReplyTo { set; get; }

        /// <summary>
        /// Value pairs
        /// </summary>
        public IDictionary<string, string> PinkoProperties { get; set; }


        /// <summary>
        /// Error code. Success.
        /// </summary>
        public int ErrorCode
        {
            get
            {
                return int.Parse(PinkoProperties[PinkoMessagePropTag.ErrorCode]);
            }
            set { PinkoProperties[PinkoMessagePropTag.ErrorCode] = value.ToString(); }
        }

        /// <summary>
        /// Error Description. User friendly. 
        /// </summary>
        public string ErrorDescription
        {
            get
            {
                return (string)PinkoProperties[PinkoMessagePropTag.ErrorDescription];
            }
            set { PinkoProperties[PinkoMessagePropTag.ErrorDescription] = value; }
        }

        /// <summary>
        /// Full technical error
        /// </summary>
        public string ErrorSystem
        {
            get
            {
                return (string) PinkoProperties[PinkoMessagePropTag.ErrorSystem];
            }
            set { PinkoProperties[PinkoMessagePropTag.ErrorSystem] = value; }
        }

        /// <summary>
        /// Verbose debug string
        /// </summary>
        /// <returns></returns>
        string IBusMessageOutbound.Verbose()
        {
            return this.Verbose();
        }

        /// <summary>
        /// Verbose debug string
        /// </summary>
        /// <returns></returns>
        string IBusMessageInbound.Verbose()
        {
            return this.Verbose();
        }
    }

    /// <summary>
    /// PinkoServiceMessageEnvelopExtensions
    /// </summary>
    public static class PinkoServiceMessageEnvelopExtensions
    {
        /// <summary>
        /// PinkoServiceMessageEnvelop
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Verbose(this PinkoServiceMessageEnvelop obj)
        {
            var errorStr = string.Empty;
            if (obj.ErrorCode != 0)
                errorStr = string.Format("ErrorCode: {0} - ErrorDescription: {1} -", obj.ErrorCode, obj.ErrorDescription);

            return string.Format("PinkoServiceMessageEnvelop: QueueName: {0} - Message: {1} - ReplyTo: {2} - {3}ContentType: {4} - Props: {5}"
                                 , obj.QueueName
                                 , obj.Message
                                 , obj.ReplyTo
                                 , errorStr
                                 , obj.ContentType
                                 , string.Join(" | ", obj.PinkoProperties.Select(x => string.Format("{0} = {1}", x.Key, x.Value)))
                );
        }
    }
}
