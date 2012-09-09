using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon;
using PinkoCommon.Interface;
using PinkoWorkerCommon.Interface;

namespace PinkoWorkerCommon
{
    /// <summary>
    /// Manage Worker Role heartbeat 
    /// </summary>
    public class WorkerRoleHeartBeat : IWorkerRoleHeartBeat
    {
        public IWorkerRoleHeartBeat Initialize()
        {
            //// Set role heartbeat
            //_heartbeatTimeObservable = Observable.Interval(TimeSpan.FromSeconds(PinkoConfiguration.HeartbeatIntervalSec), Scheduler.ThreadPool);

            //_outboundMessageBus = PinkoApplication.GetBus<IBusMessageOutbound>();

            //// Send heartbeat
            //_heartbeatTimeObservable
            //    .Subscribe(x => _outboundMessageBus.Publish(
            //        new PinkoServiceMessageEnvelop(PinkoApplication)
            //        {
            //            QueueName = PinkoConfiguration.PinkoMessageBusToWebAllRolesTopic,
            //            Message = new PinkoRoleHeartbeat()
            //        }));

            return this;
        }

        /// <summary>
        /// Time sequence
        /// </summary>
        private IObservable<long> _heartbeatTimeObservable;

        /// <summary>
        /// Main Queue to send outbound messages
        /// </summary>
        private IRxMemoryBus<IBusMessageOutbound> _outboundMessageBus;

        /// <summary>
        /// IPinkoApplication
        /// </summary>
        [Dependency]
        public IPinkoApplication PinkoApplication { get; set; }

        /// <summary>
        /// IPinkoConfiguration
        /// </summary>
        [Dependency]
        public IPinkoConfiguration PinkoConfiguration { private get; set; }
    }
}
