using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using PinkoCommon.Interface;
using PinkoCommon.Utility;

namespace PinkoCommon
{
    /// <summary>
    /// Encapsulate Application level functionality
    /// </summary>
    public class PinkoApplication : IPinkoApplication
    {
        /// <summary>
        /// Constructor - PinkoApplication 
        /// </summary>
        public PinkoApplication()
        {
            ApplicationRunningEvent = new ManualResetEvent(false);
        }

        /// <summary>
        /// Run in background treahPool/Task
        /// </summary>
        public void RunInBackground(Action action)
        {
            Task.Factory.StartNew(() => TryCatch.RunInTry(action));
        }

        /// <summary>
        /// Run in a Pinko managed worker thread
        /// </summary>
        public void RunInWorkerThread(string name, Action action)
        {
            var workerThread = new Thread(new ThreadStart(action)) {IsBackground = true, Name = name};
            workerThread.Start();
        }

        /// <summary>
        /// Run in background (Task)
        /// </summary>
        /// <param name="action"></param>
        /// <param name="callback">Callback when Asyn operation is processed. Exception is null if exception occurrs</param>
        public void RunInBackground(Action action, Action<Exception> callback)
        {
            Task.Factory.StartNew(() =>
            {
                var exception = TryCatch.RunInTry(action);
                callback(exception);
            });
        }


        /// <summary>
        /// Get in memory 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IRxMemoryBus<T> GetBus<T>()
        {
            object bus = null;

            if (!_busDictionary.TryGetValue(typeof(T), out bus))
                _busDictionary.TryAdd(typeof(T), bus = PinkoContainer.Resolve<MemoryBus<T>>());

            return bus as IRxMemoryBus<T>;
        }

        /// <summary>
        /// Get in memory 
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
        public IScheduler ThreadPoolScheduler
        {
            get { return Scheduler.CurrentThread; }
            //get { return Scheduler.ThreadPool; }
        }

        /// <summary>
        /// Run parallel
        /// </summary>
        public void ForEachParallel<T>(IEnumerable<T> collection, Action<T> action)
        {
            collection.AsParallel().ForAll(action);
        }

        /// <summary>
        /// Get Time atomic sequence. 
        /// </summary>
        /// <returns></returns>
        public long GetTimeSequence()
        {
            return DateTime.Now.Ticks;
        }

        /// <summary>
        /// MachineName
        /// </summary>
        public string MachineName
        {
            get { return Environment.MachineName; }
        }

        /// <summary>
        /// Environment User Name
        /// </summary>
        public string UserName
        {
            get { return Environment.UserName; }
        }

        /// <summary>
        /// 
        /// </summary>
        readonly ConcurrentDictionary<Type, object> _busDictionary = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Event to be signaled when application ends
        /// </summary>
        public ManualResetEvent ApplicationRunningEvent { get; private set; }

        /// <summary>
        /// IUnityContainer
        /// </summary>
        [Dependency]
        public IUnityContainer PinkoContainer { get; set; }
    }
}
