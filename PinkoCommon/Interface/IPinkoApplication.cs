using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
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
        /// Environment User Name
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// Machine Name
        /// </summary>
        string MachineName { get; }

        /// <summary>
        /// Run in background 
        /// </summary>
        void RunInBackground(Action action);

        /// <summary>
        /// Run in a Pinko managed worker thread
        /// </summary>
        void RunInWorkerThread(string name, Action action);

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

        /// <summary>
        /// Same thread scheduler. Set to same thread in unit test.
        /// </summary>
        IScheduler CurrentScheduler { get; }

        /// <summary>
        /// Get thread pool schedules. Set to same thread in unit test.
        /// </summary>
        IScheduler ThreadPoolScheduler { get; }

        /// <summary>
        /// Get Time atomic sequence. 
        /// </summary>
        long GetTimeSequence();

        /// <summary>
        /// Run parallel
        /// </summary>
        void ForEachParallel<T>(IEnumerable<T> collection, Action<T> action);

        /// <summary>
        /// Get reactive timer
        /// </summary>
        /// <returns></returns>
        IDisposable RunActionInTimer(int intervalMs, Action action);
    }
}
