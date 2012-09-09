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
            WorkerRoleFrame = PinkoContainer.Resolve<IWorkerRoleFrame>();

            //// This worker role handles these messages
            //PinkoContainer.Resolve<IMessageHandlerManager>().AddHandler<PinkoCalculateExpression>();
            //PinkoContainer.Resolve<IMessageHandlerManager>().AddHandler<PinkoCalcSubsAction>();
//koContainer.Resolve<HandleCalculateExpression>().Register()

            //// Register message type to process
            //WorkerRoleFrame.MessageHandlers.Add(PinkoContainer.Resolve<HandleCalculateExpression>().Register());
            //WorkerRoleFrame.MessageHandlers[typeof(PinkoCalcSubsAction).ToString()] = PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, PinkoCalcSubsAction>>();
            //WorkerRoleFrame.MessageHandlers.a[typeof(PinkoCalculateExpression).ToString()] = PinkoContainer.Resolve<InboundTypedPublisher<IBusMessageInbound, PinkoCalculateExpression>>();
            ////WorkerRoleFrame.MessageHandlers.AddRange(new object[]
            //{
            //    PinkoContainer.Resolve<HandlerPinkoPingMessage>().Register(),
            //});

            return rtn;
        }

        /// <summary>
        /// Run
        /// </summary>
        public override void Run()
        {
            PinkoApplication.RunInWorkerThread( "Initialize WorkerRole", () => WorkerRoleFrame.Run());
            base.Run();
        }

        /// <summary>
        /// Stopping worker role
        /// </summary>
        public override void OnStop()
        {
            Trace.TraceInformation("Stopping worker role: {0}");

            WorkerRoleFrame.Stop();

            base.OnStop();
        }

        /// <summary>
        /// WorkerRoleFrame
        /// </summary>
        public IWorkerRoleFrame WorkerRoleFrame { private get; set; }


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
