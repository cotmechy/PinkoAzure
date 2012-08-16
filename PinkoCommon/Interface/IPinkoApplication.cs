using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PinkoCommon.Interface
{
    /// <summary>
    /// Application level services for threading, etc ...
    /// </summary>
    public interface IPinkoApplication
    {
        /// <summary>
        /// Event to be signaled when application ends
        /// </summary>
        ManualResetEvent ApplicationRunningEvent { get; }

        /// <summary>
        /// Environamnet User Name
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// Machine Name
        /// </summary>
        string MachineName { get; }

        /// <summary>
        /// Run in backgroung 
        /// </summary>
        void RunInBackground(Action action);

        /// <summary>
        /// Run in a Pinko managed worker thread
        /// </summary>
        void RunInWorkerThread(Action action);

        /// <summary>
        /// Run in background (Task)
        /// </summary>
        /// <param name="action"></param>
        /// <param name="callback">Callback when Asyn operation is processed. Exception is null if exception occurrs</param>
        void RunInBackground(Action action, Action<Exception> callback);

        /// <summary>
        /// Get in memory 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IRxMemoryBus<T> GetBus<T>();

        /// <summary>
        /// Get in memory - subscriber
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IObservable<T> GetSubscriber<T>();
    }
}
