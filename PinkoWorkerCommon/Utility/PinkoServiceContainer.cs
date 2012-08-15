using Microsoft.Practices.Unity;
using PinkoCommon.IoC;

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

            return pinkoContainer;
        }
    }
}
