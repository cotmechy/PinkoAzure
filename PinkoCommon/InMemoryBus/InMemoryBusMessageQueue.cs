//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using Microsoft.Practices.Unity;
//using PinkoCommon.BaseMessageHandlers;
//using PinkoCommon.Interface;

//namespace PinkoCommon.InMemoryBus
//{
//    /// <summary>
//    /// In Queue manager. Offline development. Simulate Messaging middleware.
//    /// </summary>
//    public class InMemoryBusMessageQueue : IBusMessageQueue
//    {
//        /// <summary>
//        /// Constructor - BusMessageQueueMock 
//        /// </summary>
//        public InMemoryBusMessageQueue()
//        {
//            QueueName = "QueueName";
//        }

//        /// <summary>
//        /// Initialize
//        /// </summary>
//        public void Initialize(string connectionStr)
//        {
//            Trace.TraceWarning(string.Empty);
//            Trace.TraceWarning(string.Empty);
//            Trace.TraceWarning("**** Using InMemoryBusMessageServer *****");
//            Trace.TraceWarning("**** Using InMemoryBusMessageServer *****");
//            Trace.TraceWarning("**** Using InMemoryBusMessageServer *****");
//            Trace.TraceWarning("**** Using InMemoryBusMessageServer *****");
//            Trace.TraceWarning(string.Empty);
//            Trace.TraceWarning(string.Empty);

//            // Internal memory message bus 
//            _applicationBusMessageResponse = PinkoApplication.GetBus<IBusMessageInbound>();
//            _messageHandlerManager = PinkoContainer.Resolve<IMessageHandlerManager>();
//        }

//        /// <summary>
//        /// Start listening to incoming queues. We are usiing Task space to allowed OS to manage threads
//        /// </summary>
//        public void Listen()
//        {
//            _isRunning = true;

//            Trace.TraceInformation("Starting Listening to {0}", QueueName);

//            // ruun as long as app is running
//            while (_isRunning && !PinkoApplication.ApplicationRunningEvent.WaitOne(0))
//                PinkoApplication.ApplicationRunningEvent.WaitOne(10000);
//        }


//        /// <summary>
//        /// Close
//        /// </summary>
//        public void Close()
//        {
//            _isRunning = false;
//        }

//        /// <summary>
//        /// Send message into queue or topic
//        /// </summary>
//        /// <param name="message"></param>
//        public void Send(IBusMessageOutbound message)
//        {
//            Trace.TraceInformation("(MemBus) Sending: {0} - {1}", QueueName, message.Verbose());
//            // Just cast same message over to inbound since we are in the same process
//            //_applicationBusMessageResponse.Publish((IBusMessageInbound) message);

//            Interlocked.Increment(ref _outboudMessages);
//            _messageHandlerManager.SendToHandler((IBusMessageInbound)message);
//        }

//        /// <summary>
//        /// OutboudMessages 
//        /// </summary>
//        public long OutboudMessages
//        {
//            get { return Interlocked.Read(ref _outboudMessages); }
//        }

//        ///// <summary>
//        ///// Add extra handlers
//        ///// </summary>
//        ///// <returns></returns>
//        //public void AddBusTypeHandler<T>()
//        //{
//        //    _messageHandlerManager.AddBusTypeHandler<T>();
//        //}

//        /// <summary>
//        /// get Rx Subscriber for incoming message type
//        /// </summary>
//        /// <returns></returns>
//        public IObservable<Tuple<IBusMessageInbound, T>> GetIncomingSubscriber<T>()
//        {
//            return _messageHandlerManager.GetSubscriber<T>();
//        }

//        /// <summary>
//        /// get Rx Subscriber for incoming message type
//        /// </summary>
//        /// <returns></returns>
//        public object GetIncominglistener<T>()
//        {
//            return _messageHandlerManager.GetSubscriber<T>();
//        }

//        /// <summary>
//        /// Outbound message count
//        /// </summary>
//        private long _outboudMessages;


//        /// <summary>
//        /// IPinkoApplication
//        /// </summary>
//        [Dependency]
//        public IPinkoApplication PinkoApplication { get; set; }

//        /// <summary>
//        /// IUnityContainer
//        /// </summary>
//        [Dependency]
//        public IUnityContainer PinkoContainer { get; set; }


//        /// <summary>
//        /// MessageHandlerManager 
//        /// </summary>
//        private IMessageHandlerManager _messageHandlerManager;

//        /// <summary>
//        /// Queue Name
//        /// </summary>
//        public string QueueName { get; set; }

//        /// <summary>
//        /// Application bus message
//        /// </summary>
//        private IRxMemoryBus<IBusMessageInbound> _applicationBusMessageResponse;

//        /// <summary>
//        /// Set signal when ready to stop lis
//        /// </summary>
//        private bool _isRunning = false;
//    }
//}
