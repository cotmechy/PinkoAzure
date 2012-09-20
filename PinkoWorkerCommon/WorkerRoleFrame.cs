using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon;
using PinkoCommon.Interface;
using PinkoWorkerCommon.Handler;
using PinkoWorkerCommon.Interface;

namespace PinkoWorkerCommon
{
    /// <summary>
    /// Base worker role setup and runtime
    /// </summary>
    public class WorkerRoleFrame : IWorkerRoleFrame
    {
        /// <summary>
        /// Run()
        /// </summary>
        public void Run()
        {
            // Register base handlers
            MessageReceiveHandlers.AddRange(new object[]
            {
                PinkoContainer.Resolve<BusListenerPinkoPingMessage>().Register(),
            });

            // TODO: Move to individual service
            PinkoApplication.RunInWorkerThread("PinkoMessageBusToWorkerCalcEngineActionTopic",
                () => // Start Listening to message bus incoming messages - MessageBusWebRoleToClientsTopic
                PinkoContainer
                    .Resolve<IBusMessageServer>()
                    .GetTopic(PinkoConfiguration.PinkoMessageBusToWorkerCalcEngineTopic, _selector)
                    .Listen()
                );

            PinkoApplication.RunInWorkerThread("PinkoMessageBusToWorkerAllRolesTopic",
                () => // Start Listening to mesage bus incoming messages - MAIN: MessageBusCrossWebRolesQueue
                PinkoContainer
                    .Resolve<IBusMessageServer>()
                    .GetTopic(PinkoConfiguration.PinkoMessageBusToAllWorkerRolesTopic)
                    .Listen());

            // Return when application stops or queue stops
        }

        ///// <summary>
        ///// Worker Role OnStart()
        ///// </summary>
        ///// <returns></returns>
        //public override bool OnStart()
        //{
        //    //
        //    // The Worker role lifetime is managed bny Windows Azure, so there is not 
        //    // an IoC bootstrapper available.  
        //    // We manually set the container here and eventually as the starting point for each role.
        //    //
        //    PinkoContainer = PinkoServiceContainer.BuildContainer();  // Real Contianer
        //    PinkoApplication = PinkoContainer.Resolve<IPinkoApplication>();

        //    return base.OnStart();
        //}

        ///// <summary>
        ///// Start Heart Beat
        ///// </summary>
        //public void StartHeartBeat()
        //{
        //    // Set role heartbeat
        //    _heartbeatTimeObservable = Observable.Interval(TimeSpan.FromSeconds(PinkoConfiguration.HeartbeatIntervalSec), Scheduler.ThreadPool);

        //    _outboundMessageBus = PinkoApplication.GetBus<IBusMessageOutbound>();

        //    // Send heartbeat
        //    _heartbeatTimeObservable
        //        .Subscribe(x => _outboundMessageBus.Publish(
        //            new PinkoServiceMessageEnvelop(PinkoApplication)
        //            {
        //                QueueName = PinkoConfiguration.PinkoMessageBusToWebAllRolesTopic,
        //                Message = new PinkoRoleHeartbeat()
        //            }));
        //}

        /// <summary>
        /// Worker Role OnStop()
        /// </summary>
        public void Stop()
        {
            Trace.TraceInformation("Stopping worker role: {0}");

            PinkoApplication.ApplicationRunningEvent.Set();
            PinkoContainer.Resolve<IBusMessageServer>().Deinitialize();

            Thread.Sleep(5000);
        }


        /// <summary>
        /// Unique Selector
        /// </summary>
        private readonly string _selector = Guid.NewGuid().ToString();

        /// <summary>
        /// MessageHandlers 
        /// </summary>
        public List<object> MessageReceiveHandlers
        {
            get { return _messageReceiveHandlers; }
        }
        private readonly List<object> _messageReceiveHandlers = new List<object>();

        /// <summary>
        /// PinkoContainer
        /// </summary>
        [Dependency]
        public IUnityContainer PinkoContainer { get; set; }

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
