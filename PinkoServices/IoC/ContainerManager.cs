using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using PinkoAzureService.AzureMessageBus;
using PinkoWorkerCommon.InMemoryMessageBus;
using PinkoWorkerCommon.Interface;
using PinkoWorkerCommon.Utility;

namespace PinkoServices.IoC
{
    /// <summary>
    /// Manags container. Eventually inject properly into callers
    /// </summary>
    public class ContainerManager
    {
        /// <summary>
        /// Build 
        /// </summary>
        static public IUnityContainer BuildContainer()
        {
            var pinkoContainer = new UnityContainer();

            pinkoContainer.RegisterInstance<IPinkoConfiguration>(pinkoContainer.Resolve<PinkoConfiguration>());
            pinkoContainer.RegisterInstance<IPinkoApplication>(pinkoContainer.Resolve<PinkoApplication>());
            //pinkoContainer.RegisterInstance<ICloudConfigurationManager>(pinkoContainer.Resolve<AzureCloudConfigurationManager>());

            pinkoContainer.RegisterInstance<IBusMessageServer>(pinkoContainer.Resolve<AzureBusMessageServer>());
            //pinkoContainer.RegisterInstance<IBusMessageServer>(pinkoContainer.Resolve<InMemoryBusMessageServer>()); // Whe runnign offline

            pinkoContainer.Resolve<IBusMessageServer>().Initialize();

            return pinkoContainer;
        }

    }
}
