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
        /// Start running Worker Role
        /// </summary>
        /// <param name="brodcastTopic">Non filtered topic to broadcast to all instance of the worker role</param>
        /// <param name="directTopic">direct filter topic to specific client</param>
        void Run(string brodcastTopic, string directTopic);

        ///// <summary>
        ///// Start Heart Beat
        ///// </summary>
        //void StartHeartBeat();

        /// <summary>
        /// Worker Role OnStop()
        /// </summary>
        void Stop();

        ///// <summary>
        ///// Hold handlers
        ///// </summary>
        //List<object> MessageReceiveHandlers { get; }
    }
}
