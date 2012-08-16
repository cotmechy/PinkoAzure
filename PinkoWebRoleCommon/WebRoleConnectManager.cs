using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using PinkoCommon.Interface;
using PinkoWebRoleCommon.Interface;

namespace PinkoWebRoleCommon
{
    /// <summary>
    /// Manages connectivity with messageing bus
    /// </summary>
    public class WebRoleConnectManager : IWebRoleConnectManager
    {
        /// <summary>
        /// Constructor - WebRoleConnectManager 
        /// </summary>
        public IWebRoleConnectManager Initialize()
        {
            // Start listening to incoming topics
            PinkoContainer
                .Resolve<IPinkoApplication>()
                .RunInWorkerThread(
                    () => // All Worker roles
                    PinkoContainer
                        .Resolve<IBusMessageServer>()
                        .GetTopic(PinkoContainer.Resolve<IPinkoConfiguration>().PinkoMessageBusAllWebRolesTopic)
                        .Listen()
                );

            PinkoContainer
                .Resolve<IPinkoApplication>()
                .RunInWorkerThread(
                    () => // Selected
                    PinkoContainer
                        .Resolve<IBusMessageServer>()
                        .GetTopic(PinkoContainer.Resolve<IPinkoConfiguration>().PinkoMessageBusFeedToClientTopic, Guid.NewGuid().ToString())
                        .Listen()
                );

            return this;
        }


        /// <summary>
        /// PinkoContainer
        /// </summary>
        [Dependency]
        public IUnityContainer PinkoContainer { get; set; }

        ///// <summary>
        ///// IPinkoApplication
        ///// </summary>
        //[Dependency]
        //public IPinkoApplication PinkoApplication { get; set; }

        ///// <summary>
        ///// IPinkoConfiguration
        ///// </summary>
        //[Dependency]
        //public IPinkoConfiguration PinkoConfiguration { private get; set; }
    }
}
