using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkoWorkerCommon.Interface
{
    /// <summary>
    /// Base worker framework functionality
    /// </summary>
    public interface IWorkerRoleFrame
    {
        /// <summary>
        /// Run()
        /// </summary>
        void Run();

        ///// <summary>
        ///// Start Heart Beat
        ///// </summary>
        //void StartHeartBeat();

        /// <summary>
        /// Worker Role OnStop()
        /// </summary>
        void Stop();

        /// <summary>
        /// Hold handlers
        /// </summary>
        List<object> MessageReceiveHandlers { get; }
    }
}
