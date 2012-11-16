using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon.Interface;
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
        static private IUnityContainer BuildContainer()
        {
            var pinkoContainer = CommonContainerManager.BuildContainer();
            
            //// TODO: remove temp to run offline
            //var pinkoContainer = PinkoServiceContainer.BuildContainer();

//            pinkoContainer.RegisterInstance<IWebRoleConnectManager>(pinkoContainer.Resolve<WebRoleConnectManager>().Initialize());
            //pinkoContainer.RegisterInstance<IWebRoleSignalRManager>(pinkoContainer.Resolve<WebRoleSignalRManager>());

            //// TODO: remove temp to run offline. simulate worker role running
            //pinkoContainer.Resolve<IWorkerRoleFrame>().Run();

            //// Register incoming listeners via reactive extensions
            //pinkoContainer.Resolve<IWorkerRoleFrame>().MessageHandlers.Add(pinkoContainer.Resolve<HandleCalculateExpressionResult>().Register());

            //// Register message handler for this role 
            //// TODO: To go into web role 
            //pinkoContainer.Resolve<IMessageHandlerManager>().AddHandler<PinkoCalculateExpressionResult>();

            // Start listening to incoming calculation responses
            pinkoContainer
                .Resolve<IPinkoApplication>()
                .RunInWorkerThread("PinkoMessageBusToWebRoleCalcResultTopic",
                    () =>
                    pinkoContainer
                        .Resolve<IBusMessageServer>()
                        .GetTopic(pinkoContainer.Resolve<IPinkoConfiguration>().PinkoMessageBusToWebRoleCalcResultTopic)
                        .Listen()
                );


            return pinkoContainer;
        }
    }
}
