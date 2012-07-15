using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using PinkDao;
using SignalR.Hubs;

namespace PinkoWebRole.Hubs
{
    /// <summary>
    /// SignalR Hub
    /// </summary>
    [HubName("PinkoSingalHub")]
    public class PinkoSingalHub : Hub
    {
        /// <summary>
        /// Constructor - PinkoSingalHub 
        /// </summary>
        public PinkoSingalHub()
        {
            Debug.WriteLine("**** PinkoSingalHub()");
        }

        /// <summary>
        /// subscribe to instrument
        /// </summary>
        public void Subscribe(string instrumentName)
        {
            Debug.WriteLine("**** Subscribe:instrumentName: {0}", instrumentName);
        }

        /// <summary>
        /// Client Connected
        /// </summary>
        public void ClientConnected(string clientId)
        {
            Debug.WriteLine(string.Format("**** ClientConnected: clientId: {0} - Context.ConnectionId: {1}", clientId, Context.ConnectionId));
            NotifyClientPinkoRoleHeartbeat(new PinkoRoleHeartbeat());
        }

        /// <summary>
        /// Send to client
        /// </summary>
        public void NotifyClientPinkoRoleHeartbeat(PinkoRoleHeartbeat heartbeat)
        {
            Clients[Context.ConnectionId].addMessage(heartbeat);
        }
    }
}