using System.Diagnostics;
using Microsoft.Practices.Unity;
using PinkoCommon;
using PinkoCommon.InMemoryBus;
using PinkoCommon.Interface;

namespace PinkoServices.IoC
{
    ///// <summary>
    ///// Manags container. Eventually inject properly into callers
    ///// </summary>
    //public class ContainerManager
    //{
    //    /// <summary>
    //    /// Build 
    //    /// </summary>
    //    static public IUnityContainer BuildContainer()
    //    {
    //        var pinkoContainer = new UnityContainer();

    //        Trace.Listeners.Add(new TraceListenerDebug());

    //        pinkoContainer.RegisterInstance<IPinkoConfiguration>(pinkoContainer.Resolve<PinkoConfiguration>());
    //        pinkoContainer.RegisterInstance<IPinkoApplication>(pinkoContainer.Resolve<PinkoApplication>());
    //        //pinkoContainer.RegisterInstance<ICloudConfigurationManager>(pinkoContainer.Resolve<AzureCloudConfigurationManager>());

    //        //pinkoContainer.RegisterInstance<IBusMessageServer>(pinkoContainer.Resolve<AzureBusMessageServer>());
    //        pinkoContainer.RegisterInstance<IBusMessageServer>(pinkoContainer.Resolve<InMemoryBusMessageServer>()); // Whe runnign offline

    //        pinkoContainer.Resolve<IBusMessageServer>().Initialize();

    //        return pinkoContainer;
    //    }

    //}
}
