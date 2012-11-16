using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon;
using PinkoCommon.Interface;
using PinkoWebRoleCommon.Extensions;
using PinkoWebRoleCommon.Interface;

namespace PinkoWebService.Controllers
{
    public class PinkoClientPingController : ApiController
    {

        /// <summary>
        /// client ping accessor
        /// </summary>
        /// <param name="maketEnvId"></param>
        /// <param name="clientCtx"></param>
        /// <param name="clientId"></param>
        /// <param name="signalRId"></param>
        /// <param name="webRoleId"></param>
        /// <param name="subscribtionId"></param>
        /// <returns></returns>
        public HttpResponseMessage GetPing(string maketEnvId,
                                                string clientCtx,
                                                string clientId,
                                                string signalRId,
                                                string webRoleId,
                                                string subscribtionId
                                            )
        {
            if (string.IsNullOrEmpty(maketEnvId)) 
                return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "Invalid Pinko Ping. Missing Market Environment Id" };

            if (string.IsNullOrEmpty(clientCtx)) 
                return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "Invalid Pinko Ping. Missing Client Context." };

            if (string.IsNullOrEmpty(clientId)) 
                return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "Invalid Pinko Ping. Missing Client Id." };

            if (string.IsNullOrEmpty(signalRId)) 
                return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "Invalid Pinko Ping. Missing SignalR Id." };

            if (string.IsNullOrEmpty(webRoleId)) 
                return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "Invalid Pinko Ping. Missing Web Role Id." };

            if (string.IsNullOrEmpty(subscribtionId)) 
                return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "Invalid Pinko Ping. Missing Subscription Id" };

            var expMsg = new PinkoMsgClientPing
            {
                DataFeedIdentifier =
                {
                    MaketEnvId = maketEnvId,
                    ClientCtx = clientCtx,
                    ClientId = clientId,
                    SignalRId = signalRId,
                    WebRoleId = webRoleId,
                    SubscribtionId = subscribtionId
                }
            };

            var msgEnvelop = PinkoApplication.FactorWebEnvelop(clientCtx, WebRoleConnectManager.WebRoleId, expMsg);

            msgEnvelop.ReplyTo = string.Empty;
            msgEnvelop.QueueName = PinkoConfiguration.PinkoMessageBusToWorkerSubscriptionManagerAllTopic;

            msgEnvelop.PinkoProperties[PinkoMessagePropTag.RoleId] = WebRoleConnectManager.WebRoleId;
            ServerMessageBus.Publish(msgEnvelop);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        //// GET api/<controller>
        //public IEnumerable<string> GetPing(string )

        //            public HttpResponseMessage GetCalc(string expressionFormula,
        //                                    string maketEnvId,
        //                                    string clientCtx,
        //                                    string clientId,
        //                                    string signalRId,
        //                                    string webRoleId
        //                                    )

        //{
        //    return new string[] { string.Empty, string.Empty };
        //}

        //// GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}


        /// <summary>
        /// PinkoApplication
        /// </summary>
        [Dependency]
        public IPinkoApplication PinkoApplication { get; set; }

        /// <summary>
        /// IPinkoConfiguration
        /// </summary>
        [Dependency]
        public IPinkoConfiguration PinkoConfiguration { private get; set; }

        /// <summary>
        /// Outbound message bus to worker roles
        /// </summary>
        [Dependency]
        public IRxMemoryBus<IBusMessageOutbound> ServerMessageBus { get; set; }

        /// <summary>
        /// IWebRoleConnectManager
        /// </summary>
        [Dependency]
        public IWebRoleConnectManager WebRoleConnectManager { get; set; }
    }
}