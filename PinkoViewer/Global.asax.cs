﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon.Interface;
using PinkoWebRoleCommon;
using PinkoWebRoleCommon.Handler;
using PinkoWebRoleCommon.Interface;
using PinkoWebRoleCommon.IoC;

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

            // Register pinko services
            // Setup Web Role
            pinkoContainer.Resolve<IWebRoleConnectManager>().InitializeWebRole();

            // Register SignalR
            pinkoContainer.RegisterInstance<IWebRoleSignalRManager>(pinkoContainer.Resolve<WebRoleSignalRManager>());

            // Configure signalR for this web role
            pinkoContainer.Resolve<IWebRoleSignalRManager>().Initialize();

            // Register incoming listeners via reactive extensions
            pinkoContainer.Resolve<IWebRoleConnectManager>()
                .BusListenerHandlers.Add(PinkoWebRoleContainerManager.Container.Resolve<BusListenerCalculateExpressionResult>().Register());

            ////// Register message handler for this role 
            //pinkoContainer.Resolve<IMessageHandlerManager>().AddHandler<PinkoCalculateExpressionResult>();
        }
    }
}