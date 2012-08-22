using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkoCommon.Interface
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
        /// Calc engine send real time data to specific clients. Clinet use selectors.
        /// </summary>
        string PinkoMessageBusToWebFeedToClientTopic { get; }

        /// <summary>
        /// Broadcast to all web roles
        /// </summary>
        string PinkoMessageBusToWebAllRolesTopic { get; }

        /// <summary>
        /// Broadcast to all Calc Engines
        /// </summary>
        string PinkoMessageBusToWorkerAllRolesTopic { get; }

        /// <summary>
        /// Messgaes to specific Calc engine. the calc engine has  selector
        /// </summary>
        string PinkoMessageBusToWorkerCalcEngineActionTopic { get; }

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
