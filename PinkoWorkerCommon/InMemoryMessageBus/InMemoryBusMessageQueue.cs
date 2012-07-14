using System.Diagnostics;
using System.Threading;
using Microsoft.Practices.Unity;
using PinkoWorkerCommon.BaseMessageHandlers;
using PinkoWorkerCommon.Interface;

namespace PinkoWorkerCommon.InMemoryMessageBus
{
    /// <summary>
    /// In Queue manager. Offline develpment. Simulate Messageing middleware.
    /// </summary>
    public class InMemoryBusMessageQueue : IBusMessageQueue
    {
        /// <summary>
        /// Constructor - BusMessageQueueMock 
        /// </summary>
        public InMemoryBusMessageQueue()
        {
            QueueName = "QueueName";
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize(string connectionStr)
        {
            Trace.TraceWarning(string.Empty);
            Trace.TraceWarning(string.Empty);
            Trace.TraceWarning("**** Using InMemoryBusMessageServer *****");
            Trace.TraceWarning("**** Using InMemoryBusMessageServer *****");
            Trace.TraceWarning("**** Using InMemoryBusMessageServer *****");
            Trace.TraceWarning("**** Using InMemoryBusMessageServer *****");
            Trace.TraceWarning(string.Empty);
            Trace.TraceWarning(string.Empty);

            // Internal memory message bus 
            _applicationBusMessageResponse = PinkoApplication.GetBus<IBusMessageInbound>();
            _messageHandlerManager = PinkoContainer.Resolve<MessageHandlerManager>().Initialize();
        }

        /// <summary>
        /// Start listening to incoming queues. We are usiing Task space to allowed OS to manage threads
        /// </summary>
        public void Listen()
        {
            _isRunning = true;

            Trace.TraceInformation("Starting Listening to {0}", QueueName);

            // ruun as long as app is running
            while (_isRunning && !PinkoApplication.ApplicationRunningEvent.WaitOne(0))
                PinkoApplication.ApplicationRunningEvent.WaitOne(10000);
        }


        /// <summary>
        /// Close
        /// </summary>
        public void Close()
        {
            _isRunning = false;
        }

        /// <summary>
        /// Send message into queue or topic
        /// </summary>
        /// <param name="message"></param>
        public void Send(IBusMessageOutbound message)
        {
            Trace.TraceInformation("(MemBus) Sending: {0} - {1}", QueueName, message.Verbose());
            // Jsut cast smae message over to inbound sinece we are inthe same process
            //_applicationBusMessageResponse.Publish((IBusMessageInbound) message);

            Interlocked.Increment(ref _outboudMessages);
            _messageHandlerManager.SendToHandler((IBusMessageInbound)message);
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
        /// MessageHandlerManager 
        /// </summary>
        private MessageHandlerManager _messageHandlerManager;

        /// <summary>
        /// Queue Name
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Aplication bus message
        /// </summary>
        private IRxMemoryBus<IBusMessageInbound> _applicationBusMessageResponse;

        /// <summary>
        /// Set signal when ready to stop lis
        /// </summary>
        private bool _isRunning = false;
    }
}
