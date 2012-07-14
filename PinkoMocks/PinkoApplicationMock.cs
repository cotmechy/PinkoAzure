using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Practices.Unity;
using PinkoWorkerCommon.Interface;
using PinkoWorkerCommon.Utility;

namespace PinkoMocks
{
    public class PinkoApplicationMock : IPinkoApplication
    {
        /// <summary>
        /// Constructor - PinkoApplicationMock 
        /// </summary>
        public PinkoApplicationMock()
        {
            ApplicationRunningEvent = new ManualResetEvent(false);
        }

        /// <summary>
        /// Event to be signaled when application ends
        /// </summary>
        public ManualResetEvent ApplicationRunningEvent { get; private set; }

        /// <summary>
        /// Environamnet User Name
        /// </summary>
        public string UserName
        {
            get { return "USERNAME"; }
        }

        /// <summary>
        /// MachineName
        /// </summary>
        public string MachineName
        {
            get { return "MachineName"; }
        }

        /// <summary>
        /// Run in backgroung 
        /// </summary>
        public void RunInBackground(Action action)
        {
            action();
        }

        /// <summary>
        /// Run in background (Task)
        /// </summary>
        /// <param name="action"></param>
        /// <param name="callback">Callback when Asyn operation is processed. Exception is null if exception occurrs</param>
        public void RunInBackground(Action action, Action<Exception> callback)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// EnvironmentVariables
        /// </summary>
        public IDictionary EnvironmentVariables
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Get in memory 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IRxMemoryBus<T> GetBus<T>()
        {
            object bus = null;

            if (!_concurrentDictionary.TryGetValue(typeof(T), out bus))
                _concurrentDictionary.TryAdd(typeof(T), bus = PinkoContainer.Resolve<MemoryBus<T>>());

            return bus as IRxMemoryBus<T>;
        }

        /// <summary>
        /// Get in memory - subscriber
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IObservable<T> GetSubscriber<T>()
        {
            return GetBus<T>().Subscriber;
        }
        readonly ConcurrentDictionary<Type, object> _concurrentDictionary = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// IUnityContainer
        /// </summary>
        [Dependency]
        public IUnityContainer PinkoContainer { get; set; }
    }
}
