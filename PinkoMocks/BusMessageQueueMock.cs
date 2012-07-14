//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Reactive.Linq;
//using System.Text;
//using Microsoft.Practices.Unity;
//using PinkoWorkerCommon.Interface;
//using PinkoWorkerCommon.Utility;

//namespace PinkoMocks
//{
//    public class BusMessageQueueMock : IBusMessageQueue
//    {

//        /// <summary>
//        /// Constructor - BusMessageQueueMock 
//        /// </summary>
//        public BusMessageQueueMock()
//        {
//            QueueName = "QueueName";
//        }

//        /// <summary>
//        /// Initialize
//        /// </summary>
//        public void Initialize()
//        {
//        }

//        /// <summary>
//        /// Initialize
//        /// </summary>
//        public void Initialize(string azureServerConnectionString)
//        {
//            throw new NotImplementedException();
//        }

//        /// <summary>
//        /// Start listening to incoming queues. We are usiing Task space to allowed OS to manage threads
//        /// </summary>
//        public void Listen()
//        {
//        }


//        /// <summary>
//        /// Close
//        /// </summary>
//        public void Close()
//        {
//        }

//        /// <summary>
//        /// Send message into queue or topic
//        /// </summary>
//        /// <param name="message"></param>
//        public void Send(IBusMessageOutbound message)
//        {
//            OutboudMessages.Add(message);
//        }

//        /// <summary>
//        /// <summary>
//        /// Queue Name
//        /// </summary>
//        public string QueueName { get; set; }

//        /// <summary>
//        /// 
//        /// </summary>
//        public List<IBusMessageOutbound> OutboudMessages = new List<IBusMessageOutbound>();

//    }
//}
