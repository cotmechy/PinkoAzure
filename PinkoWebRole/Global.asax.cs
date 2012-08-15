using System;
using System.Web.Mvc;
using System.Web.Routing;
using PinkoWebRoleCommon.IoC;

namespace PinkoWebRole
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    // SignalR: https://github.com/SignalR/SignalR/wiki/QuickStart-Persistent-Connections

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            // https://github.com/SignalR/SignalR/wiki/Hubs
            //routes.MapHubs("~/echo");

            //// Streaming SignalR
            //routes.MapConnection<PinkoSignalRConnection>("echo", "echo/{*operation}");

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        /// <summary>
        /// Application_Start
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            var machineName = Environment.MachineName;



            //// timer
            //_observable = Observable.Interval(TimeSpan.FromMilliseconds(1000), Scheduler.ThreadPool);

            //_observable
            //    .Subscribe(x =>
            //    {
            //        //
            //        // http://stackoverflow.com/questions/7549179/signalr-posting-a-message-to-a-hub-via-an-action-method
            //        //
            //        var context = GlobalHost.ConnectionManager.GetHubContext<PinkoSingalHub>();

            //        var beat = new PinkoRoleHeartbeatHub
            //                       {
            //                           ResponderDateTime = DateTime.Now,
            //                           ResponderMachine = machineName
            //                       };

            //        //context.Clients.addMessage(beat);
            //        context.Clients.NotifyClientPinkoRoleHeartbeat(beat);


            //        //context.Clients.addMessage(beat);
            //        //context.Clients[group].methodInJavascript("hello world");

            //        ////var clients = Hub.GetClients<NewsFeedHub>();
            //        ////clients.MethodOnTheJavascript("Good news!"); //dynamic method called on the client

            //        //// Important: .Resolve is an extension method inside SignalR.Infrastructure namespace.
            //        //this.
            //        //var connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
            //        //var clients = connectionManager.GetClients<MyHub>();

            //        //// Broadcast to all clients.
            //        //clients.MethodOnTheJavascript("Good news!");

            //        //// Broadcast only to clients in a group.
            //        //clients["someGroupName"].MethodOnTheJavascript("Hello, some group!");

            //        //// Broadcast only to a particular client.
            //        //clients["someConnectionId"].MethodOnTheJavascript("Hello, particular client!");

            //        //
            //        // http://stackoverflow.com/questions/9942591/iis-background-thread-and-signalr
            //        //
            //        //var connectionManager = RouteTable.Routes[]
            //        //var demoClients = connectionManager.GetClients<MyHubDerivedClass>();
            //        //demoClients.TellUsers(msg);

            //        //var connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
            //        //var demoClients = connectionManager.GetClients<MyHubDerivedClass>();
            //        //demoClients.TellUsers(msg);

            //        //var hub = new PinkoSingalHub();
            //        //hub.NotifyClientPinkoRoleHeartbeat(
            //        //    new PinkoRoleHeartbeat
            //        //    {
            //        //        ResponderDateTime = DateTime.Now,
            //        //        ResponderMachine = "MachineName"
            //        //    });

            //    });
        }

        private IObservable<long> _observable;

    }
}