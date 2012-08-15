using System.Diagnostics;
using Microsoft.Practices.Unity;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.InMemoryBus;
using PinkoCommon.Interface;

namespace PinkoCommon.IoC
{
    /// <summary>
    /// Unity container
    /// </summary>
    public class CommonContainerManager
    {
        /// <summary>
        /// Build 
        /// </summary>
        static public IUnityContainer BuildContainer()
        {
            var pinkoContainer = new UnityContainer();

            Trace.Listeners.Add(new TraceListenerDebug());

            pinkoContainer.RegisterInstance<IPinkoConfiguration>(pinkoContainer.Resolve<PinkoConfiguration>());
            pinkoContainer.RegisterInstance<IPinkoApplication>(pinkoContainer.Resolve<PinkoApplication>());
            //pinkoContainer.RegisterInstance<ICloudConfigurationManager>(pinkoContainer.Resolve<AzureCloudConfigurationManager>());

            pinkoContainer.RegisterInstance<IMessageHandlerManager>(pinkoContainer.Resolve<MessageHandlerManager>().Initialize()); // Handle messages

            //pinkoContainer.RegisterInstance<IBusMessageServer>(pinkoContainer.Resolve<AzureBusMessageServer>());
            pinkoContainer.RegisterInstance<IBusMessageServer>(pinkoContainer.Resolve<InMemoryBusMessageServer>()); // when running offline

            pinkoContainer.Resolve<IBusMessageServer>().Initialize();

            return pinkoContainer;
        }
    }
}
