using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace PinkoWebRoleCommon.Interface
{
    /// <summary>
    /// Connect manger
    /// </summary>
    public interface IWebRoleConnectManager
    {
        /// <summary>
        /// Initialize 
        /// </summary>
        IWebRoleConnectManager InitializeWebRole();

        ///// <summary>
        ///// Register all real-time message handlers to be routed via SiganlR to web clients
        ///// </summary>
        //void RegisterRealTimeMessageHandlers();

        ///// <summary>
        ///// Mock heartbeat publisher to use in development. Mocks server her beat
        ///// </summary>
        //void StartmockHearbeat();

        /// <summary>
        /// Stop all inproc pinko services, etc..
        /// </summary>
        void StopPinkoWebRole();

        /// <summary>
        /// WebRoleId
        /// </summary>
        string WebRoleId { get; }

        /// <summary>
        /// handlers to messages received from the bus. delivered via Rx in memory bus
        /// </summary>
        List<object> BusListenerHandlers { get; }
    }
}
