using System;
using PinkoWebRoleCommon.Interface;
using SignalR.Hubs;

namespace PinkoMocks
{
    public class WebRoleSignalRManagerMock : IWebRoleSignalRManager
    {
        /// <summary>
        /// Register all real-time message handlers to be routed via SiganlR to web clients
        /// </summary>
        public IWebRoleSignalRManager Initialize()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SingalR publisher context
        /// </summary>
        public IHubContext PinkoSingalHubContext
        {
            get { return new HubContextMock(); }
            set {  }
        }

    }
}
