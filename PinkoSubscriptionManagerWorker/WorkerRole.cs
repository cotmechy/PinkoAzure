using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.Practices.Unity;
using Microsoft.WindowsAzure.ServiceRuntime;
using PinkoCommon.Interface;
using PinkoWorkerCommon.Interface;
using PinkoWorkerCommon.Utility;

namespace PinkoSubscriptionManagerWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        //public override void Run()
        //{
        //    // This is a sample worker implementation. Replace with your logic.
        //    Trace.WriteLine("PinkoSubscriptionManagerWorker entry point called", "Information");

        //    while (true)
        //    {
        //        Thread.Sleep(10000);
        //        Trace.WriteLine("Working", "Information");
        //    }
        //}

        //public override bool OnStart()
        //{
        //    // Set the maximum number of concurrent connections 
        //    ServicePointManager.DefaultConnectionLimit = 12;

        //    // For information on handling configuration changes
        //    // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

        //    return base.OnStart();
        //}

        /// <summary>
        /// Worker Role OnStart()
        /// </summary>
        /// <returns></returns>
        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            var rtn = base.OnStart();

            PinkoContainer = PinkoServiceContainer.BuildContainer();  // Real Container

            PinkoApplication = PinkoContainer.Resolve<IPinkoApplication>();
            PinkoConfiguration = PinkoContainer.Resolve<IPinkoConfiguration>();
            WorkerRoleFrame = PinkoContainer.Resolve<IWorkerRoleFrame>();

            PinkoServiceContainer.RegisterSubscriptionManagerWorkerExtra(PinkoContainer);

            return rtn;
        }

        /// <summary>
        /// Run
        /// </summary>
        public override void Run()
        {
            PinkoApplication.RunInWorkerThread("Initialize PinkoSubscriptionManagerWorker",
                                               () =>
                                               WorkerRoleFrame.Run(
                                                   PinkoConfiguration.PinkoMessageBusToWorkerAllSubscriptionManagerWorker,
                                                   PinkoConfiguration.PinkoMessageBusToWorkerSubscriptionManagerWorker)
                );


            base.Run();
        }

        /// <summary>
        /// Stopping worker role
        /// </summary>
        public override void OnStop()
        {
            Trace.TraceInformation("Stopping worker role PinkoSubscriptionManagerWorker");

            WorkerRoleFrame.Stop();

            base.OnStop();
        }

        /// <summary>
        /// WorkerRoleFrame
        /// </summary>
        public IWorkerRoleFrame WorkerRoleFrame { private get; set; }

        /// <summary>
        /// IPinkoConfiguration
        /// </summary>
        public IPinkoConfiguration PinkoConfiguration { private get; set; }

        /// <summary>
        /// PinkoContainer
        /// </summary>
        public IUnityContainer PinkoContainer { get; set; }

        /// <summary>
        /// IPinkoApplication
        /// </summary>
        public IPinkoApplication PinkoApplication { get; set; }

    }
}
