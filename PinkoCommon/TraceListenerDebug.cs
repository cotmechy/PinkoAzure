using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PinkoCommon
{
    public class TraceListenerDebug : TraceListener
    {
        /// <summary>
        /// When overridden in a derived class, writes the specified message to the listener you create in the derived class.
        /// </summary>
        /// <param name="message">A message to write. </param><filterpriority>2</filterpriority>
        public override void Write(string message)
        {
            //Debug.Write(string.Format("{1}: {0}", message, Process.GetCurrentProcess().Id), _header());
        }

        /// <summary>
        /// When overridden in a derived class, writes a message to the listener you create in the derived class, followed by a line terminator.
        /// </summary>
        /// <param name="message">A message to write. </param><filterpriority>2</filterpriority>
        public override void WriteLine(string message)
        {
            //Debug.WriteLine(string.Format("{1}: {0}", message, Process.GetCurrentProcess().Id), _header());
        }

        private readonly Func<string> _header = new Func<string>( () => string.Format("{0}: ({1})", DateTime.Now.ToLongTimeString(), Process.GetCurrentProcess().Id )); 

    }
}
