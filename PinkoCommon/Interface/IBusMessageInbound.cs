using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkoCommon.Interface
{
    /// <summary>
    /// Bus message
    /// </summary>
    public interface IBusMessageInbound
    {
        /// <summary>
        /// Serializable message 
        /// </summary>
        object Message { get; set; }

        /// <summary>
        /// Queue Name
        /// </summary>
        string QueueName { get; }

        /// <summary>
        /// ReplyTo
        /// </summary>
        string ReplyTo { get; }

        /// <summary>
        /// Message ContentType
        /// </summary>
        string ContentType { get; }

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


    ///// <summary>
    ///// BusMessageInboundExtensions
    ///// </summary>
    //public static class BusMessageInboundExtensions
    //{
    //    /// <summary>
    //    /// IBusMessageInbound
    //    /// </summary>
    //    /// <param name="obj"></param>
    //    /// <returns></returns>
    //    public static string Verbose(this IBusMessageInbound obj)
    //    {
    //        return string.Format("PinkoServiceMessage: IBusMessageInbound: QueueName: {0} - Message: {1} - ReplyTo: {2} - Props: {3}"
    //                             , obj.QueueName
    //                             , obj.Message
    //                             , obj.ReplyTo
    //                             , string.Join(" | ", obj.PinkoProperties.Select(x => string.Format("{0} = {1}", x.Key, x.Value)))
    //            );
    //    }
    //}
}
