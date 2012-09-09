using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon;
using PinkoCommon.Interface;
using PinkoWebRoleCommon.Interface;
using PinkoWebRoleCommon.SignalRHub;
using SignalR;
using SignalR.Hubs;

namespace PinkoWebRoleCommon
{
    /// <summary>
    /// Manages Signal R connectivity
    /// </summary>
    public class WebRoleSignalRManager : IWebRoleSignalRManager
    {
        /// <summary>
        /// Register all real-time message handlers to be routed via SiganlR to web clients
        /// </summary>
        public IWebRoleSignalRManager Initialize()
        {
            PinkoSingalHubContext = GlobalHost.ConnectionManager.GetHubContext<PinkoSingalHub>();

            Trace.TraceInformation("Registering message handlers: {0}", GetType());

            // route incoming heartbeat to all clients
            PinkoContainer.Resolve<IMessageHandlerManager>().GetSubscriber<PinkoRoleHeartbeat>()
                .Do(x => Trace.TraceInformation("WebRole Received: {0}: {1} - {2}", GetType(), x.Item1.Verbose(), x.Item2.ToString()))
                .Subscribe(x => PinkoSingalHubContext.Clients.notifyClientPinkoRoleHeartbeat(
                                                                                                x.Item1.PinkoProperties[PinkoMessagePropTag.MachineName],
                                                                                                x.Item1.PinkoProperties[PinkoMessagePropTag.DateTimeStamp])
                                                                                            );

            return this;
        }

        /// <summary>
        /// PinkoApplication
        /// </summary>
        [Dependency]
        public IPinkoApplication PinkoApplication { get; set; }

        /// <summary>
        /// IUnityContainer
        /// </summary>
        [Dependency]
        public IUnityContainer PinkoContainer { get; set; }

        /// <summary>
        /// SingalR publisher context
        /// </summary>
        public IHubContext PinkoSingalHubContext { get; private set; }
    }
}

