using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon;
using PinkoCommon.Extension;
using PinkoCommon.Interface;
using PinkoWebRoleCommon;
using PinkoWebRoleCommon.Extensions;
using PinkoWebRoleCommon.Interface;

namespace PinkoWebService.Controllers
{
    public class PinkoFormProcessorSubscriberController : ApiController
    {
        /// <summary>
        /// subscribe to formula
        /// </summary>
        /// <param name="expressionFormula"></param>
        /// <param name="maketEnvId"></param>
        /// <param name="clientCtx"></param>
        /// <param name="clientId"></param>
        /// <param name="signalRId"></param>
        /// <param name="webRoleId"></param>
        /// <param name="subscribtionId"> </param>
        /// <returns></returns>
        //[HttpGet]
        public HttpResponseMessage GetSubscribe(string expressionFormula,
                                                string maketEnvId,
                                                string clientCtx,
                                                string clientId,
                                                string signalRId,
                                                string webRoleId,
                                                string subscribtionId
                                            )
        {
            var expMsg = new PinkoMsgCalculateExpression
            {
                MsgAction = PinkoMessageAction.UserSubscription,
                ExpressionFormulas = PinkoUserExpressionFormulaCommonExtensions.FromUrlParameter(expressionFormula),
                ExpressionFormulasStr = expressionFormula.Replace(" ", string.Empty),
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

            if (expMsg.ExpressionFormulas.Length == 0)
                return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "Invalid Pinko formula. The expression is either empty or invalid" };

            var msgEnvelop = PinkoApplication.FactorWebEnvelop(clientCtx, WebRoleConnectManager.WebRoleId, expMsg);

            msgEnvelop.ReplyTo = PinkoConfiguration.PinkoMessageBusToWebRoleCalcResultTopic;
            msgEnvelop.QueueName = string.IsNullOrEmpty(webRoleId) ? PinkoConfiguration.PinkoMessageBusToWorkerSubscriptionManagerAllTopic : PinkoConfiguration.PinkoMessageBusToWorkerSubscriptionManagerTopic;

            msgEnvelop.PinkoProperties[PinkoMessagePropTag.RoleId] = WebRoleConnectManager.WebRoleId;
            ServerMessageBus.Publish(msgEnvelop);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        //// GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<controller>
        //public IEnumerable<string> Get(string expressionFormula,
        //                                        string maketEnvId,
        //                                        string clientCtx,
        //                                        string clientId,
        //                                        string signalRId,
        //                                        string webRoleId,
        //                                        string subscribtionId
        //                                    )
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}


        // GET api/<controller>/5
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
        /// Outbound message bus to worker roles
        /// </summary>
        [Dependency]
        public IRxMemoryBus<IBusMessageOutbound> ServerMessageBus { get; set; }

        /// <summary>
        /// IPinkoApplication
        /// </summary>
        [Dependency]
        public IPinkoApplication PinkoApplication { get; set; }

        /// <summary>
        /// IWebRoleConnectManager
        /// </summary>
        [Dependency]
        public IWebRoleConnectManager WebRoleConnectManager { get; set; }

        /// <summary>
        /// IPinkoConfiguration
        /// </summary>
        [Dependency]
        public IPinkoConfiguration PinkoConfiguration { get; set; }
    }
}