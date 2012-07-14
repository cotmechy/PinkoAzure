using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkoWorkerCommon.Interface
{

    ///// <summary>
    ///// Generig
    ///// </summary>
    //public interface IRxMemoryBusBase
    //{
    //    /// <summary>
    //    /// Publish message into bus
    //    /// </summary>
    //    /// <returns></returns>
    //    void Publish(object message);
    //}


    /// <summary>
    /// In Memory Reactive Extension observable memory bus
    /// </summary>
    public interface IRxMemoryBus<T>
    {
        /// <summary>
        /// Initialize
        /// </summary>
        void Initialize();

        /// <summary>
        /// Get reactive Subscriber
        /// </summary>
        /// <returns></returns>
        IObservable<T> Subscriber { get; }

        /// <summary>
        /// Publish message into bus
        /// </summary>
        /// <returns></returns>
        void Publish(T message);
    }
}
