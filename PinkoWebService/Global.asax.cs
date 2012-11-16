using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon.Interface;
using PinkoWebRoleCommon.Interface;
using PinkoWebRoleCommon.IoC;
using PinkoWebService.Controllers;

namespace PinkoWebService
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var pinkoContainer = PinkoWebRoleContainerManager.Container;

            // Register pinko services
            // Setup Web Role
            _webRoleConnectManager = pinkoContainer
                                        .Resolve<IWebRoleConnectManager>()
                                        .InitializeWebRole();

            // This service will be dealing with this message type
            pinkoContainer.Resolve<IIncominBusMessageHandlerManager>().AddBusTypeHandler<PinkoMsgCalculateExpression>();
   
            // Register service controllers for this web service
            PinkoWebRoleContainerManager.Container.RegisterType<PinkoFormProcessorSubscriberController>();
            PinkoWebRoleContainerManager.Container.RegisterType<PinkoFormProcessorController>();
            PinkoWebRoleContainerManager.Container.RegisterType<PinkoDictionaryController>();
            PinkoWebRoleContainerManager.Container.RegisterType<PinkoClientPingController>();
        }

        /// <summary>
        /// Pinko services
        /// </summary>
        private IWebRoleConnectManager _webRoleConnectManager;
    }
}