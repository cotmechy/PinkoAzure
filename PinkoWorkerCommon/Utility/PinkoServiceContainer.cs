using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon.Interface;
using PinkoCommon.IoC;
using PinkoExpressionCommon;
using PinkoExpressionEngine;
using PinkoWorkerCommon.Handler;
using PinkoWorkerCommon.Interface;
using PinkoWorkerCommon.Providers;

namespace PinkoWorkerCommon.Utility
{
    public class PinkoServiceContainer
    {
        /// <summary>
        /// Build 
        /// </summary>
        static public IUnityContainer BuildContainer()
        {
            var pinkoContainer = CommonContainerManager.BuildContainer();

            //pinkoContainer.RegisterInstance<IWorkerRoleHeartBeat>(pinkoContainer.Resolve<WorkerRoleHeartBeat>().Initialize());
            pinkoContainer.RegisterInstance<IWorkerRoleFrame>(pinkoContainer.Resolve<WorkerRoleFrame>());
            pinkoContainer.RegisterInstance<IPinkoMarketEnvManager>(pinkoContainer.Resolve<PinkoMarketEnvManager>());
            pinkoContainer.RegisterInstance<IPinkoExpressionEngine>(PinkoExpressionEngineFactory.GetNewEngine());

            // Register message type to process
            pinkoContainer.Resolve<IWorkerRoleFrame>().MessageReceiveHandlers.Add(pinkoContainer.Resolve<BusListenerCalculateExpressionSnapshot>().Register()); // This could go in the worker Role project

            // This worker role handles these messages 
            pinkoContainer.Resolve<IMessageHandlerManager>().AddBusTypeHandler<PinkoMsgCalculateExpression>();
            //pinkoContainer.Resolve<IMessageHandlerManager>().AddHandler<PinkoCalcSubsAction>();

            return pinkoContainer;
        }
    }
}
