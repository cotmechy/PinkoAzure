using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkoWorkerCommon.Interface
{
    /// <summary>
    /// IPinkoConfiguration
    /// </summary>
    public interface IPinkoConfiguration
    {
        /// <summary>
        /// HeartbeatInterval in seconds
        /// </summary>
        double HeartbeatIntervalSec { get; }

        /// <summary>
        /// EnvironmentVariables
        /// </summary>
        IDictionary EnvironmentVariables { get; }

        /// <summary>
        /// Main bus for workers 
        /// </summary>
        string MessageBusCrossWebRolesQueue { get; }

        /// <summary>
        /// Topic brodcats to all clients
        /// </summary>
        string MessageBusWebRoleToClientsTopic { get; }

        /// <summary>
        /// Queue/Topic configuration
        /// </summary>
        Dictionary<string, Tuple<string, bool>> QueueConfiguration { get; }

        /// <summary>
        /// Message queue interval check 
        /// </summary>
        int MessageQueueCheckIntervalMs { get; }

        /// <summary>
        /// Get configuration setting
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetSetting(string name);
    }
}
