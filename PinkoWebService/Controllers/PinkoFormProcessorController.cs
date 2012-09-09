using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon.Interface;
using PinkoWebRoleCommon.Extensions;
using PinkoWebRoleCommon.Interface;
using PinkoWebRoleCommon.IoC;

namespace PinkoWebService.Controllers
{
    public class PinkoFormProcessorController : ApiController
    {

        ///// <summary>
        ///// Constructor - PinkoFormProcessorController 
        ///// </summary>
        //public PinkoFormProcessorController()
        //{
        //    //PinkoApplication = PinkoWebRoleContainerManager
        //    //    .Container
        //    //    .Resolve<IPinkoApplication>();

        //    //_webRoleConnectManager =
        //    //    PinkoWebRoleContainerManager
        //    //        .Container.Resolve<IWebRoleConnectManager>();

        //    //_serverMessageBus = PinkoApplication
        //    //    .GetBus<IBusMessageOutbound>();
        //}


        ////
        //// GET: /PinkoFormProcessor/
        //public HttpResponseMessage Get()
        //{
        //    Trace.TraceInformation(GetType() + ": Get");

        //    return new HttpResponseMessage(HttpStatusCode.OK);
        //}


        ////
        //// GET: /PinkoFormProcessor/
        //public HttpResponseMessage PostCalculateFormula(PinkoCalculateExpression reqFormula)
        //{
        //    //Trace.TraceInformation(GetType() + ": Post(PinkoCalculateExpression): " + reqFormula.Verbose());

        //    reqFormula.ResultValue = double.MinValue; 

        //    ServerMessageBus.Publish(PinkoApplication.FactorWebEnvelop("", WebRoleConnectManager.WebRoleId, reqFormula));

        //    return new HttpResponseMessage(HttpStatusCode.OK);
        //}

        //
        // GET: /PinkoFormProcessor/
        //public HttpResponseMessage GetCalc(PinkoCalculateExpression reqForm)
        public HttpResponseMessage GetCalc(string expressionFormula, string maketEnvId, string clientCtx)
        {
            var expMsg = new PinkoCalculateExpression
            {
                ExpressionFormula = expressionFormula,
                MaketEnvId = maketEnvId,
                //ResultValue = double.MinValue,
                ClientCtx = clientCtx
            };

            var msgEnvelop = PinkoApplication.FactorWebEnvelop(clientCtx, WebRoleConnectManager.WebRoleId, expMsg);
            //Trace.TraceInformation("GetCalc: {0}", expMsg.Verbose());

            msgEnvelop.ReplyTo = PinkoConfiguration.PinkoMessageBusToWebFeedToClientTopic;
            msgEnvelop.QueueName = PinkoConfiguration.PinkoMessageBusToWorkerCalcEngineActionTopic;

            ServerMessageBus.Publish(msgEnvelop);

            return new HttpResponseMessage(HttpStatusCode.OK);
            //var response = Request.CreateResponse<PinkoCalculateExpression>(HttpStatusCode.Created, reqForm);
            //string uri = Url.Route(null, new { id = reqForm.MaketEnvId });
            //response.Headers.Location = new Uri(Request.RequestUri, uri);
            //return response;

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

        ////
        //// GET: /PinkoFormProcessor/
        //public string GetRequestFormula(string clientCtx, string marketEnvId, string expressionFormula)
        //{

        //    var expMsg = new PinkoCalculateExpression
        //    {
        //        ExpressionFormula = expressionFormula,
        //        MaketEnvId = marketEnvId,
        //        ResultValue = double.MinValue,
        //        ClientCtx = clientCtx

        //    };

        //    _serverMessageBus.Publish(_pinkoApplication.FactorWebEnvelop("", _webRoleConnectManager.WebRoleId, expMsg));

        //    return string.Empty;
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
