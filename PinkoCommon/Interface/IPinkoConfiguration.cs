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
        /// Main bus for ALL workers 
        /// </summary>
        string PinkoMessageBusToAllWorkersTopic { get; }


        /// <summary>
        /// All subscription managers worker roles
        /// </summary>
        string PinkoMessageBusToWorkerSubscriptionManagerAllTopic { get; }

        /// <summary>
        /// Single subscription managers worker role
        /// </summary>
        string PinkoMessageBusToWorkerSubscriptionManagerTopic { get; }

        /// <summary>
        /// Calc engine send real time data to specific clients. Client use selectors.
        /// </summary>
        string PinkoMessageBusToWebRoleTopic { get; }

        /// <summary>
        /// Broadcast to all web roles
        /// </summary>
        string PinkoMessageBusToWebRolesAllTopic { get; }

        /// <summary>
        /// Broadcast to all Calc Engines
        /// </summary>
        string PinkoMessageBusToWorkerCalcEngineAllTopic { get; }

        /// <summary>
        /// Messages to specific Calc engine. the calc engine has  selector
        /// </summary>
        string PinkoMessageBusToWorkerCalcEngineTopic { get; }

        /// <summary>
        /// To WebRole calculation results
        /// </summary>
        string PinkoMessageBusToWebRoleCalcResultTopic { get; }

        /// <summary>
        /// Queue/Topic configuration
        /// </summary>
        Dictionary<string, Tuple<string, bool>> QueueConfiguration { get; }

        /// <summary>
        /// Message queue interval check 
        /// </summary>
        int MessageQueueCheckIntervalMs { get; }

        /// <summary>
        /// Queue into initial subscription to calc engines
        /// </summary>
        string PinkoMessageBusToCalcEngineQueue { get; }

        /// <summary>
        /// Get configuration setting
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetSetting(string name);

        /// <summary>
        /// Set the interval for engine calculation
        /// </summary>
        int RunCalcIntervalMs { get; }

        /// <summary>
        /// Interval to increase realtime logging interval
        /// </summary>
        long RealTimeLogIntervalInterval { get; }

        /// <summary>
        /// Client timeout. 
        /// </summary>
        int ClientTimeoutThresholdMs { get; }
    
        /// <summary>
        /// Client timeout cehc intevals
        /// </summary>
        int ClientTimeoutThresholdIntervalMs { get; }
    }
}
