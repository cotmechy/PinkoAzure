using System.Diagnostics;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.ExceptionTypes;
using PinkoCommon.Interface;
using PinkoCommon.Interface.Storage;
using PinkoExpressionCommon;
using PinkoMocks.Hanlders;
using PinkoWebRoleCommon.Interface;

namespace PinkoMocks
{
    /// <summary>
    /// PinkoContainerMock
    /// </summary>
    public class PinkoContainerMock
    {
        public static IUnityContainer GetMockContainer(bool regsiterBusManager = true)
        {
            var pinkoContainer = new UnityContainer();

            Trace.Listeners.Add(new TraceListenerDebug());

            //var mockRepo = new MockRepository();

            //
            //// Rhino IPinkoConfiguration
            ////
            //container.RegisterInstance<IPinkoConfiguration>(mockRepo.StrictMock<IPinkoConfiguration>());
            //var pinkoConfiguration = container.Resolve<IPinkoConfiguration>();
            //pinkoConfiguration.HeartbeatIntervalSec = 1.0;
            pinkoContainer.RegisterInstance<IPinkoConfiguration>(new PinkoConfiguration());
            pinkoContainer.RegisterInstance<IPinkoApplication>(pinkoContainer.Resolve<PinkoApplicationMock>());
            pinkoContainer.RegisterInstance<IRxMemoryBus<IBusMessageOutbound>>(pinkoContainer.Resolve<IPinkoApplication>().GetBus<IBusMessageOutbound>());
            pinkoContainer.RegisterInstance<IRxMemoryBus<PinkoExceptionDataNotSubscribed>>(pinkoContainer.Resolve<IPinkoApplication>().GetBus<PinkoExceptionDataNotSubscribed>());

            pinkoContainer.RegisterInstance<IPinkoExpressionEngine>(new PinkoExpressionEngineMock());
            pinkoContainer.RegisterInstance<IPinkoStorage>(new PinkoStorageMock());
            pinkoContainer.RegisterInstance<IIncominBusMessageHandlerManager>(pinkoContainer.Resolve<IncominBusMessageHandlerManager>().Initialize()); // Handle messages
            pinkoContainer.RegisterInstance<IPinkoMarketEnvironment>(new PinkoMarketEnvironmentMock());
            pinkoContainer.RegisterInstance<IPinkoMarketEnvManager>(pinkoContainer.Resolve<PinkoMarketEnvManagerMock>());
            pinkoContainer.RegisterInstance<IWebRoleSignalRManager>(pinkoContainer.Resolve<WebRoleSignalRManagerMock>());


            if (regsiterBusManager)
            {
                pinkoContainer.RegisterInstance<IBusMessageServer>(pinkoContainer.Resolve<BusMessageServerMock>().Initialize());
                //container.RegisterInstance<IBusMessageServer>(container.Resolve<MsMqBusMessageServer>().Initialize());
                //container.RegisterInstance<IBusMessageServer>(container.Resolve<InMemoryBusMessageServer>().Initialize());
            }
            pinkoContainer.RegisterInstance<IWebRoleConnectManager>(new WebRoleConnectManagerMock());

            //container.RegisterInstance<IBusMessageServer>(container.Resolve<BusMessageServerMock>());
            //container.RegisterInstance<ICloudConfigurationManager>(new CloudConfigurationManagerMock());

            //container.Resolve<IBusMessageServer>().Initialize();


            ////
            //// IPinkoMarketEnvironment / IPinkoDataAccessLayer
            ////
            //var marketEnv = MockRepository.GenerateStub<IPinkoMarketEnvironment>();
            //marketEnv.PinkoDataAccessLayer = new PinkoDataAccessLayerMock();
            //container.RegisterInstance(marketEnv);
            pinkoContainer.RegisterInstance(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpression>>().Register());
            pinkoContainer.RegisterInstance(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgPing>>().Register());
            pinkoContainer.RegisterInstance(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpressionResult>>().Register());
            pinkoContainer.RegisterInstance(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgClientConnect>>().Register());
            pinkoContainer.RegisterInstance(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgClientTimeout>>().Register());
            pinkoContainer.RegisterInstance(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgClientPing>>().Register());
            

            //pinkoContainer.RegisterInstance(pinkoContainer.Resolve<BusListenerCalculateExpressionSnapshotHandler>().Register());
            //pinkoContainer.RegisterInstance(pinkoContainer.Resolve<BusListenerCalculateSubsCalcExpressionHandler>().Register());
            //pinkoContainer.RegisterInstance<InboundMessageHandler<PinkoMsgCalculateExpression>>(pinkoContainer.Resolve<BusListenerUserSubscriberExpressionHandlerMock>().Register());
            //pinkoContainer.RegisterInstance(pinkoContainer.Resolve<BusListenerPinkoPingMessage>().Register());

            pinkoContainer.Resolve<IIncominBusMessageHandlerManager>().AddBusTypeHandler<PinkoMsgClientPing>();
            pinkoContainer.Resolve<IIncominBusMessageHandlerManager>().AddBusTypeHandler<PinkoMsgClientTimeout>();
            pinkoContainer.Resolve<IIncominBusMessageHandlerManager>().AddBusTypeHandler<PinkoMsgCalculateExpression>();
            pinkoContainer.Resolve<IIncominBusMessageHandlerManager>().AddBusTypeHandler<PinkoMsgClientConnect>();
            pinkoContainer.Resolve<IIncominBusMessageHandlerManager>().AddBusTypeHandler<PinkoMsgPing>();
            pinkoContainer.Resolve<IIncominBusMessageHandlerManager>().AddBusTypeHandler<PinkoMsgRoleHeartbeat>();
            pinkoContainer.Resolve<IIncominBusMessageHandlerManager>().AddBusTypeHandler<PinkoMsgCalculateExpressionResult>();



            return pinkoContainer;
        }
    }
}
