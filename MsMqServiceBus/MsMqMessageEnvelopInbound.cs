using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PinkDao;
using PinkoCommon;
using PinkoCommon.ExceptionTypes;
using PinkoCommon.Extension;
using PinkoCommon.Extensions;
using PinkoCommon.Interface;
using PinkoCommon.Utility;
using PinkoWorkerCommon.ExceptionTypes;

namespace PinkoMsMqServiceBus
{
    public class MsMqMessageEnvelopInbound : PinkoServiceMessageEnvelop
    {
        /// <summary>
        /// Constructor - AzureBrokeredMessage 
        /// </summary>
        public MsMqMessageEnvelopInbound(IPinkoApplication pinkoApplication, Message msg)
            : base(pinkoApplication)
        {
            _message = msg;

            Trace.TraceInformation("Message Label: {0}", msg.Label);
            MsMqMessageEnvelopInbound inbound = null;

            if (msg.Label == typeof(PinkoMsgRoleHeartbeat).ToString())
                inbound = this.FromMsMqWrapper<PinkoMsgRoleHeartbeat>(msg);

            if (msg.Label == typeof(PinkoMsgPing).ToString())
                inbound =this.FromMsMqWrapper<PinkoMsgPing>(msg);

            if (msg.Label == typeof(PinkoMsgCalculateExpression).ToString())
                inbound = this.FromMsMqWrapper<PinkoMsgCalculateExpression>(msg);

            if (msg.Label == typeof(PinkoMsgCalculateExpressionResult).ToString())
                inbound = this.FromMsMqWrapper<PinkoMsgCalculateExpressionResult>(msg);

            if (msg.Label == typeof(PinkoMsgClientConnect).ToString())
                inbound = this.FromMsMqWrapper<PinkoMsgClientConnect>(msg);

            if (msg.Label == typeof(PinkoMsgClientTimeout).ToString())
                inbound = this.FromMsMqWrapper<PinkoMsgClientTimeout>(msg);

            if (msg.Label == typeof(PinkoMsgClientPing).ToString())
                inbound = this.FromMsMqWrapper<PinkoMsgClientPing>(msg);
            
            if (inbound.IsNull())
                throw new PinkoExceptionMsMqNotFound("Missing MsMq handler in MsMqMessageEnvelopInbound()");
        }

        /// <summary>
        /// Original Azure broker message
        /// </summary>
        private readonly Message _message;
    }


    /// <summary>
    /// DataItem
    /// http://blogs.msdn.com/b/adam/archive/2010/09/10/how-to-serialize-a-dictionary-or-hashtable-in-c.aspx
    /// </summary>
    public class DictionaryDataItem
    {
        public string Key;

        public string Value;

        public DictionaryDataItem()
        {
            
        }

        public DictionaryDataItem(string key, string value)
        {
            Key = key;
            Value = value;
        }

        static public string Serialize(IDictionary<string, string> dict)
        {
            var tempdataitems = new List<DictionaryDataItem>(dict.Count);

            tempdataitems.AddRange(dict.Keys.Select(key => new DictionaryDataItem(key, dict[key].ToString())));

            var serializer = new XmlSerializer(typeof(List<DictionaryDataItem>));
            var sw = new StringWriter();
            var ns = new XmlSerializerNamespaces();

            serializer.Serialize(sw, tempdataitems, ns);

            return sw.ToString();
        }

        static public IDictionary<string, string> Deserialize(string rawData)
        {
            var xs = new XmlSerializer(typeof(List<DictionaryDataItem>));
            var sr = new StringReader(rawData);

            var templist = (List<DictionaryDataItem>)xs.Deserialize(sr);

            return templist.ToDictionary<DictionaryDataItem, string, string>(di => di.Key, di => di.Value);
        }
    }
}
