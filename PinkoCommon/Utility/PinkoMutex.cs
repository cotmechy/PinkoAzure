using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace PinkoCommon.Utility
{
    /// <summary>
    /// Wrapper around native event to allows deadlock logging
    /// </summary>
    public class PinkoMutex
    {
        /// <summary>
        /// signal event
        /// </summary>
        /// <returns></returns>
        public void Release()
        {
            _mutext.ReleaseMutex();
        }

        /// <summary>
        /// Wait for specific timeout
        /// </summary>
        /// <returns>
        /// True: signaled
        /// False: non-signaled
        /// </returns>
        public bool Lock(int milliseconds)
        {
            return _mutext.WaitOne(milliseconds);
        }

        /// <summary>
        /// Wait for ever. Logs possible deadlock.
        /// </summary>
        /// <returns></returns>
        public long Lock()
        {
            var startTimes = TimeoutTimes;

            while (!Lock(DefaultMilliseconds))
            {
                Trace.TraceWarning("Possible Deadlock. Times: {1} - Threshold: {0}", DefaultMilliseconds, TimeoutTimes - startTimes);
                Interlocked.Increment(ref _timeoutTimes);
            }

            return TimeoutTimes - startTimes;
        }


        /// <summary>
        /// Lock in using statement - Disposible pattern
        /// </summary>
        public PinkoMutexDisposable LockDisposible()
        {
            return new PinkoMutexDisposable(this);
        }

        /// <summary>
        /// Lock in using statement - Disposible pattern
        /// </summary>
        public PinkoMutexDisposable LockDisposible(int milliseconds)
        {
            return new PinkoMutexDisposable(this, milliseconds);
        }


        /// <summary>
        /// Default timeout for logging when infinite wait
        /// </summary>
        public int DefaultMilliseconds = 30000;

        /// <summary>
        /// count of how many time this instance have timeout
        /// </summary>
        public long TimeoutTimes
        {
            get { return Interlocked.Read(ref _timeoutTimes); }
        }
        private long _timeoutTimes;

        /// <summary>
        /// Kernel Mutes
        /// </summary>
        private readonly Mutex _mutext = new Mutex();
    }



    /// <summary>
    /// Use mutex with disposable patter
    /// </summary>
    public class PinkoMutexDisposable : IDisposable
    {
        private readonly PinkoMutex _mutext;
        private bool _isLocked = false;

        /// <summary>
        /// Constructor - PinkoMutexDispodable 
        /// </summary>
        public PinkoMutexDisposable(PinkoMutex mutex)
        {
            _mutext = mutex;
            _mutext.Lock();
            _isLocked = true;
        }

        public PinkoMutexDisposable(PinkoMutex mutex, int milliseconds)
        {
            _mutext = mutex;
            _isLocked = _mutext.Lock(milliseconds);
        }

        /// <summary>
        /// Is underlying mutex locked ?
        /// </summary>
        public bool IsLocked
        {
            get { return _isLocked; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_isLocked)
                _mutext.Release();
            _isLocked = false;
        }
    }

}
