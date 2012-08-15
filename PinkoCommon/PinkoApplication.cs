using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        /// Run in backgroung 
        /// </summary>
        public void RunInBackground(Action action)
        {
            Task.Factory.StartNew(() => TryCatch.RunInTry(action));
        }

        /// <summary>
        /// Run in a Pinko managed worker thread
        /// </summary>
        public void RunInWrokerThread(Action action)
        {
            var workerThread = new Thread(new ThreadStart(action)) {IsBackground = true};
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
        /// MachineName
        /// </summary>
        public string MachineName
        {
            get { return Environment.MachineName; }
        }

        /// <summary>
        /// Environamnet User Name
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
