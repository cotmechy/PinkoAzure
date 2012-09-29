using System.Configuration;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using PinkoCommon.Interface;
using Microsoft.Practices.Unity.Configuration;

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

            //pinkoContainer.RegisterInstance<IPinkoConfiguration>(pinkoContainer.Resolve<PinkoConfiguration>());
            //pinkoContainer.RegisterInstance<IPinkoApplication>(pinkoContainer.Resolve<PinkoApplication>());
            ////pinkoContainer.RegisterInstance<ICloudConfigurationManager>(pinkoContainer.Resolve<AzureCloudConfigurationManager>());

            //pinkoContainer.RegisterInstance<IMessageHandlerManager>(pinkoContainer.Resolve<MessageHandlerManager>().Initialize()); // Handle messages

            //pinkoContainer.RegisterInstance<IBusMessageServer>(pinkoContainer.Resolve<AzureBusMessageServer>());
            ////pinkoContainer.RegisterInstance<IBusMessageServer>(pinkoContainer.Resolve<InMemoryBusMessageServer>().Initialize()); // when running offline
            //pinkoContainer.RegisterInstance<IBusMessageServer>(pinkoContainer.Resolve<MsMqBusMessageServer>().Initialize()); // when running offline

            if (ConfigurationManager.GetSection("unity") != null)
                pinkoContainer.LoadConfiguration();

//            Microsoft.Practices.Unity.Configuration.UnityConfigurationSection

                        
            //// Load unity config file
            //var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = "PinkoCommon.dll.config" };
            //var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            //var unitySection = (UnityConfigurationSection)configuration.GetSection("unity");
            //pinkoContainer.LoadConfiguration(unitySection);

            pinkoContainer.RegisterInstance<IRxMemoryBus<IBusMessageOutbound>>(pinkoContainer.Resolve<IPinkoApplication>().GetBus<IBusMessageOutbound>());

            return pinkoContainer;
        }
    }
}
