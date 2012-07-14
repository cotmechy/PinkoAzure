using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkoWorkerCommon.Interface
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
        IDictionary<string, object> PinkoProperties { get;  }
    }


    /// <summary>
    /// BusMessageOutboundExtensions
    /// </summary>
    public static class BusMessageOutboundExtensions
    {

        /// <summary>
        /// IBusMessageOutbound
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Verbose(this IBusMessageOutbound obj)
        {
            return string.Format("PinkoServiceMessage: IBusMessageOutbound: QueueName: {0} - Message: {1} - ReplyTo: {2} - Props: {3}"
                                                , obj.QueueName
                                                , obj.Message
                                                , obj.ReplyTo
                                                , string.Join(" | ", obj.PinkoProperties.Select(x => string.Format("{0} = {1}", x.Key, x.Value)))
                                                );
        }
    }

}
