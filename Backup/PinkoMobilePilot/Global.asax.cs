using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Web.Routing;
using PinkDao;
using PinkoCommon;
using PinkoCommon.Interface;
using PinkoWebRoleCommon.IoC;
using PinkoWebRoleCommon.SignalRHub;
using PinkoWebRoleCommon.Utility;
using Microsoft.Practices.Unity;
using SignalR;
using PinkoWebRoleCommon.Extensions;

namespace PinkoMobilePilot
{
    public class Global : System.Web.HttpApplication
    {

        /// <summary>
        /// Register all realtime message handlers to be routed via SiganlR to web clients
        /// </summary>
        public void RegisterRealTimeMessageHandlers()
        {
            var pinkoSingalHubContext = GlobalHost.ConnectionManager.GetHubContext<PinkoSingalHub>();

            PinkoContainer = PinkoWebRoleContainerManager.Container;
            var pinkoApplication = PinkoContainer.Resolve<IPinkoApplication>();

            Trace.TraceInformation("Registering message handlers: {0}", GetType());

            pinkoApplication
                .GetSubscriber<Tuple<IBusMessageInbound, PinkoRoleHeartbeat>>()
                .Do(x => Trace.TraceInformation("WebRole Received: {0}: {1} - {2}", GetType(), x.Item1.Verbose(), x.Item2.ToString()))
                .Subscribe(x => pinkoSingalHubContext.Clients.notifyClientPinkoRoleHeartbeat(x.Item1.PinkoProperties[PinkoMessagePropTag.MachineName],
                                                                                             x.Item1.PinkoProperties[PinkoMessagePropTag.DateTimeStamp]));
        }

        /// <summary>
        /// Web Role Start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_Start(object sender, EventArgs e)
        {
            Trace.TraceInformation("Starting Web Role: {0}", GetHashCode());
            //// Streaming SignalR
            RouteTable.Routes.MapConnection<PinkoSignalRConnection>("echo", "echo/{*operation}");

            PinkoContainer = PinkoWebRoleContainerManager.Container;
            var pinkoApplication = PinkoContainer.Resolve<IPinkoApplication>();
            var pinkoConfiguration = PinkoContainer.Resolve<IPinkoConfiguration>();

            RegisterRealTimeMessageHandlers();

            // TODO: Temporary
            // timer
            _webRoleHearBeat = Observable.Interval(TimeSpan.FromMilliseconds(1000), Scheduler.ThreadPool);

            //var incomingTopic =
            //    PinkoContainer
            //        .Resolve<IBusMessageServer>()
            //        .GetTopic(pinkoConfiguration.PinkoMessageBusToWebAllRolesTopic);

            // Set listener for outbound messages 
            var incomingTopic =
                pinkoApplication
                    .GetBus<IBusMessageOutbound>();

            _webRoleHearBeat
                .Subscribe(x => incomingTopic.Publish(pinkoApplication.FactorWebEnvelop(string.Empty, WebRoleId, new PinkoRoleHeartbeat()))); 

            //_webRoleHearBeat
            //    .Subscribe(x =>
            //    {
            //        //
            //        // https://github.com/SignalR/SignalR/tree/master/samples/SignalR.Hosting.AspNet.Samples
            //        //

            //        //
            //        // https://github.com/SignalR/SignalR/wiki/SignalR-JS-Client-Hubs  - Server to client
            //        //


            //        //
            //        // http://stackoverflow.com/questions/7549179/signalr-posting-a-message-to-a-hub-via-an-action-method
            //        //
            //        ////var context = GlobalHost.ConnectionManager.GetHubContext<PinkoSingalHub>();

            //        var beat = new PinkoRoleHeartbeatHubDao
            //        {
            //            ResponderDateTime = DateTime.Now,
            //            ResponderMachine = machineName
            //        };

            //        //pinkoSingalHubContext.Clients.addMessage(beat);
            //        //context.Clients.NotifyClientPinkoRoleHeartbeat("beat");

            //        //context.Connection.Broadcast(DateTime.Now.ToString());
            //        Debug.WriteLine(string.Format("Sending hearbeat to cleints: {0}", beat.Verbose()));
            //        pinkoSingalHubContext.Clients.notifyClientPinkoRoleHeartbeat(beat.ResponderDateTime, beat.ResponderMachine);


            //    });



            //var  IMessageHandlerManager

            ////Trace.Listeners.Add()

            //// timer
            //_webRoleHearBeat = Observable.Interval(TimeSpan.FromMilliseconds(1000), Scheduler.ThreadPool);

            ////var context = GlobalHost.ConnectionManager.GetHubContext<PinkoSingalHub>();
            ////var context = GlobalHost.ConnectionManager.GetConnectionContext<Str>();
            //var pinkoSingalHubContext = GlobalHost.ConnectionManager.GetHubContext<PinkoSingalHub>();

            //_webRoleHearBeat
            //    .Subscribe(x =>
            //    {
            //        //
            //        // https://github.com/SignalR/SignalR/tree/master/samples/SignalR.Hosting.AspNet.Samples
            //        //

            //        //
            //        // https://github.com/SignalR/SignalR/wiki/SignalR-JS-Client-Hubs  - Server to client
            //        //


            //        //
            //        // http://stackoverflow.com/questions/7549179/signalr-posting-a-message-to-a-hub-via-an-action-method
            //        //
            //        ////var context = GlobalHost.ConnectionManager.GetHubContext<PinkoSingalHub>();

            //        var beat = new PinkoRoleHeartbeatHub
            //        {
            //            ResponderDateTime = DateTime.Now,
            //            ResponderMachine = machineName
            //        };

            //        //pinkoSingalHubContext.Clients.addMessage(beat);
            //        //context.Clients.NotifyClientPinkoRoleHeartbeat("beat");

            //        //context.Connection.Broadcast(DateTime.Now.ToString());
            //        Debug.WriteLine(string.Format("Sending hearbeat to cleints: {0}", beat.Verbose()));
            //        pinkoSingalHubContext.Clients.notifyClientPinkoRoleHeartbeat(beat.ResponderDateTime, beat.ResponderMachine);


            //        //var clients = Hub.GetClients<NewsFeedHub>();
            //        //clients.MethodOnTheJavascript("Good news!"); //dynamic method called on the client


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
            Trace.TraceInformation("Stopping Web Role: {0}", GetHashCode());
            //  Code that runs on application shutdown
            PinkoContainer = PinkoWebRoleContainerManager.Container;
            PinkoContainer.Resolve<IPinkoApplication>().ApplicationRunningEvent.Set();
            Thread.Sleep(3000);
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            Trace.TraceError("Application_Error Web Role: {0}", GetHashCode());

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
            Trace.TraceError("Session_Start Web Role: {0} - e: {1}", GetHashCode(), e.ToString());

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.
            Trace.TraceError("Session_End Web Role: {0} - e: {1}", GetHashCode(), e.ToString());

        }

        /// <summary>
        /// Hearbeat Reactive extension - processes via SignalR
        /// </summary>
        private IObservable<long> _webRoleHearBeat;

        /// <summary>
        /// Uniwue web role id
        /// </summary>
        public readonly string WebRoleId = Guid.NewGuid().ToString();

        /// <summary>
        /// PinkoContainer
        /// </summary>
        public IUnityContainer PinkoContainer { get; set; }
    }
}
