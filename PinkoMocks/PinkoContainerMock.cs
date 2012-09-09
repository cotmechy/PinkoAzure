using System.Diagnostics;
using Microsoft.Practices.Unity;
using PinkoCommon;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.Interface;
using PinkoExpressionCommon;
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
            var container = new UnityContainer();

            Trace.Listeners.Add(new TraceListenerDebug());

            //var mockRepo = new MockRepository();

            //
            //// Rhino IPinkoConfiguration
            ////
            //container.RegisterInstance<IPinkoConfiguration>(mockRepo.StrictMock<IPinkoConfiguration>());
            //var pinkoConfiguration = container.Resolve<IPinkoConfiguration>();
            //pinkoConfiguration.HeartbeatIntervalSec = 1.0;

            container.RegisterInstance<IPinkoConfiguration>(new PinkoConfiguration());
            container.RegisterInstance<IPinkoApplication>(container.Resolve<PinkoApplicationMock>());
            container.RegisterInstance<IPinkoExpressionEngine>(new PinkoExpressionEngineMock());
            container.RegisterInstance<IMessageHandlerManager>(container.Resolve<MessageHandlerManager>().Initialize()); // Handle messages

            container.RegisterInstance<IPinkoMarketEnvironment>(new PinkoMarketEnvironmentMock());
            container.RegisterInstance<IPinkoMarketEnvManager>(container.Resolve<PinkoMarketEnvManagerMock>());

            if (regsiterBusManager)
            {
                container.RegisterInstance<IBusMessageServer>(container.Resolve<BusMessageServerMock>().Initialize());
                //container.RegisterInstance<IBusMessageServer>(container.Resolve<MsMqBusMessageServer>().Initialize());
                //container.RegisterInstance<IBusMessageServer>(container.Resolve<InMemoryBusMessageServer>().Initialize());
            }
            container.RegisterInstance<IWebRoleConnectManager>(new WebRoleConnectManagerMock());

            //container.RegisterInstance<IBusMessageServer>(container.Resolve<BusMessageServerMock>());
            //container.RegisterInstance<ICloudConfigurationManager>(new CloudConfigurationManagerMock());

            //container.Resolve<IBusMessageServer>().Initialize();


            ////
            //// IPinkoMarketEnvironment / IPinkoDataAccessLayer
            ////
            //var marketEnv = MockRepository.GenerateStub<IPinkoMarketEnvironment>();
            //marketEnv.PinkoDataAccessLayer = new PinkoDataAccessLayerMock();
            //container.RegisterInstance(marketEnv);

            container.RegisterInstance<IRxMemoryBus<IBusMessageOutbound>>(container.Resolve<IPinkoApplication>().GetBus<IBusMessageOutbound>());


            return container;
        }
    }
}
