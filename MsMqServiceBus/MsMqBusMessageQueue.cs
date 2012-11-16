﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Messaging;
using System.Threading;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon.ExceptionTypes;
using PinkoCommon.Extension;
using PinkoCommon.Extensions;
using PinkoCommon.Interface;

namespace PinkoMsMqServiceBus
{

    /// <summary>
    /// MsMqBusMessageQueue
    /// </summary>
    public class MsMqBusMessageQueue : IBusMessageQueue
    {
        /// <summary>
        /// QueueName
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize(string azureServerConnectionString, string selector)
        {
            Trace.TraceWarning(string.Empty);
            Trace.TraceWarning(string.Empty);
            Trace.TraceWarning("**** Using MsMqBusMessageQueue *****");
            Trace.TraceWarning("**** Using MsMqBusMessageQueue *****");
            Trace.TraceWarning("**** Using MsMqBusMessageQueue *****");
            Trace.TraceWarning("**** Using MsMqBusMessageQueue *****");
            Trace.TraceWarning(string.Empty);
            Trace.TraceWarning(string.Empty);

            _msmqPrivateName = string.Format(@".\private$\{0}", QueueName);
            Trace.TraceInformation("Connecting to private msmq queue: {0}", _msmqPrivateName);

            if (!MessageQueue.Exists(_msmqPrivateName))
                MessageQueue.Create(_msmqPrivateName);

            _msgQueue = new MessageQueue(_msmqPrivateName, false, false)
                {
                    Formatter = MsmqFormatter
                    //MessageReadPropertyFilter = new MessagePropertyFilter()
                };

            // set message property
            //_msgQueue.MessageReadPropertyFilter.SetAll();

            MessageQueue.GetPrivateQueuesByMachine(".").ToList().ForEach(x => Trace.TraceInformation("Current MsMq Queues: {0}", x.QueueName));
        }

        /// <summary>
        /// Start listening to incoming queues. We are using Task space to allowed OS to manage threads
        /// </summary>
        public void Listen()
        {
            Trace.TraceInformation("Starting Listening to {0}", QueueName);

            _msgQueueReader = new MessageQueue(_msmqPrivateName, false, false)
            {
                Formatter = MsmqFormatter
            };

            Trace.TraceInformation("Starting Listening to private: {0}", _msmqPrivateName);

            //var waitForMsg = new TimeSpan(0, PinkoConfiguration.MessageQueueCheckIntervalMs, 0);

            // run as long as app is running
            while (!PinkoApplication.ApplicationRunningEvent.WaitOne(0))
            {
                // start a new queue and listen for a message
                //MessageQueue.Create(testQueue); //".\\PinkoUbnistTestTopic");

                //var queues = MessageQueue.GetPrivateQueuesByMachine(".");
                //queues.ToList().ForEach(x => Debug.WriteLine(">>" + x.QueueName));

                // http://social.msdn.microsoft.com/Forums/en/csharplanguage/thread/c60d5605-0507-47b6-92d4-8f6b904dd978
                //System.Messaging.Message messageReceived = new System.Messaging.Message();
                //messageReceived = mq.Receive();
                //string body = mq.Formatter.Read(messageReceived) as string;

                var msg = _msgQueueReader.Receive();

                Trace.TraceInformation("MsMq Received:Queue: {0}", QueueName);

                // Send to listeners via Rx bus
                IncominBusMessageHandlerManager.SendToHandler(new MsMqMessageEnvelopInbound(PinkoApplication, msg)
                {
                    QueueName = QueueName
                });
            }
        }

        /// <summary>
        /// Close
        /// </summary>
        public void Close()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send message into queue or topic
        /// </summary>
        /// <param name="message"></param>
        public void Send(IBusMessageOutbound message)
        {
            using (var msmqMsg = FactorNewOutboundMessage(message))
            {
                //Debug.WriteLine("------------------------");
                //MessageQueue.GetPrivateQueuesByMachine(".").ToList().ForEach(x => Debug.WriteLine(">>" + x.QueueName));
                //Debug.WriteLine("------------------------");

                //// MUST ADD Deserializer type for new type type in AzureQueueClientExtensions::GetDeserializer()
                ////Debug.Assert(AzureQueueClientExtensions.TypeDeserializerdict.ContainsKey(bm.ContentType));
                //if (!AzureQueueClientExtensions.TypeDeserializerdict.ContainsKey(bm.ContentType))
                //    throw new PinkoExceptionAzureDeserializerNotFound(bm.ContentType);

                //// move properties into the underlying message
                //message.PinkoProperties.ForEach(x => bm.Properties[x.Key] = x.Value);

                // Keep count
                Interlocked.Increment(ref _outboudMessages);

                //// Send
                _msgQueue.Send(msmqMsg, MessageQueueTransactionType.None);
            }
        }

