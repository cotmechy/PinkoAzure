using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Web.Routing;
using PinkDao;
using PinkoWebRoleCommon.HubModels;
using PinkoWebRoleCommon.SignalRHub;
using PinkoWebRoleCommon.Utility;
using SignalR;

namespace PinkoMobilePilot
{
    public class Global : System.Web.HttpApplication
    {
        /// <summary>
        /// Web Role Start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_Start(object sender, EventArgs e)
        {
            //// Streaming SignalR
            RouteTable.Routes.MapConnection<PinkoSignalRConnection>("echo", "echo/{*operation}");

            var machineName = Environment.MachineName;

            // timer
            _webRoleHearBeat = Observable.Interval(TimeSpan.FromMilliseconds(1000), Scheduler.ThreadPool);

            //var context = GlobalHost.ConnectionManager.GetHubContext<PinkoSingalHub>();
            //var context = GlobalHost.ConnectionManager.GetConnectionContext<Str>();
            var pinkoSingalHubContext = GlobalHost.ConnectionManager.GetHubContext<PinkoSingalHub>();

            _webRoleHearBeat
                .Subscribe(x =>
                {
                    //
                    // https://github.com/SignalR/SignalR/tree/master/samples/SignalR.Hosting.AspNet.Samples
                    //
                    
                    //
                    // https://github.com/SignalR/SignalR/wiki/SignalR-JS-Client-Hubs  - Server to client
                    //


                    //
                    // http://stackoverflow.com/questions/7549179/signalr-posting-a-message-to-a-hub-via-an-action-method
                    //
                    ////var context = GlobalHost.ConnectionManager.GetHubContext<PinkoSingalHub>();

                    var beat = new PinkoRoleHeartbeatHub
                    {
                        ResponderDateTime = DateTime.Now,
                        ResponderMachine = machineName
                    };

                    //pinkoSingalHubContext.Clients.addMessage(beat);
                    //context.Clients.NotifyClientPinkoRoleHeartbeat("beat");

                    //context.Connection.Broadcast(DateTime.Now.ToString());
                    Debug.WriteLine(string.Format("Sending hearbeat to cleints: {0}", beat.Verbose()));
                    pinkoSingalHubContext.Clients.notifyClientPinkoRoleHeartbeat(beat.ResponderDateTime, beat.ResponderMachine);


                    //var clients = Hub.GetClients<NewsFeedHub>();
                    //clients.MethodOnTheJavascript("Good news!"); //dynamic method called on the client


                    //context.Clients.addMessage(beat);
                    //context.Clients[group].methodInJavascript("hello world");

                    ////var clients = Hub.GetClients<NewsFeedHub>();
                    ////clients.MethodOnTheJavascript("Good news!"); //dynamic method called on the client

                    //// Important: .Resolve is an extension method inside SignalR.Infrastructure namespace.
                    //this.
                    //var connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
                    //var clients = connectionManager.GetClients<MyHub>();

                    //// Broadcast to all clients.
                    //clients.MethodOnTheJavascript("Good news!");

                    //// Broadcast only to clients in a group.
                    //clients["someGroupName"].MethodOnTheJavascript("Hello, some group!");

                    //// Broadcast only to a particular client.
                    //clients["someConnectionId"].MethodOnTheJavascript("Hello, particular client!");

                    //
                    // http://stackoverflow.com/questions/9942591/iis-background-thread-and-signalr
                    //
                    //var connectionManager = RouteTable.Routes[]
                    //var demoClients = connectionManager.GetClients<MyHubDerivedClass>();
                    //demoClients.TellUsers(msg);

                    //var connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
                    //var demoClients = connectionManager.GetClients<MyHubDerivedClass>();
                    //demoClients.TellUsers(msg);

                    //var hub = new PinkoSingalHub();
                    //hub.NotifyClientPinkoRoleHeartbeat(
                    //    new PinkoRoleHeartbeat
                    //    {
                    //        ResponderDateTime = DateTime.Now,
                    //        ResponderMachine = "MachineName"
                    //    });

                });

        //protected void Application_Start(object sender, EventArgs e)
        //{
        //    ThreadPool.QueueUserWorkItem(_ =>
        //    {
        //        var context = GlobalHost.ConnectionManager.GetConnectionContext<Streaming>();
        //        var hubContext = GlobalHost.ConnectionManager.GetHubContext<DemoHub>();

        //        while (true)
        //        {
        //            try
        //            {
        //                context.Connection.Broadcast(DateTime.Now.ToString());
        //                hubContext.Clients.fromArbitraryCode(DateTime.Now.ToString());
        //            }
        //            catch (Exception ex)
        //            {
        //                Trace.TraceError("SignalR error thrown in Streaming broadcast: {0}", ex);
        //            }
        //            Thread.Sleep(2000);
        //        }
        //    });


        //    RouteTable.Routes.MapConnection<SendingConnection>("sending-connection", "sending-connection/{*operation}");
        //    RouteTable.Routes.MapConnection<TestConnection>("test-connection", "test-connection/{*operation}");
        //    RouteTable.Routes.MapConnection<Raw>("raw", "raw/{*operation}");
        //    RouteTable.Routes.MapConnection<Streaming>("streaming", "streaming/{*operation}");

        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

        /// <summary>
        /// Hearbeat Reactive extension - processes via SignalR
        /// </summary>
        private IObservable<long> _webRoleHearBeat;

    }
}
