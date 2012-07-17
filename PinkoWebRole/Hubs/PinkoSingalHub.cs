﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using PinkDao;
using PinkoWorkerCommon.Extensions;
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
            Debug.WriteLine(this.VerboseIdentity());
        }

        /// <summary>
        /// subscribe to instrument
        /// </summary>
        public void Subscribe(string instrumentName)
        {
            Debug.WriteLine("{0}: Subscribe(): instrumentName: {1}", this.Verbose(), instrumentName);
        }

        /// <summary>
        /// Client Connected
        /// </summary>
        public void ClientConnected(string clientId)
        {
            Debug.WriteLine("{0}: Subscribe(): clientId: {1} - Context.ConnectionId: {2}", this.Verbose(), clientId, Context.ConnectionId);
            NotifyClientPinkoRoleHeartbeat(new PinkoRoleHeartbeat());
        }

        /// <summary>
        /// Send to client
        /// </summary>
        public void NotifyClientPinkoRoleHeartbeat(PinkoRoleHeartbeat heartbeat)
        {
            Debug.WriteLine("{0}: NotifyClientPinkoRoleHeartbeat(): Context.ConnectionId: {1}", this.Verbose(), heartbeat.Verbose());
            Clients[Context.ConnectionId].OnPinkoRoleHeartbeat(heartbeat);
        }
    }


    /// <summary>
    /// PinkoSingalHubExtensions
    /// </summary>
    ///  
    public static class PinkoSingalHubExtensions
    {
        /// <summary>
        /// PinkoSingalHub
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Verbose(this PinkoSingalHub obj)
        {
            return string.Format("{0} "
                                            , obj.VerboseIdentity()
                                            );
        }
    }

}