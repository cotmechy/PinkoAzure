using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.Http;
using Microsoft.Practices.Unity;
using PinkoCommon.Interface;
using PinkoWebRoleCommon.Interface;
using PinkoWebRoleCommon.IoC;

namespace PinkoWebRoleCommon
{
    /// <summary>
    /// Manages connectivity with messaging bus
    /// </summary>
    public class WebRoleConnectManager : IWebRoleConnectManager
    {
        /// <summary>
        /// Constructor - WebRoleConnectManager 
        /// </summary>
        public IWebRoleConnectManager Initialize()
        {
            // Start listening to incoming topics
            PinkoApplication.RunInWorkerThread(PinkoContainer.Resolve<IPinkoConfiguration>().PinkoMessageBusToWebRolesAllTopic,
                    () => // All Worker roles
                    PinkoContainer
                        .Resolve<IBusMessageServer>()
                        .GetTopic(PinkoContainer.Resolve<IPinkoConfiguration>().PinkoMessageBusToWebRolesAllTopic)
                        .Listen()
                );

            PinkoApplication.RunInWorkerThread(PinkoContainer.Resolve<IPinkoConfiguration>().PinkoMessageBusToWebRoleTopic,
                    () => // Selected
                    PinkoContainer
                        .Resolve<IBusMessageServer>()
                        .GetTopic(PinkoContainer.Resolve<IPinkoConfiguration>().PinkoMessageBusToWebRoleTopic, Guid.NewGuid().ToString())
                        .Listen()
                );

            return this;
        }

        /// <summary>
        /// Initialize Web Roles
        /// </summary>
        public IWebRoleConnectManager InitializeWebRole()
        {
            GlobalConfiguration.Configuration.DependencyResolver = PinkoContainer.Resolve<PinkoWebDependencyResolver>();

            //RegisterRealTimeMessageHandlers();

            return this;
        }

        ///// <summary>
        ///// Register all real-time message handlers to be routed via SiganlR to web clients
        ///// </summary>
        //public void RegisterRealTimeMessageHandlers()
        //{
        //    var pinkoSingalHubContext = GlobalHost.ConnectionManager.GetHubContext<PinkoSingalHub>();

        //    Trace.TraceInformation("Registering message handlers: {0}", GetType());

        //    // route incoming heartbeat to all clients
        //    PinkoApplication
        //        .GetSubscriber<Tuple<IBusMessageInbound, PinkoRoleHeartbeat>>()
        //        .Do(x => Trace.TraceInformation("WebRole Received: {0}: {1} - {2}", GetType(), x.Item1.Verbose(), x.Item2.ToString()))
        //        .Subscribe(x => pinkoSingalHubContext.Clients.notifyClientPinkoRoleHeartbeat(x.Item1.PinkoProperties[PinkoMessagePropTag.MachineName],
        //                                                                                     x.Item1.PinkoProperties[PinkoMessagePropTag.DateTimeStamp]));
        //}


        ///// <summary>
        ///// Mock heartbeat publisher to use in development. Mocks server her beat
        ///// </summary>
        //public void StartmockHearbeat()
        //{
        //    // TODO: Temporary
        //    // timer
        //    _webRoleHearBeat = Observable.Interval(TimeSpan.FromMilliseconds(1000), Scheduler.ThreadPool);


        //    // Set listener for outbound messages 
        //    var incomingTopic =
        //        PinkoApplication
        //            .GetBus<IBusMessageOutbound>();

        //    _webRoleHearBeat
        //        .Subscribe(x => incomingTopic.Publish(PinkoApplication.FactorWebEnvelop(string.Empty, new PinkoRoleHeartbeat())));
        //}

        /// <summary>
        /// Stop all inproc pinko services, etc..
        /// </summary>
        public void StopPinkoWebRole()
        {
            PinkoApplication.ApplicationRunningEvent.Set();
            Thread.Sleep(3000);
        }


        ///// <summary>
        ///// Configure API with unity resolver to load handlers with unity
        ///// </summary>
        //public void ConfigureApi(HttpConfiguration config)
        //{

        //    //PinkoContainer = PinkoWebRoleContainerManager.Container;
        //    //config.DependencyResolver = PinkoWebRoleContainerManager.Container;
        //    config.DependencyResolver = PinkoContainer.Resolve<PinkoWebDependencyResolver>();
        //}

        ///// <summary>
        ///// handlers to messages received from the bus. delivered via Rx in memory bus
        ///// </summary>
        //public List<object> BusListenerHandlers
        //{
        //    get { return _busListenerHandlers; }
        //}
        //private readonly List<object> _busListenerHandlers = new List<object>();


        /// <summary>
        /// WebRoleId
        /// </summary>
        public string WebRoleId
        {
            get { return _webRoleId; }
        }

        /////// <summary>
        /////// Heartbeat Reactive extension - processes via SignalR
        /////// </summary>
        //private IObservable<long> _webRoleHearBeat;

        /// <summary>
        /// Unique web role id
        /// </summary>
        private readonly string _webRoleId = Guid.NewGuid().ToString();

        /// <summary>
        /// PinkoContainer
        /// </summary>
        [Dependency]
        public IUnityContainer PinkoContainer { get; set; }

        /// <summary>
        /// PinkoApplication
        /// </summary>
        [Dependency]
        public IPinkoApplication PinkoApplication { get; set; }
    }
}
