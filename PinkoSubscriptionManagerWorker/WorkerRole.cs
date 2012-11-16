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
        /// <summary>
        /// Worker Role OnStart()
        /// </summary>
        /// <returns></returns>
        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            var rtn = base.OnStart();
            var pinkoServiceContainer = new PinkoServiceContainer();

            PinkoContainer = pinkoServiceContainer.BuildContainer();  // Real Container

            PinkoApplication = PinkoContainer.Resolve<IPinkoApplication>();
            PinkoConfiguration = PinkoContainer.Resolve<IPinkoConfiguration>();
            WorkerRoleFrame = PinkoContainer.Resolve<IWorkerRoleFrame>();

            pinkoServiceContainer.RegisterSubscriptionManagerWorkerExtra(PinkoContainer);

            return rtn;
        }

        /// <summary>
        /// Run
        /// </summary>
        public override void Run()
        {
            //PinkoApplication.RunInWorkerThread("Initialize PinkoSubscriptionManagerWorker",
            //                                   () =>
            //                                   WorkerRoleFrame.Run(
            //                                       PinkoConfiguration.PinkoMessageBusToWorkerAllSubscriptionManagerTopic,
            //                                       PinkoConfiguration.PinkoMessageBusToWorkerSubscriptionManagerTopic)
            //    );


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
