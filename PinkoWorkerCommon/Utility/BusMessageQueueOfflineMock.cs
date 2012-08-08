using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using PinkoWorkerCommon.Interface;

namespace PinkoWorkerCommon.Utility
{
    ///// <summary>
    ///// In Queue manager. Offline develpment. Simulate Messageing middleware.
    ///// </summary>
    //public class BusMessageQueueOfflineMock : IBusMessageQueue
    //{
    //    /// <summary>
    //    /// Constructor - BusMessageQueueMock 
    //    /// </summary>
    //    public BusMessageQueueOfflineMock()
    //    {
    //        QueueName = "QueueName";
    //    }

    //    /// <summary>
    //    /// Initialize
    //    /// </summary>
    //    public void Initialize()
    //    {
    //        // Internal memory message bus 
    //        _applicationBusMessageResponse = PinkoApplication.GetBus<IBusMessageInbound>();
    //        _applicationBusMessageSend = PinkoApplication.GetSubscriber<IBusMessageOutbound>();

    //        // Set listener for outbounce messages 
    //        _applicationBusMessageSend
    //            .Where(x => x.QueueName.Equals(QueueName))
    //            .Do(x => Trace.TraceInformation("Sending: {0}", x.Verbose()))
    //            .Subscribe(x =>
    //            {
    //                _applicationBusMessageResponse.Publish((IBusMessageInbound) x);
    //                //using (var bm = FactorNewOutboundMessage(x))
    //                //{
    //                //    // MUST ADD Deserializer type for new error type
    //                //    Debug.Assert(AzureQueueClientExtensions.TypeDeserializerdict.ContainsKey(bm.ContentType));

    //                //    AzureMessageClient.Send(bm);
    //                //}
    //            });
    //    }

    //    /// <summary>
    //    /// Start listening to incoming queues. We are usiing Task space to allowed OS to manage threads
    //    /// </summary>
    //    public void Listen()
    //    {
    //        _isRunning = true;

    //        Trace.TraceInformation("Starting Listening to {0}", QueueName);

    //        // ruun as long as app is running
    //        while (_isRunning && !PinkoApplication.ApplicationRunningEvent.WaitOne(0))
    //            PinkoApplication.ApplicationRunningEvent.WaitOne(10000);
    //    }


    //    /// <summary>
    //    /// Close
    //    /// </summary>
    //    public void Close()
    //    {
    //        _isRunning = false;
    //    }

    //    /// <summary>
    //    /// IPinkoApplication
    //    /// </summary>
    //    [Dependency]
    //    public IPinkoApplication PinkoApplication { get; set; }

    //    /// <summary>
    //    /// Queue Name
    //    /// </summary>
    //    public string QueueName { get; set; }


    //    /// <summary>
    //    /// Aplication bus message
    //    /// </summary>
    //    private IRxMemoryBus<IBusMessageInbound> _applicationBusMessageResponse;

    //    /// <summary>
    //    /// Receive all message vis this Rxmemory bus
    //    /// </summary>
    //    private IObservable<IBusMessageOutbound> _applicationBusMessageSend;

    //    /// <summary>
    //    /// Set signal when ready to stop lis
    //    /// </summary>
    //    private bool _isRunning = false;
    //}
}
