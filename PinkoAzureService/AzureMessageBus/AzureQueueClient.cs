using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using PinkDao;
using PinkoWorkerCommon.BaseMessageHandlers;
using PinkoWorkerCommon.ExceptionTypes;
using PinkoWorkerCommon.Interface;
using PinkoWorkerCommon.Utility;

namespace PinkoAzureService.AzureMessageBus
{
    /// <summary>
    /// AzureQueueClient
    /// </summary>
    public class AzureQueueClient : IBusMessageQueue
    {
        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize(string azureServerConnectionString)
        {
            //AzureMessageClient = PinkoConfiguration.QueueConfiguration[QueueName].Item2
            //                                ? (MessageClientEntity)QueueClient.CreateFromConnectionString(azureServerConnectionString, QueueName)
            //                                : (MessageClientEntity)TopicClient.CreateFromConnectionString(azureServerConnectionString, QueueName);
            //IsTopic = AzureMessageClient is TopicClient;

            // Internal memory message bus 
            _messageHandlerManager = PinkoContainer.Resolve<MessageHandlerManager>().Initialize();

            // Azure msg factory
            var tokenProvider = TokenProvider.CreateSharedSecretTokenProvider(PinkoConfiguration.GetSetting("Issuer"), PinkoConfiguration.GetSetting("SecretKey"));

            _messageFactory = MessagingFactory.Create(AzureNamespaceManager.Address, tokenProvider);

            _msgSender = _messageFactory.CreateMessageSender(QueueName);


            // http://msdn.microsoft.com/en-us/library/windowsazure/hh699844.aspx

            // http://msdn.microsoft.com/en-us/library/windowsazure/microsoft.servicebus.messaging.messagingfactory.aspx

            //_azureNamespaceManager.CreateSubscription(queueName,"PinkoTopic");


            //_azureNamespaceManager.crea

            //MessageReceiver receiver = .CreateMessageReceiver("DataCollectionTopic/subscriptions/Inventory");
            //BrokeredMessage receivedMessage = receiver.Receive();
            //try
            //{
            //    ProcessMessage(receivedMessage);
            //    receivedMessage.Complete();
            //}
            //catch (Exception e)
            //{
            //    receivedMessage.Abandon();
            //}

            //Similar to using queues, the simplest way to receive messages from a subscription is to use a MessageReceiver object which you can create directly from the MessagingFactory using CreateMessageReceiver. You can use one of the two different receive modes (ReceiveAndDelete and PeekLock), as discussed in Creating Applications that Use Service Bus Queues.

            //Note that when you create a MessageReceiver for subscriptions, the entityPath parameter is of the form topicPath/subscriptions/subscriptionName. Therefore, to create a MessageReceiver for the Inventory subscription of the DataCollectionTopic topic, entityPath must be DataCollectionTopic/subscriptions/Inventory. The code appears as follows:

        }

        /// <summary>
        /// Start listening to incoming queues. We are usiing Task space to allowed OS to manage threads
        /// </summary>
        public void Listen()
        {
            var msgReceiver = _messageFactory.CreateMessageReceiver(QueueName);
            _isRunning = true;

            Trace.TraceInformation("Starting Listening to {0}", QueueName);

            // ruun as long as app is running
            while (_isRunning && !PinkoApplication.ApplicationRunningEvent.WaitOne(0))
            {
                Exception ex = TryCatch.RunInTry(() =>  
                {
                    // Receive the message
                    BrokeredMessage receivedMessage = null;

                    // TODO: how to listen to topics
                    //receivedMessage = IsTopic ? null : _queueClient.Receive();
                    receivedMessage = msgReceiver.Receive();

                    Trace.TraceInformation("Checking Azure Message Queue: {0}", QueueName);

                    if (receivedMessage != null)
                    {
                        // Process the message
                        Trace.TraceInformation("Recevied: {0}: {1}", QueueName, receivedMessage.Verbose());

                        // Send to listeners
                        _messageHandlerManager.SendToHandler(new AzureBrokeredMessageEnvelopInbound(receivedMessage)
                                                                   {
                                                                       QueueName = QueueName
                                                                   });

                        receivedMessage.Complete();
                    }
                });

                if (ex is MessagingException)
                {
                    if (!((MessagingException)ex).IsTransient)
                    {
                        Trace.WriteLine(ex.Message);
                        throw ex;
                    }

                    PinkoApplication.ApplicationRunningEvent.WaitOne(PinkoConfiguration.MessageQueueCheckIntervalMs);
                }


                if (ex is OperationCanceledException)
                {
                    if (!_isRunning)
                    {
                        Trace.WriteLine(ex.Message);
                        throw ex;
                    }
                }
            }

            msgReceiver.Close();
        }

        /// <summary>
        /// Close
        /// </summary>
        public void Close()
        {
            _isRunning = false;
            _msgSender.Close();
        }


