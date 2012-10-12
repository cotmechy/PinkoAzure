using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon.Interface;
using PinkoCommon.IoC;
using PinkoExpressionCommon;
using PinkoExpressionEngine;
using PinkoWorkerCommon.Handler;
using PinkoWorkerCommon.Interface;

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
            //pinkoContainer.RegisterInstance<IWorkerRoleFrame>(pinkoContainer.Resolve<WorkerRoleFrame>());
            //pinkoContainer.RegisterInstance<IPinkoMarketEnvManager>(pinkoContainer.Resolve<PinkoMarketEnvManager>());
            //pinkoContainer.RegisterInstance<IPinkoExpressionEngine>(PinkoExpressionEngineFactory.GetNewEngine());

            //pinkoContainer.RegisterInstance<IPinkoDataFeedIdentifierManager>(pinkoContainer.Resolve<PinkoDataFeedIdentifierManager>());

            //pinkoContainer.RegisterInstance<IPinkoFormulaSubscriberManager>(pinkoContainer.Resolve<PinkoFormulaSubscriberManager>());

            //// Register message type to process
            //pinkoContainer.Resolve<IWorkerRoleFrame>().MessageReceiveHandlers.Add(pinkoContainer.Resolve<BusListenerCalculateExpressionSnapshotHandler>().Register()); // This could go in the worker Role project
            //pinkoContainer.Resolve<IWorkerRoleFrame>().MessageReceiveHandlers.Add(pinkoContainer.Resolve<BusListenerSubscribeExpressionHandler>().Register()); // This could go in the worker Role project

            //// This worker role handles these messages 
            //pinkoContainer.Resolve<IMessageHandlerManager>().AddBusTypeHandler<PinkoMsgCalculateExpression>();

            return pinkoContainer;
        }

        /// <summary>
        /// Specific items for CalcEngine
        /// </summary>
        static public void RegisterCalcEngineExtra(IUnityContainer pinkoContainer)
        {
            pinkoContainer.RegisterInstance<IPinkoExpressionEngine>(PinkoExpressionEngineFactory.GetNewEngine());

            // Register message type to process types
            pinkoContainer.Resolve<IWorkerRoleFrame>().MessageReceiveHandlers.Add(pinkoContainer.Resolve<BusListenerCalculateExpressionSnapshotHandler>().Register()); // This could go in the worker Role project

            // This worker role handles these messages 
            pinkoContainer.Resolve<IMessageHandlerManager>().AddBusTypeHandler<PinkoMsgCalculateExpression>();
        }

        /// <summary>
        /// Specific items for SubscriptionManagerWorker
        /// </summary>
        static public void RegisterSubscriptionManagerWorkerExtra(IUnityContainer pinkoContainer)
        {
            //pinkoContainer.RegisterInstance<IPinkoFormulaSubscriberManager>(pinkoContainer.Resolve<PinkoFormulaSubscriberManager>());

            // Register message type to process
            pinkoContainer.Resolve<IWorkerRoleFrame>().MessageReceiveHandlers.Add(pinkoContainer.Resolve<BusListenerUserSubscriberExpressionHandler>().Register()); // This could go in the worker Role project

            // This worker role handles these messages types
            pinkoContainer.Resolve<IMessageHandlerManager>().AddBusTypeHandler<PinkoMsgCalculateExpression>();
        }
    }
}





