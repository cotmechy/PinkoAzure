using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.Interface;
using PinkoWebRoleCommon;
using PinkoWebRoleCommon.Interface;
using PinkoWebRoleCommon.IoC;
using PinkoWebRoleCommon.RoleHandler;
using PinkoWebRoleCommon.SignalRHub;

namespace PinkoViewer
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            var pinkoContainer = PinkoWebRoleContainerManager.Container;

            // This worker role handles these messages - Registers Rx publisher that listens to message bus
            pinkoContainer.Resolve<IIncominBusMessageHandlerManager>().AddBusTypeHandler<PinkoMsgRoleHeartbeat>();
            pinkoContainer.Resolve<IIncominBusMessageHandlerManager>().AddBusTypeHandler<PinkoMsgCalculateExpressionResult>();




            // Hub will initialize when constructed by SignalR
            PinkoSingalHub.PinkoSingalHubInitBus = hub =>
                {
                    // Inject member manually after instance created
                    hub.WebRoleConnectManager = pinkoContainer.Resolve<IWebRoleConnectManager>();
                    hub.PinkoApplication = pinkoContainer.Resolve<IPinkoApplication>();
                    hub.ServerMessageBus = pinkoContainer.Resolve<IRxMemoryBus<IBusMessageOutbound>>();
                    hub.PinkoConfiguration = pinkoContainer.Resolve<IPinkoConfiguration>();
                };


            // Register pinko services
            // Setup Web Role
            pinkoContainer.Resolve<IWebRoleConnectManager>().InitializeWebRole();

            // Register SignalR
            pinkoContainer.RegisterInstance<IWebRoleSignalRManager>(pinkoContainer.Resolve<WebRoleSignalRManager>().Initialize());

            pinkoContainer.RegisterInstance(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpressionResult>>().Register());


            // Register incoming listeners via reactive extensions
            //pinkoContainer.Resolve<IWebRoleConnectManager>().BusListenerHandlers.Add(pinkoContainer.Resolve<WebRoleBusListenerCalculateExpressionResultHandler>().Register());
            pinkoContainer.RegisterInstance(pinkoContainer.Resolve<WebRoleBusListenerCalculateExpressionResultHandler>().Register());

            //// Start listening to incoming calculation responses
            //pinkoContainer
            //    .Resolve<IPinkoApplication>()
            //    .RunInWorkerThread("PinkoMessageBusToWebRoleCalcResultTopic",
            //        () =>
            //        pinkoContainer
            //            .Resolve<IBusMessageServer>()
            //            .GetTopic(pinkoContainer.Resolve<IPinkoConfiguration>().PinkoMessageBusToWebRoleCalcResultTopic)
            //            .Listen()
            //    );

        }
    }
}