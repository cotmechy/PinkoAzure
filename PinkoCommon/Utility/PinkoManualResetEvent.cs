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
    public class PinkoManualResetEvent
    {
        /// <summary>
        /// Constructor - PinkoManualResetEvent 
        /// </summary>
        public PinkoManualResetEvent(bool signaled)
        {
            _nativeEvent = new ManualResetEvent(signaled);
        }

        /// <summary>
        /// Signal event
        /// </summary>
        /// <returns></returns>
        public void Set()
        {
            _nativeEvent.Set();
        }

        /// <summary>
        /// unsignal event
        /// </summary>
        /// <returns></returns>
        public void Reset()
        {
            _nativeEvent.Reset();
        }

        /// <summary>
        /// Wait for ever. Logs possible deadlock.
        /// </summary>
        /// <returns>
        /// True: signaled
        /// False: non-signaled
        /// </returns>
        public bool WaitOne(int milliseconds)
        {
            return _nativeEvent.WaitOne(milliseconds);
        }

        /// <summary>
        /// Wait for ever. Logs possible deadlock.
        /// </summary>
        /// <returns></returns>
        public long WaitOne()
        {
            var startTimes = TimeoutTimes;

            while (!_nativeEvent.WaitOne(DefaultMilliseconds))
            {
                Trace.TraceWarning("Possible Deadlock. Threshold: {0}", DefaultMilliseconds);
                Interlocked.Increment(ref _timeoutTimes);
            }

            return TimeoutTimes - startTimes;
        }

        /// <summary>
        /// Default timeout for logging when infinite wait
        /// </summary>
        public int DefaultMilliseconds = 30000;

        /// <summary>
        /// native event
        /// </summary>
        private readonly ManualResetEvent _nativeEvent;

        /// <summary>
        /// count of how many time this instance have timedout
        /// </summary>
        public long TimeoutTimes
        {
            get { return Interlocked.Read(ref _timeoutTimes); }
        }
        private long _timeoutTimes;
    }
}
