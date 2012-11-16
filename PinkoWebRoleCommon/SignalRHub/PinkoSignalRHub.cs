using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using PinkDao;
using PinkoCommon;
using PinkoCommon.Extensions;
using PinkoCommon.Interface;
using PinkoWebRoleCommon.Interface;
using SignalR.Hubs;
using PinkoWebRoleCommon.Extensions;

namespace PinkoWebRoleCommon.SignalRHub
{
    /// <summary>
    /// SignalR Hub
    /// </summary>
    [HubName("PinkoSingalHub")]
    public class PinkoSingalHub : Hub, IConnected, IDisconnect
    {
        /// <summary>
        /// Global instance. Factored by SignalR. We need to have a reference to it.
        /// </summary>
        public static Action<PinkoSingalHub> PinkoSingalHubInitBus = null;

        /// <summary>
        /// Constructor - PinkoSingalHub 
        /// </summary>
        public PinkoSingalHub()
        {
            Debug.WriteLine(this.VerboseIdentity());

            PinkoSingalHubInitBus(this); // Unity dependencies here because SignalR has control of the instantiation.
        }

        ///// <summary>
        ///// subscribe to instrument
        ///// </summary>
        //public void Subscribe(string instrumentName)
        //{
        //    Debug.WriteLine("{0}: Subscribe(): instrumentName: {1}", this.Verbose(), instrumentName);
        //}

        /// <summary>
        /// Client Connected - Called from the client javascript page.
        /// </summary>
        public void ClientConnected(string clientId, string oldWebRoleId, string oldSignalRId)
        {
            Debug.WriteLine("{0}: ClientConnected(): clientId: {1} - Context.ConnectionId: {2}", this.Verbose(), clientId, Context.ConnectionId);

            var clientIdentifier = new PinkoMsgClientConnect
            {
                DataFeedIdentifier =
                {
                    ClientId = clientId,
                    SignalRId = Context.ConnectionId,
                    WebRoleId = WebRoleConnectManager.WebRoleId,

                    PreviousWebRoleId = oldWebRoleId,
                    PreviousSignalRId = oldSignalRId
                }
            };

            var msgEnvelop = PinkoApplication.FactorWebEnvelop(clientId, WebRoleConnectManager.WebRoleId, clientIdentifier);

            // Broadcast to all Worker roles
            msgEnvelop.QueueName = PinkoConfiguration.PinkoMessageBusToWorkerCalcEngineAllTopic; //.PinkoMessageBusToWorkerAllSubscriptionManagerTopic;
            ServerMessageBus.Publish(msgEnvelop);

            // Send to Web roles
            Clients[Context.ConnectionId].reconnectionIdentifiers(clientId, clientIdentifier.DataFeedIdentifier.SignalRId, clientIdentifier.DataFeedIdentifier.WebRoleId);
        }


        /// <summary>
        /// Send to client - SignalR will stub this method in the browser
        /// </summary>
        public void ReconnectionIdentifiers(string clientId, string signalRId, string webRoleId)
        {
            // Do not implement. Implemented in browser by SignalR
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
        public void ExpressionResponse(string clientCtx, string results, string resultType, string subscribtionId)
        {
            // Do not implement. Implemented in browser by SignalR
        }

        /// <summary>
        /// IWebRoleConnectManager
        /// </summary>
        public IWebRoleConnectManager WebRoleConnectManager { get; set; }

        /// <summary>
        /// PinkoApplication
        /// </summary>
        public IPinkoApplication PinkoApplication { get; set; }

        /// <summary>
        /// Outbound message bus to worker roles
        /// </summary>
        public IRxMemoryBus<IBusMessageOutbound> ServerMessageBus { get; set; }

        /// <summary>
        /// IPinkoConfiguration
        /// </summary>
        public IPinkoConfiguration PinkoConfiguration { private get; set; }

        /// <summary>
        /// Called when a new connection is made to the <see cref="T:SignalR.Hubs.IHub"/>.
        /// </summary>
        public Task Connect()
        {
            Trace.TraceInformation("PinkoSingalHub: SignaR Hub Connected: {0} - {1}", Context.ConnectionId, DateTime.Now.ToString());
            return Clients.joined(Context.ConnectionId, DateTime.Now.ToString());
        }

        /// <summary>
        /// Called when a connection reconencts to the <see cref="T:SignalR.Hubs.IHub"/> after a timeout.
        /// </summary>
        /// <param name="groups">The groups the connection are a member of.</param>
        public Task Reconnect(IEnumerable<string> groups)
        {
            Trace.TraceInformation("PinkoSingalHub: SignaR Hub Reconnect: {0} - {1}", Context.ConnectionId, DateTime.Now.ToString());
            return Clients.rejoined(Context.ConnectionId, DateTime.Now.ToString());
        }

        /// <summary>
        /// Called when a connection is disconnected from the <see cref="T:SignalR.Hubs.IHub"/>.
        /// </summary>
        /// <remarks>
        /// This method is invoked from the server side which means the only valid property on the <see cref="T:SignalR.Hubs.HubCallerContext"/>
        ///             is the connection id.
        /// </remarks>
        public Task Disconnect()
        {
            Trace.TraceInformation("PinkoSingalHub: SignaR Hub Disconnected: {0} - {1}", Context.ConnectionId, DateTime.Now.ToString());
            return Clients.leave(Context.ConnectionId, DateTime.Now.ToString());
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
            return string.Format("{0} - ConnectionId: {1}", 
                                        obj.VerboseIdentity(), 
                                        obj.Context.ConnectionId
                                        );
        }
    }
}
