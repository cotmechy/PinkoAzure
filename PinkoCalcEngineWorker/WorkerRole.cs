using System.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using PinkDao;
using Microsoft.Practices.Unity;
using PinkoCommon.Interface;
using PinkoWorkerCommon.Interface;
using PinkoWorkerCommon.Utility;

namespace PinkoCalcEngineWorker
{
    /// <summary>
    /// WorkerRole
    /// </summary>
    public class WorkerRole : RoleEntryPoint
    {
        /// <summary>
        /// Worker Role OnStart()
        /// </summary>
        /// <returns></returns>
        public override bool OnStart()
        {
            var rtn = base.OnStart();

            PinkoContainer = PinkoServiceContainer.BuildContainer();  // Real Container
            
            PinkoApplication = PinkoContainer.Resolve<IPinkoApplication>();
            PinkoConfiguration = PinkoContainer.Resolve<IPinkoConfiguration>();
            WorkerRoleFrame = PinkoContainer.Resolve<IWorkerRoleFrame>();

            PinkoServiceContainer.RegisterCalcEngineExtra(PinkoContainer);

            return rtn;
        }

        /// <summary>
        /// Run
        /// </summary>
        public override void Run()
        {
            // start listening to Topic messages
            PinkoApplication.RunInWorkerThread("Initialize PinkoCalcEngineWorker",
                                               () =>
                                               WorkerRoleFrame.Run(
                                                   PinkoConfiguration.PinkoMessageBusToWorkerAllCalcEngineTopic,
                                                   PinkoConfiguration.PinkoMessageBusToWorkerCalcEngineTopic)
                );


            base.Run();
        }

        /// <summary>
        /// Stopping worker role
        /// </summary>
        public override void OnStop()
        {
            Trace.TraceInformation("Stopping worker role PinkoCalcEngineWorker");

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
