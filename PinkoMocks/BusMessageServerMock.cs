//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Reactive.Linq;
//using System.Text;
//using PinkoWorkerCommon.Interface;

//namespace PinkoMocks
//{
//    public class BusMessageServerMock : IBusMessageServer
//    {
//        /// <summary>
//        /// Constructor - BusMessageServerMock 
//        /// </summary>
//        public BusMessageServerMock(IPinkoApplication pinkoApplication)
//        {
//            // Set listener for outbound messages 
//            pinkoApplication
//                .GetSubscriber<IBusMessageOutbound>()
//                .Do(x => Trace.TraceInformation("(BusMessageServerMock) Sending: {0}", x.Verbose()))
//                .Subscribe(x => GetQueue(string.IsNullOrEmpty(x.ReplyTo) ? x.QueueName : x.ReplyTo).Send(x));
//        }


//        /// <summary>
//        /// Server connection string
//        /// </summary>
//        public string AzureServerConnectionString
//        {
//            get { return "AzureServerConnectionString"; }
//        }

//        /// <summary>
//        /// Connect queue
//        /// </summary>
//        public IBusMessageQueue ConnectToQueue(string queueName)
//        {
//            if (!Buses.ContainsKey(queueName))
//                Buses[queueName] = new BusMessageQueueMock() { QueueName = queueName };

//            return Buses[queueName];
//        }

//        /// <summary>
//        /// Connect queue
//        /// </summary>
//        public IBusMessageQueue GetQueue(string queueName)
//        {
//            return ConnectToQueue(queueName);
//        }

//        /// <summary>
//        /// Initialize message bus
//        /// </summary>
//        public void Initialize()
//        {
//            // Nothing to do
//        }

//        /// <summary>
//        /// Deinitialize message bus
//        /// </summary>
//        public void Deinitialize()
//        {
//            // Nothing to do
//        }

//        /// <summary>
//        /// Buses
//        /// </summary>
//        public Dictionary<string, BusMessageQueueMock> Buses = new Dictionary<string, BusMessageQueueMock>(); 
//    }
//}
