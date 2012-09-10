using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkoCommon.Interface
{
    public interface IBusMessageOutbound
    {
        /// <summary>
        /// Serializable message 
        /// </summary>
        object Message { set; get; }

        /// <summary>
        /// Queue Name
        /// </summary>
        string QueueName { set; get; }

        /// <summary>
        /// ReplyTo
        /// </summary>
        string ReplyTo { set; get; }

        /// <summary>
        /// Value pairs
        /// </summary>
        IDictionary<string, string> PinkoProperties { get; }

        /// <summary>
        /// Error code. Non 0 is an error
        /// </summary>
        int ErrorCode { get; set; }

        /// <summary>
        /// Error Description. User friendly. 
        /// </summary>
        string ErrorDescription { get; set; }

        /// <summary>
        /// Full technical error
        /// </summary>
        string ErrorSystem { get; set; }

        /// <summary>
        /// Verbose debug string
        /// </summary>
        /// <returns></returns>
        string Verbose();
    }
}
