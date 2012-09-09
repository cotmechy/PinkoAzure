using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinkoWebRoleCommon.Interface;

namespace PinkoMocks
{
    public class WebRoleConnectManagerMock : IWebRoleConnectManager
    {
        private string _webRoleId;

        /// <summary>
        /// Initialize 
        /// </summary>
        public IWebRoleConnectManager InitializeWebRole()
        {
            return this;
        }

        /// <summary>
        /// Register all real-time message handlers to be routed via SiganlR to web clients
        /// </summary>
        public void RegisterRealTimeMessageHandlers()
        {
            // noting to do
        }

        /// <summary>
        /// Mock heartbeat publisher to use in development. Mocks server her beat
        /// </summary>
        public void StartmockHearbeat()
        {
            // Nothing to do
        }

        /// <summary>
        /// Stop all inproc pinko services, etc..
        /// </summary>
        public void StopPinkoWebRole()
        {
            // Nothing to do
        }

        /// <summary>
        /// WebRoleId
        /// </summary>
        public string WebRoleId 
        {
            get { return "WebRoleIdMock"; }
        }

        /// <summary>
        /// handlers to messages received from the bus. delivered via Rx in memory bus
        /// </summary>
        public List<object> BusListenerHandlers { get; private set; }
    }
}
