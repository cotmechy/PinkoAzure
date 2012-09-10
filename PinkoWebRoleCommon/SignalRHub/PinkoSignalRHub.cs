using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PinkDao;
using PinkoCommon.Extensions;
//using PinkoWebRoleCommon.HubModels;
using SignalR.Hubs;

namespace PinkoWebRoleCommon.SignalRHub
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
            Debug.WriteLine("{0}: ClientConnected(): clientId: {1} - Context.ConnectionId: {2}", this.Verbose(),
                            clientId, Context.ConnectionId);
        }

        /// <summary>
        /// Send to client - SignalR will stub this method in the browser
        /// </summary>
        public void NotifyClientPinkoRoleHeartbeat(string dateTimeStamp, string machineName, string signalRId)
        {
            // Do not implement. Implemented in browser by SignalR
        }


        /// <summary>
        /// Send to client - SignalR will stub this method in the browser
        /// </summary>
        public void ExpressionResponseError(string clientCtx, string expression, string resultValue, string errorCode, string errorDescription)
        {
            // Do not implement. Implemented in browser by SignalR
        }

        /// <summary>
        /// Send to client - SignalR will stub this method in the browser
        /// </summary>
        public void ExpressionResponse(string clientCtx, string expression, string resultType, string resultValue)
        {
            // Do not implement. Implemented in browser by SignalR
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
            return string.Format("{0} ", obj.VerboseIdentity());
        }
    }
}
