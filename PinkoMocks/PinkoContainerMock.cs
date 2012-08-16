using System.Diagnostics;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.InMemoryBus;
using PinkoCommon.Interface;
using PinkoExpressionCommon;
using Rhino.Mocks;

namespace PinkoMocks
{
    /// <summary>
    /// PinkoContainerMock
    /// </summary>
    public class PinkoContainerMock
    {
        public static IUnityContainer GetMokContainer()
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
            container.RegisterInstance<IPinkoMarketEnvManager>(new PinkoMarketEnvManagerMock() { PinkoMarketEnv = container.Resolve<IPinkoMarketEnvironment>() });
            
            container.RegisterInstance<IBusMessageServer>(container.Resolve<InMemoryBusMessageServer>());
            //container.RegisterInstance<IBusMessageServer>(container.Resolve<BusMessageServerMock>());
            //container.RegisterInstance<ICloudConfigurationManager>(new CloudConfigurationManagerMock());

            //container.Resolve<IBusMessageServer>().Initialize();


            ////
            //// IPinkoMarketEnvironment / IPinkoDataAccessLayer
            ////
            //var marketEnv = MockRepository.GenerateStub<IPinkoMarketEnvironment>();
            //marketEnv.PinkoDataAccessLayer = new PinkoDataAccessLayerMock();
            //container.RegisterInstance(marketEnv);



            return container;
        }
    }
}