        /// <summary>
        /// Send message into queue or topic
        /// </summary>
        /// <param name="message"></param>
        public void Send(IBusMessageOutbound message)
        {
            using (var bm = FactorNewOutboundMessage(message))
            {
                // MUST ADD Deserializer type for new type type in AzureQueueClientExtensions::GetDeserializer()
                //Debug.Assert(AzureQueueClientExtensions.TypeDeserializerdict.ContainsKey(bm.ContentType));
                if (!AzureQueueClientExtensions.TypeDeserializerdict.ContainsKey(bm.ContentType))
                    throw new PinkoExceptionAzureDeserializerNotFound(bm.ContentType);

                // move properties into the undelying message
                message.PinkoProperties.ForEach(x => bm.Properties[x.Key] = x.Value);

                // Keep count
                Interlocked.Read(ref _outboudMessages);

                // Send
                _msgSender.Send(bm);
            }
        }

        /// <summary>
        /// Stop listening to queu message
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
        }

        /// <summary>
        /// Get new outbaound message
        /// </summary>
        /// <returns></returns>
        static public BrokeredMessage FactorNewOutboundMessage(IBusMessageOutbound msg)
        {
            return
                new BrokeredMessage(msg.Message)
                {
                    ContentType = msg.Message.GetType().ToString()
                };
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
        /// IPinkoApplication
        /// </summary>
        [Dependency]
        public IPinkoApplication PinkoApplication { get; set; }

        /// <summary>
        /// IUnityContainer
        /// </summary>
        [Dependency]
        public IUnityContainer PinkoContainer { get; set; }

        /// <summary>
        /// IPinkoConfiguration
        /// </summary>
        [Dependency]
        public IPinkoConfiguration PinkoConfiguration { private get; set; }

        /// <summary>
        /// MessageHandlerManager 
        /// </summary>
        private MessageHandlerManager _messageHandlerManager;


        ///// <summary>
        ///// QueueClient is thread-safe. Recommended that you cache 
        ///// </summary>
        //private MessageClientEntity AzureMessageClient
        //{
        //    get { return _azureMessageClient; }
        //    set 
        //    { 
        //        _azureMessageClient = value;
        //        _topicClient = _azureMessageClient as TopicClient;
        //        _queueClient = _azureMessageClient as QueueClient;
        //    }
        //}
        //private MessageClientEntity _azureMessageClient;

        ///// <summary>
        ///// Pre-Cast 
        ///// </summary>
        //private QueueClient _queueClient;
        //private TopicClient _topicClient;

        /// <summary>
        /// Queue Name
        /// </summary>
        public string QueueName { get; set; }

        ///// <summary>
        ///// Queue Name
        ///// </summary>
        //public bool IsTopic { get; set; }

        /// <summary>
        /// Set signal when ready to stop lis
        /// </summary>
        private bool _isRunning = false;

        /// <summary>
        /// AzureNamespaceManager
        /// </summary>
        public NamespaceManager AzureNamespaceManager;

        /// <summary>
        /// MessagingFactory
        /// </summary>
        private MessagingFactory _messageFactory;

        /// <summary>
        /// MessageSender
        /// </summary>
        private MessageSender _msgSender;

        ///// <summary>
        ///// Message receiver
        ///// </summary>
        //private MessageReceiver _msgReceiver;
    }



    /// <summary>
    /// Extensions
    /// </summary>
    public static class AzureQueueClientExtensions
    {
        /// <summary>
        /// Extension to get pre-define deserialziation types
        /// </summary>
        public static object GetBody(this BrokeredMessage obj)
        {
            Func<BrokeredMessage, object> deserializeAction;
            TypeDeserializerdict.TryGetValue(obj.ContentType, out deserializeAction);

            return deserializeAction == null ? null : deserializeAction(obj);
        }


        /// <summary>
        /// Get deserializers
        /// </summary>
        /// <returns></returns>
        static Dictionary<string, Func<BrokeredMessage, object>> GetDeserializer()
        {
            var typeDeser = new Dictionary<string, Func<BrokeredMessage, object>>();

            typeDeser[typeof(string).ToString()] = x => x.GetBody<string>();
            typeDeser[typeof(PinkoPingMessage).ToString()] = x => x.GetBody<PinkoPingMessage>();
            typeDeser[typeof(PinkoRoleHeartbeat).ToString()] = x => x.GetBody<PinkoRoleHeartbeat>();

            return typeDeser;
        }


        /// <summary>
        /// AzureQueueClient
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Verbose(this BrokeredMessage obj)
        {
            return string.Format("BrokeredMessage: ContentType: {0} - CorrelationId: {1} - DeliveryCount: {2} - ExpiresAtUtc: {3} - Label: {4} - MessageId: {5} - ReplyTo: {6} - Size: {7} - TimeToLive: {8} - SequenceNumber: {9} - SessionId: {10} - Size: {11} - TimeToLive: {12} - To: {13}"
                                                , obj.ContentType
                                                , obj.CorrelationId
                                                , obj.DeliveryCount
                                                , obj.ExpiresAtUtc
                                                , obj.Label
                                                , obj.MessageId
                                                , obj.ReplyTo
                                                , obj.Size
                                                , obj.TimeToLive
                                                , obj.SequenceNumber
                                                , obj.SessionId
                                                , obj.Size
                                                , obj.TimeToLive
                                                , obj.To
                                                );
        }


        
        /// <summary>
        /// Static deserializers list
        /// </summary>
        static public Dictionary<string, Func<BrokeredMessage, object>> TypeDeserializerdict = GetDeserializer();
    }

}
