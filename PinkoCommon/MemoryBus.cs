using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Practices.Unity;
using PinkoCommon.Interface;

namespace PinkoCommon
{
    /// <summary>
    /// In Memory Reactive Extension observable memory bus
    /// </summary>
    public class MemoryBus<T> : IRxMemoryBus<T>
    {
        /// <summary>
        /// Init
        /// </summary>
        public void Initialize()
        {
            // http://weblogs.asp.net/sweinstein/archive/2010/01/10/16-ways-to-create-iobservables-without-implementing-iobservable.aspx
        }

        ///// <summary>
        ///// Publish message into bus
        ///// </summary>
        ///// <returns></returns>
        //public void Publish(object message)
        //{
        //    Publish(((T) message));
        //}

        /// <summary>
        /// Publish message into bus
        /// </summary>
        /// <returns></returns>
        public void Publish(T message)
        {
            Trace.TraceInformation("({3}): {2}: {1}: Publishing Message: {0}", message, GetType(), GetHashCode(), Process.GetCurrentProcess().Id);

            //PinkoApplication.RunInBackground(() => _subjectPump.OnNext(message));
            _subjectPump.OnNext(message);
        }

        ///// <summary>
        ///// Publish message into bus
        ///// </summary>
        ///// <returns></returns>
        //public void PublishSync(T message)
        //{
        //    //Trace.TraceInformation("Publishing Message (SYNC): {0}", message);

        //    _subjectPump.OnNext(message);
        //}

        /// <summary>
        /// Observable 
        /// </summary>
        public IObservable<T> Subscriber { get { return _subjectPump.AsObservable(); } }

        /// <summary>
        /// Subject pump
        /// </summary>
        readonly Subject<T> _subjectPump = new Subject<T>();

        ///// <summary>
        ///// IPinkoApplication
        ///// </summary>
        //[Dependency]
        //public IPinkoApplication PinkoApplication { get; set; }
    }
}
