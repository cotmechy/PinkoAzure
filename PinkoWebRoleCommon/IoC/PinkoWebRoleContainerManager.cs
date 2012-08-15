using Microsoft.Practices.Unity;
using PinkoCommon.IoC;
using PinkoWebRoleCommon.Interface;

namespace PinkoWebRoleCommon.IoC
{
    public class PinkoWebRoleContainerManager
    {
        /// <summary>
        /// Container
        /// </summary>
        static public IUnityContainer Container = BuildContainer();

        /// <summary>
        /// Build 
        /// </summary>
        static public IUnityContainer BuildContainer()
        {
            var pinkoContainer = CommonContainerManager.BuildContainer();

            pinkoContainer.RegisterInstance<IWebRoleConnectManager>(pinkoContainer.Resolve<WebRoleConnectManager>().Initialize(), new ContainerControlledLifetimeManager());

            return pinkoContainer;
        }
    }
}
