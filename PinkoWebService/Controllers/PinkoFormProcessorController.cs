using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon;
using PinkoCommon.Extension;
using PinkoCommon.Interface;
using PinkoWebRoleCommon.Extensions;
using PinkoWebRoleCommon.Interface;

namespace PinkoWebService.Controllers
{
    public class PinkoFormProcessorController : ApiController
    {
        //
        // GET: /PinkoFormProcessor/
        // public HttpResponseMessage GetCalc(PinkoCalculateExpression reqForm)
        public HttpResponseMessage GetCalc( string expressionFormula, 
                                            string maketEnvId, 
                                            string clientCtx, 
                                            string clientId, 
                                            string signalRId,
                                            string webRoleId
                                            )
        {
            var expMsg = new PinkoMsgCalculateExpression
                {
                    ExpressionFormulas = PinkoUserExpressionFormulaCommonExtensions.FromUrlParameter(expressionFormula),
                    ExpressionFormulasStr = expressionFormula.Replace(" ", string.Empty),
                    DataFeedIdentifier =
                        {
                            MaketEnvId = maketEnvId,
                            ClientCtx = clientCtx,
                            ClientId = clientId,
                            SignalRId = signalRId,
                            WebRoleId = webRoleId,
                            SubscribtionId = "subscribtionId"
                        }
                };

            if (expMsg.ExpressionFormulas.Length == 0)
                return new HttpResponseMessage(HttpStatusCode.BadRequest) {ReasonPhrase = "Invalid Pinko formula. The expression is either empty or invalid"};

            var msgEnvelop = PinkoApplication.FactorWebEnvelop(clientCtx, WebRoleConnectManager.WebRoleId, expMsg);

            msgEnvelop.ReplyTo = PinkoConfiguration.PinkoMessageBusToWebRoleCalcResultTopic;
            msgEnvelop.QueueName = PinkoConfiguration.PinkoMessageBusToWorkerAllSubscriptionManagerTopic;

            msgEnvelop.PinkoProperties[PinkoMessagePropTag.RoleId] = WebRoleConnectManager.WebRoleId; 
            ServerMessageBus.Publish(msgEnvelop);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        // // http://www.codeproject.com/Articles/344078/ASP-NET-WebAPI-Getting-Started-with-MVC4-and-WebAP
        //public HttpResponseMessage PostBook(Book book)
        //{
        //    book = _repository.Add(book);
        //    var response = Request.CreateResponse<Book>(HttpStatusCode.Created, book);
        //    string uri = Url.Route(null, new { id = book.Id });
        //    response.Headers.Location = new Uri(Request.RequestUri, uri);
        //    return response;
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
