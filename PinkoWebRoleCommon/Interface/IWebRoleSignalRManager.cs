using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SignalR.Hubs;

namespace PinkoWebRoleCommon.Interface
{
    public interface IWebRoleSignalRManager
    {
        /// <summary>
        /// Register all real-time message handlers to be routed via SiganlR to web clients
        /// </summary>
        IWebRoleSignalRManager Initialize();

        /// <summary>
        /// SingalR publisher context
        /// </summary>
        IHubContext PinkoSingalHubContext { get; }
    }
}
