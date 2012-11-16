using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.ExceptionTypes;
using PinkoCommon.Interface;
using PinkoCommon.IoC;
using PinkoExpressionCommon;
using PinkoExpressionEngine;
using PinkoWorkerCommon.Interface;
using PinkoWorkerCommon.RoleHandlers;

namespace PinkoWorkerCommon.Utility
{
    public class PinkoServiceContainer
    {
        /// <summary>
        /// Build 
        /// </summary>
        public IUnityContainer BuildContainer()
        {
            //
            // Add to unit test: PinkoServiceContainerTests
            //

            var pinkoContainer = CommonContainerManager.BuildContainer();

            //pinkoContainer.RegisterInstance<IWorkerRoleHeartBeat>(pinkoContainer.Resolve<WorkerRoleHeartBeat>().Initialize());
            //pinkoContainer.RegisterInstance<IWorkerRoleFrame>(pinkoContainer.Resolve<WorkerRoleFrame>());
            //pinkoContainer.RegisterInstance<IPinkoMarketEnvManager>(pinkoContainer.Resolve<PinkoMarketEnvManager>());
            //pinkoContainer.RegisterInstance<IPinkoExpressionEngine>(PinkoExpressionEngineFactory.GetNewEngine());

            //pinkoContainer.RegisterInstance<IPinkoDataFeedIdentifierManager>(pinkoContainer.Resolve<PinkoDataFeedIdentifierManager>());

            //pinkoContainer.RegisterInstance<IPinkoFormulaSubscriberManager>(pinkoContainer.Resolve<PinkoFormulaSubscriberManager>());

            //// Register message type to process
            //pinkoContainer.Resolve<IWorkerRoleFrame>().MessageReceiveHandlers.Add(pinkoContainer.Resolve<BusListenerCalculateExpressionSnapshotHandler>().Register()); 
            //pinkoContainer.Resolve<IWorkerRoleFrame>().MessageReceiveHandlers.Add(pinkoContainer.Resolve<BusListenerSubscribeExpressionHandler>().Register()); 

            //// This worker role handles these messages 
            //pinkoContainer.Resolve<IMessageHandlerManager>().AddBusTypeHandler<PinkoMsgCalculateExpression>();

            return pinkoContainer;
        }

        /// <summary>
        /// Specific items for CalcEngine
        /// </summary>
        public void RegisterCalcEngineExtra(IUnityContainer pinkoContainer)
        {
            //
            // Add to unit test: PinkoServiceContainerTests
            //

            // Register calculation engine
            pinkoContainer.RegisterInstance<IPinkoExpressionEngine>(PinkoExpressionEngineFactory.GetNewEngine());

            // Register 
            pinkoContainer.RegisterInstance<IRxMemoryBus<PinkoExceptionDataNotSubscribed>>(pinkoContainer.Resolve<IPinkoApplication>().GetBus<PinkoExceptionDataNotSubscribed>());

            // Register message type to process types 

            //pinkoContainer.RegisterInstance(pinkoContainer.Resolve<BusListenerCalculateExpressionSnapshotHandler>().Register());
            //pinkoContainer.RegisterInstance(pinkoContainer.Resolve<BusListenerCalculateSubsCalcExpressionHandler>().Register());
            pinkoContainer.RegisterInstance(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpression>>().Register());
            pinkoContainer.RegisterInstance(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgClientConnect>>().Register());
            pinkoContainer.RegisterInstance(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgPing>>().Register());
            pinkoContainer.RegisterInstance(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgClientTimeout>>().Register());
            pinkoContainer.RegisterInstance(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgClientTimeout>>().Register());
            pinkoContainer.RegisterInstance(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgClientPing>>().Register());

            pinkoContainer.RegisterInstance(pinkoContainer.Resolve<RoleBusListenerPinkoPingMessageHandle>().Register());
            pinkoContainer.RegisterInstance(pinkoContainer.Resolve<RoleUserSubscriptionHandler>().Register());
            pinkoContainer.RegisterInstance(pinkoContainer.Resolve<RoleCalculateExpressionSnapshotHandler>().Register());
            pinkoContainer.RegisterInstance(pinkoContainer.Resolve<RoleCalculateSubsCalcExpressionHandler>().Register());


//            //
//            // Snapshot 
//            //
//            pinkoContainer.Resolve<IWorkerRoleFrame>().MessageReceiveHandlers.Add(pinkoContainer.Resolve<BusListenerCalculateExpressionSnapshotHandler>().Register());
//            pinkoContainer.Resolve<IWorkerRoleFrame>().MessageReceiveHandlers.Add(pinkoContainer.Resolve<BusListenerCalculateSubsCalcExpressionHandler>().Register());

//            //
//            // Subscription manger
//            //
////            pinkoContainer.RegisterInstance<PinkoDictionary<string, PinkoMsgCalculateExpression>>(new PinkoDictionary<string, PinkoMsgCalculateExpression>());
//            pinkoContainer.Resolve<IWorkerRoleFrame>().MessageReceiveHandlers.Add(pinkoContainer.Resolve<BusListenerUserSubscriberExpressionHandler>().Register());

            // This worker role handles these messages - Registers Rx publisher that listens to message bus
            pinkoContainer.Resolve<IIncominBusMessageHandlerManager>().AddBusTypeHandler<PinkoMsgCalculateExpression>();
            pinkoContainer.Resolve<IIncominBusMessageHandlerManager>().AddBusTypeHandler<PinkoMsgClientConnect>();
            pinkoContainer.Resolve<IIncominBusMessageHandlerManager>().AddBusTypeHandler<PinkoMsgPing>();
            //pinkoContainer.Resolve<IIncominBusMessageHandlerManager>().AddBusTypeHandler<string>();
            pinkoContainer.Resolve<IIncominBusMessageHandlerManager>().AddBusTypeHandler<PinkoMsgRoleHeartbeat>();
            pinkoContainer.Resolve<IIncominBusMessageHandlerManager>().AddBusTypeHandler<PinkoMsgCalculateExpressionResult>();
            pinkoContainer.Resolve<IIncominBusMessageHandlerManager>().AddBusTypeHandler<PinkoMsgClientTimeout>();
            pinkoContainer.Resolve<IIncominBusMessageHandlerManager>().AddBusTypeHandler<PinkoMsgClientPing>();

            RegisterTopicListeners(pinkoContainer);
        }


