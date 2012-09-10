﻿using System;
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
using PinkoCommon.Interface;
using PinkoCommon.Utility;

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
            //string queue = _message.DestinationQueue.QueueName;
            //string val = _message.Body.ToString();

            Trace.TraceInformation("Message Label: {0}", msg.Label);
            
            //this.FromMsMqWrapper(msg);

            if (msg.Label == typeof(PinkoRoleHeartbeat).ToString())
                this.FromMsMqWrapper<PinkoRoleHeartbeat>(msg);

            if (msg.Label == typeof(PinkoPingMessage).ToString())
                this.FromMsMqWrapper<PinkoPingMessage>(msg);

            if (msg.Label == typeof(PinkoCalculateExpression).ToString())
                this.FromMsMqWrapper<PinkoCalculateExpression>(msg);

            if (msg.Label == typeof(PinkoCalculateExpressionResult).ToString())
                this.FromMsMqWrapper<PinkoCalculateExpressionResult>(msg);


            //Message = _message.Body;
            //ReplyTo = string.IsNullOrEmpty(_message) ? string.Empty : _message.ReplyTo;

            //msg.Container.Components.ForEach(x => PinkoProperties[x.Key] = x.Value.ToString());

            //var chars = new char[_message.Extension.Length / sizeof(char)];
            //Buffer.BlockCopy(_message.Extension, 0, chars, 0, _message.Extension.Length);
            //DataItem.DeserializeData(new string(chars));

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
