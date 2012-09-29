using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Practices.Unity;
using PinkoCommon.Interface;
using PinkoWorkerCommon.Handler;
using PinkoWorkerCommon.Interface;

namespace PinkoWorkerCommon
{
    /// <summary>
    /// Base worker role setup and runtime
    /// </summary>
    public class WorkerRoleFrame : IWorkerRoleFrame
    {
        /// <summary>
        /// Run()
        /// </summary>
        public void Run(string brodcastTopic, string directTopic)
        {
            // Register base handlers
            MessageReceiveHandlers.AddRange(new object[]
            {
                PinkoContainer.Resolve<BusListenerPinkoPingMessage>().Register(),
            });

            PinkoApplication.RunInWorkerThread(PinkoConfiguration.PinkoMessageBusToAllWorkersTopic,
                () => 
                PinkoContainer
                    .Resolve<IBusMessageServer>()
                    .GetTopic(PinkoConfiguration.PinkoMessageBusToAllWorkersTopic)
                    .Listen()
                );

            PinkoApplication.RunInWorkerThread(directTopic,
                () => 
                PinkoContainer
                    .Resolve<IBusMessageServer>()
                    .GetTopic(directTopic, _selector)
                    .Listen()
                );

            PinkoApplication.RunInWorkerThread(brodcastTopic,
                () => 
                PinkoContainer
                    .Resolve<IBusMessageServer>()
                    .GetTopic(brodcastTopic)
                    .Listen());
        }


        /// <summary>
        /// Worker Role OnStop()
        /// </summary>
        public void Stop()
        {
            Trace.TraceInformation("Stopping worker role...");

            PinkoApplication.ApplicationRunningEvent.Set();
            PinkoContainer.Resolve<IBusMessageServer>().Deinitialize();

            Thread.Sleep(5000);
        }


        /// <summary>
        /// Unique Selector
        /// </summary>
        private readonly string _selector = Guid.NewGuid().ToString();

        /// <summary>
        /// MessageHandlers 
        /// </summary>
        public List<object> MessageReceiveHandlers
        {
            get { return _messageReceiveHandlers; }
        }
        private readonly List<object> _messageReceiveHandlers = new List<object>();

        /// <summary>
        /// PinkoContainer
        /// </summary>
        [Dependency]
        public IUnityContainer PinkoContainer { get; set; }

        /// <summary>
        /// IPinkoApplication
        /// </summary>
        [Dependency]
        public IPinkoApplication PinkoApplication { get; set; }

        /// <summary>
        /// IPinkoConfiguration
        /// </summary>
        [Dependency]
        public IPinkoConfiguration PinkoConfiguration { private get; set; }
    }
}