        /// <summary>
        /// Register Listeners
        /// </summary>
        static public void RegisterTopicListeners(IUnityContainer pinkoContainer)
        {
            // register topics we are listening to
            var pinkoApplication = pinkoContainer.Resolve<IPinkoApplication>();
            var pinkoConfiguration = pinkoContainer.Resolve<IPinkoConfiguration>();
            var workerRoleFrame = pinkoContainer.Resolve<IWorkerRoleFrame>();

            pinkoApplication.RunInWorkerThread("Initialize PinkoSubscriptionManagerWorker",
                                               () =>
                                               workerRoleFrame.Run(
                                                   pinkoConfiguration.PinkoMessageBusToWorkerSubscriptionManagerAllTopic,
                                                   pinkoConfiguration.PinkoMessageBusToWorkerSubscriptionManagerTopic)
                );

            // Hook to incoming for ALL worker roles
            pinkoApplication.RunInWorkerThread(pinkoConfiguration.PinkoMessageBusToAllWorkersTopic,
                () =>
                pinkoContainer
                    .Resolve<IBusMessageServer>()
                    .GetTopic(pinkoConfiguration.PinkoMessageBusToAllWorkersTopic)
                    .Listen()
                );

            // start listening to Topic messages
            pinkoApplication.RunInWorkerThread("Initialize PinkoCalcEngineWorker",
                                               () =>
                                               workerRoleFrame.Run(
                                                   pinkoConfiguration.PinkoMessageBusToWorkerCalcEngineAllTopic,
                                                   pinkoConfiguration.PinkoMessageBusToWorkerCalcEngineTopic)
                );

        }

        /// <summary>
        /// Specific items for SubscriptionManagerWorker
        /// </summary>
        public void RegisterSubscriptionManagerWorkerExtra(IUnityContainer pinkoContainer)
        {
            //
            // Add to unit test: PinkoServiceContainerTests
            //

            //pinkoContainer.RegisterInstance<IPinkoFormulaSubscriberManager>(pinkoContainer.Resolve<PinkoFormulaSubscriberManager>());

            //// Register message type to process
            //pinkoContainer.Resolve<IWorkerRoleFrame>().MessageReceiveHandlers.Add(pinkoContainer.Resolve<BusListenerUserSubscriberExpressionHandler>().Register()); 

            //// This worker role handles these messages types
            //pinkoContainer.Resolve<IMessageHandlerManager>().AddBusTypeHandler<PinkoMsgCalculateExpression>();
        }
    }
}





