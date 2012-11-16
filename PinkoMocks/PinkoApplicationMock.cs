using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using PinkoCommon;
using PinkoCommon.Extension;
using PinkoCommon.Interface;

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
        /// Environment User Name
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
        /// Run in background 
        /// </summary>
        public void RunInBackground(Action action)
        {
            action();
        }

        /// <summary>
        /// Run in a Pinko managed worker thread
        /// </summary>
        public void RunInWorkerThread(string name, Action action)
        {
            //Task.Factory.StartNew(action);
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

        public IScheduler CurrentScheduler
        {
            get { return Scheduler.CurrentThread; }
        }

        /// <summary>
        /// Get thread pool schedules. Set to same thread in unit test.
        /// </summary>
        public IScheduler ThreadPoolScheduler
        {
            get { return Scheduler.CurrentThread; }
        }

        /// <summary>
        /// Get Time atomic sequence. 
        /// </summary>
        /// <returns></returns>
        public long GetTimeSequence()
        {
            return TestSequenceValue;
        }

        /// <summary>
        /// Run parallel
        /// </summary>
        public void ForEachParallel<T>(IEnumerable<T> collection, Action<T> action)
        {
            collection.ForEach(action);
            //collection.AsParallel().ForAll(action);
        }

        /// <summary>
        /// Get reactive timer
        /// </summary>
        /// <returns></returns>
        public IDisposable RunActionInTimer(int intervalMs, Action action)
        {
            return null;
        }

        public const long TestSequenceValue = 100000009;

        /// <summary>
        /// Get in memory - subscriber
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IObservable<T> GetSubscriber<T>()
        {
            return GetBus<T>().Subscriber;
        }

        /// <summary>
        /// Get thread pool schedules. Set to same thread in unit test.
        /// </summary>
        readonly ConcurrentDictionary<Type, object> _concurrentDictionary = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// IUnityContainer
        /// </summary>
        [Dependency]
        public IUnityContainer PinkoContainer { get; set; }
    }
}
