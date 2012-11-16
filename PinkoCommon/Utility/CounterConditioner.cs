using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PinkoCommon.Utility
{
    /// <summary>
    /// Keeps a counter to set threshold
    /// </summary>
    public class CounterConditioner
    {

        /// <summary>
        /// Constructor - CounterConditioner 
        /// </summary>
        public CounterConditioner(long interval)
        {
            _interval = interval;
        }


        /// <summary>
        /// Action if counter interval is reached
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool Process( Action action)
        {
            var counter = Interlocked.Increment(ref _counter);
            if (counter % _interval == 0 || counter == 1)
            {
                action();
                return true;
            }

            return false;
        }


        private long _counter = 0;
        private readonly long _interval;
    }
}
