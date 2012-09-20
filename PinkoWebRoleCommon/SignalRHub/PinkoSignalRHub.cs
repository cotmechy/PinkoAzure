using System;
using System.Diagnostics;
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
    public class PinkoSingalHub : Hub
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
        public void ClientConnected(string clientId, string oldWebRoleId)
        {
            Debug.WriteLine("{0}: ClientConnected(): clientId: {1} - Context.ConnectionId: {2}", this.Verbose(), clientId, Context.ConnectionId);

            // TODO: Send broadcast to all calc engines to clients to replace routing web role id
            
            var clientIdentifier = new PinkoMsgClientConnect
            {
                DataFeedIdentifier =
                {
                    ClientId = clientId,
                    SignalRId = Context.ConnectionId,
                    PreviousWebRoleId = oldWebRoleId,
                    WebRoleId = WebRoleConnectManager.WebRoleId
                }
            };

            var msgEnvelop = PinkoApplication.FactorWebEnvelop(clientId, WebRoleConnectManager.WebRoleId, clientIdentifier);

            // Send to Worker roles
            msgEnvelop.QueueName = PinkoConfiguration.PinkoMessageBusToAllWorkerRolesTopic;
            ServerMessageBus.Publish(msgEnvelop);

            //// Send to Web roles
            //msgEnvelop.QueueName = PinkoConfiguration.PinkoMessageBusToWebRoleTopic;
            //ServerMessageBus.Publish(msgEnvelop);
            Clients[Context.ConnectionId].reconnectionIdentifiers(clientId, clientIdentifier.DataFeedIdentifier.SignalRId, clientIdentifier.DataFeedIdentifier.WebRoleId);
        }


        /// <summary>
        /// Send to client related keys
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
        public void ExpressionResponse(string clientCtx, string expression, string resultType, string resultValue)
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
