using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.Practices.Unity;
using Microsoft.WindowsAzure.ServiceRuntime;
using PinkDao;
using PinkoServices.Handlers;
using PinkoServices.IoC;
using PinkoWorkerCommon.Interface;
using PinkoWorkerCommon.Utility;

namespace PinkoServices
{
    /// <summary>
    /// Base type for all Azure Web roles.
    /// It mamanges internal bus conenctivity, etc for Pinko
    /// </summary>
    public class AzureWebRoleBase : RoleEntryPoint
    {
        /// <summary>
        /// Run()
        /// </summary>
        public override void Run()
        {
            // Register base handlers
            _handlers.AddRange(new object[]
            {
                PinkoContainer.Resolve<HandlerStringMessage>().Register(),
                PinkoContainer.Resolve<HandlerPinkoPingMessage>().Register(),
                PinkoContainer.Resolve<HandlerPinkoRoleHeartbeat>().Register()
            });

            // start hearbeats
            StartHeartBeat();

            // Start Listening to mesage bus incoming messages
            PinkoContainer
                .Resolve<IBusMessageServer>()
                .GetQueue(PinkoContainer.Resolve<IPinkoConfiguration>().MessageBusCrossWebRolesQueue)
                .Listen();

            // Return when application stops or queue stops
        }

        /// <summary>
        /// Worker Role OnStart()
        /// </summary>
        /// <returns></returns>
        public override bool OnStart()
        {
            //
            // The Worker role lifetime is managed bny Windows Azure, so there is not 
            // an IoC bootstrapper available.  
            // We manually set the container here and eventually as the starting point for each role.
            //
            PinkoContainer = ContainerManager.BuildContainer();  // Real Contianer
            PinkoApplication = PinkoContainer.Resolve<IPinkoApplication>();

            return base.OnStart();
        }

        /// <summary>
        /// Start Heart Beat
        /// </summary>
        public void StartHeartBeat()
        {
            // Set role heartbeat
            _heartbeatTimeObservable = Observable.Interval(TimeSpan.FromSeconds(PinkoContainer.Resolve<IPinkoConfiguration>().HeartbeatIntervalSec), Scheduler.ThreadPool);

            _outboundMessageBus = PinkoApplication.GetBus<IBusMessageOutbound>();

            // Send heartbeat
            _heartbeatTimeObservable
                .Subscribe(x => _outboundMessageBus.Publish(PinkoServiceMessageEnvelop.FactorClientMessage(PinkoContainer.Resolve<IPinkoConfiguration>().MessageBusWebRoleToClientsTopic, 
                                                                                                    new PinkoRoleHeartbeat
                                                                                                    {
                                                                                                        ResponderDateTime = DateTime.Now,
                                                                                                        ResponderMachine = PinkoApplication .MachineName
                                                                                                    })));
        }

        /// <summary>
        /// Worker Role OnStop()
        /// </summary>
        public override void OnStop()
        {
            Trace.TraceInformation("Stopping worker role: {0}");

            PinkoApplication.ApplicationRunningEvent.Set();
            PinkoContainer.Resolve<IBusMessageServer>().Deinitialize();

            Thread.Sleep(5000);
            base.OnStop();
        }

        /// <summary>
        /// Hold hnadlers
        /// </summary>
        readonly List<object> _handlers = new List<object>();

        /// <summary>
        /// Time sequence
        /// </summary>
        private IObservable<long> _heartbeatTimeObservable;

        /// <summary>
        /// Main Queue to send outbound messages
        /// </summary>
        private IRxMemoryBus<IBusMessageOutbound> _outboundMessageBus;

        /// <summary>
        /// PinkoContainer
        /// </summary>
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