        /// <summary>
        /// Get new outbound message and convert it into MsMQ message type
        /// </summary>
        /// <returns></returns>
        public Message FactorNewOutboundMessage(IBusMessageOutbound msg)
        {
            var msmqMsg = new Message {Formatter = MsmqFormatter};

            if (msg.Message.GetType() == typeof(PinkoMsgRoleHeartbeat))
            {
                msmqMsg.Body = msg.ToMsMqWrapper<PinkoMsgRoleHeartbeat>();
                msmqMsg.Label = msg.Message.GetType().ToString();
            }
            if (msg.Message.GetType() == typeof(PinkoMsgPing))
            {
                msmqMsg.Body = msg.ToMsMqWrapper<PinkoMsgPing>();
                msmqMsg.Label = msg.Message.GetType().ToString();
            }
            if (msg.Message.GetType() == typeof(PinkoMsgCalculateExpression))
            {
                msmqMsg.Body = msg.ToMsMqWrapper<PinkoMsgCalculateExpression>();
                msmqMsg.Label = msg.Message.GetType().ToString();
            }
            if (msg.Message.GetType() == typeof(PinkoMsgCalculateExpressionResult))
            {
                msmqMsg.Body = msg.ToMsMqWrapper<PinkoMsgCalculateExpressionResult>();
                msmqMsg.Label = msg.Message.GetType().ToString();
            }
            if (msg.Message.GetType() == typeof(PinkoMsgClientConnect))
            {
                msmqMsg.Body = msg.ToMsMqWrapper<PinkoMsgClientConnect>();
                msmqMsg.Label = msg.Message.GetType().ToString();
            }
            if (msg.Message.GetType() == typeof(PinkoMsgClientTimeout))
            {
                msmqMsg.Body = msg.ToMsMqWrapper<PinkoMsgClientTimeout>();
                msmqMsg.Label = msg.Message.GetType().ToString();
            }
            if (msg.Message.GetType() == typeof(PinkoMsgClientPing))
            {
                msmqMsg.Body = msg.ToMsMqWrapper<PinkoMsgClientPing>();
                msmqMsg.Label = msg.Message.GetType().ToString();
            }


            if (string.IsNullOrEmpty(msmqMsg.Label))
                throw new PinkoExceptionMsMqNotFound("Missing MsMq handler in FactorNewOutboundMessage()");

            // set message property
            //_msgQueue.MessageReadPropertyFilter.SetAll();

            //var enc = new ASCIIEncoding();
            //msmqMsg.Extension = enc.GetBytes(DictionaryDataItem.Serialize(msg.PinkoProperties));

            // move properties into the underlying message
            //msmqMsg.ResponseQueue = msg.ReplyTo;
            //msmqMsg.BodyType = 
            //msg.PinkoProperties.ForEach(x => msmqMsg.Container.Add(x));
            //msmqMsg.Formatter = _msgQueue.Formatter;

            return msmqMsg;
        }

        /// <summary>
        /// OutboudMessages 
        /// </summary>
        public long OutboudMessages
        {
            get { return Interlocked.Read(ref _outboudMessages); }
        }

        private long _outboudMessages;

        /// <summary>
        /// get Rx Subscriber for incoming message type
        /// </summary>
        /// <returns></returns>
        public IObservable<Tuple<IBusMessageInbound, T>> GetIncomingSubscriber<T>()
        {
            return IncominBusMessageHandlerManager.GetSubscriber<T>();
        }

        /// <summary>
        /// MsMQ queue
        /// </summary>
        private MessageQueue _msgQueue;
        private MessageQueue _msgQueueReader;
        private string _msmqPrivateName;

        public readonly XmlMessageFormatter MsmqFormatter = new XmlMessageFormatter(new Type[]
            {
                typeof (MsMqWrapper<PinkoMsgRoleHeartbeat>),
                typeof (MsMqWrapper<PinkoMsgPing>),
                typeof (MsMqWrapper<PinkoMsgCalculateExpression>),
                typeof (MsMqWrapper<PinkoMsgCalculateExpressionResult>),
                typeof (MsMqWrapper<PinkoMsgClientConnect>),
                typeof (MsMqWrapper<PinkoMsgClientTimeout>),
                typeof (PinkoDataFeedIdentifier),
                typeof (PinkoFormPoint),
                typeof (PinkoMsgRoleHeartbeat),
                typeof (PinkoMsgPing),
                typeof (PinkoMsgCalculateExpression),
                typeof (PinkoMsgCalculateExpressionResult),
                typeof (PinkoMsgClientConnect),
                typeof (PinkoMsgClientTimeout),
                typeof (PinkoMsgClientPing),
                typeof (MsMqWrapper<PinkoMsgClientPing>),
            });

        /// <summary>
        /// PinkoApplication
        /// </summary>
        [Dependency]
        public IPinkoApplication PinkoApplication { get; set; }

        /// <summary>
        /// IPinkoConfiguration
        /// </summary>
        [Dependency]
        public IPinkoConfiguration PinkoConfiguration { private get; set; }


        /// <summary>
        /// IMessageHandlerManager
        /// </summary>
        [Dependency]
        public IIncominBusMessageHandlerManager IncominBusMessageHandlerManager { get; set; }

        /// <summary>
        /// IUnityContainer
        /// </summary>
        [Dependency]
        public IUnityContainer PinkoContainer { get; set; }
    }
}
