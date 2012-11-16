using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinkoCommon.Interface;

namespace PinkoCommon
{
    public class PinkoConfiguration : IPinkoConfiguration
    {
        /// <summary>
        /// Constructor - PinkoConfiguration 
        /// </summary>
        public PinkoConfiguration()
        {
            QueueConfiguration = new Dictionary<string, Tuple<string, bool>>();

            QueueConfiguration[PinkoMessageBusToAllWorkersTopic] = new Tuple<string, bool>(PinkoMessageBusToAllWorkersTopic, true);

            QueueConfiguration[PinkoMessageBusToWorkerCalcEngineAllTopic] = new Tuple<string, bool>(PinkoMessageBusToWorkerCalcEngineAllTopic, true);
            QueueConfiguration[PinkoMessageBusToWorkerCalcEngineTopic] = new Tuple<string, bool>(PinkoMessageBusToWorkerCalcEngineTopic, true);

            QueueConfiguration[PinkoMessageBusToWebRolesAllTopic] = new Tuple<string, bool>(PinkoMessageBusToWebRolesAllTopic, true);
            QueueConfiguration[PinkoMessageBusToWebRoleTopic] = new Tuple<string, bool>(PinkoMessageBusToWebRoleTopic, true);

            QueueConfiguration[PinkoMessageBusToWebRoleCalcResultTopic] = new Tuple<string, bool>(PinkoMessageBusToWebRoleCalcResultTopic, true);

            QueueConfiguration[PinkoMessageBusToWorkerSubscriptionManagerAllTopic] = new Tuple<string, bool>(PinkoMessageBusToWorkerSubscriptionManagerAllTopic, true);
            QueueConfiguration[PinkoMessageBusToWorkerSubscriptionManagerTopic] = new Tuple<string, bool>(PinkoMessageBusToWorkerSubscriptionManagerTopic, true);

            QueueConfiguration[PinkoMessageBusToCalcEngineQueue] = new Tuple<string, bool>(PinkoMessageBusToCalcEngineQueue, false);  // Queue

            // Set bus connection screen
            KeyValues["Issuer"] = "----";
            KeyValues["SecretKey"] = "-------";

            KeyValues["Microsoft.ServiceBus.ConnectionString"] = "Endpoint=sb://pinko-app-bus-dev.servicebus.windows.net;SharedSecretIssuer=" + KeyValues["Issuer"] + ";SharedSecretValue=" + KeyValues["SecretKey"];
        }

        /// <summary>
        /// EnvironmentVariables
        /// </summary>
        public IDictionary EnvironmentVariables
        {
            get { return Environment.GetEnvironmentVariables(); }
        }


        /// <summary>
        /// HeartbeatInterval in seconds
        /// </summary>
        public double HeartbeatIntervalSec
        {
            get { return _heartbeatIntervalSec; }
            set { _heartbeatIntervalSec = value; }
        }
        private double _heartbeatIntervalSec = 5;

        /// <summary>
        /// Queue/Topic configuration
        /// </summary>
        public Dictionary<string, Tuple<string, bool>> QueueConfiguration { get; set; }

        /// <summary>
        /// Message queue interval check 5
        /// </summary>
        public int MessageQueueCheckIntervalMs
        {
            get { return 1000; }
        }

        /// <summary>
        /// Calc engine send real time data to specific clients. Client use selectors.
        /// </summary>
        public string PinkoMessageBusToWebRoleTopic
        {
            get { return "PinkoMessageBusToWebRoleTopic"; }
        }

        /// <summary>
        /// Broadcast to all web roles
        /// </summary>
        public string PinkoMessageBusToWebRolesAllTopic
        {
            get { return "PinkoMessageBusToAllWebRolesTopic"; }
        }

        /// <summary>
        /// Main bus for ALL workers 
        /// </summary>
        public string PinkoMessageBusToAllWorkersTopic
        {
            get { return "PinkoMessageBusToAllWorkersTopic"; }
        }

        /// <summary>
        /// All subscription managers worker roles
        /// </summary>
        public string PinkoMessageBusToWorkerSubscriptionManagerAllTopic
        {
            get { return "PinkoMessageBusToWorkerAllSubscriptionManagerWorker";  } 
        }


        /// <summary>
        /// Single subscription managers worker role
        /// </summary>
        public string PinkoMessageBusToWorkerSubscriptionManagerTopic
        {
            get { return "PinkoMessageBusToWorkerSubscriptionManagerWorker"; }
        }

        /// <summary>
        /// Queue into initial subscription to calc engines
        /// </summary>
        public string PinkoMessageBusToCalcEngineQueue
        {
            get { return "PinkoMessageBusToCalcEngineQueue"; }
        }

        /// <summary>
        /// Main bus for calcEngines
        /// </summary>
        public string PinkoMessageBusToWorkerCalcEngineAllTopic
        {
            get { return "PinkoMessageBusToWorkerAllCalcEngineTopic"; }
        }

        /// <summary>
        /// To worker role with calcengineId selector
        /// </summary>
        public string PinkoMessageBusToWorkerCalcEngineTopic
        {
            get { return "PinkoMessageBusToWorkerCalcEngineTopic"; }
        }

        /// <summary>
        /// To CaclEngine calculation results to Web roles
        /// </summary>
        public string PinkoMessageBusToWebRoleCalcResultTopic
        {
            get { return "PinkoMessageBusToWebRoleCalcResultTopic"; }
        }

        /// <summary>
        /// Get configuration setting
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetSetting(string name)
        {
            if (!KeyValues.ContainsKey(name))
                KeyValues[name] = name;

            return KeyValues[name];
        }

        /// <summary>
        /// Set the interval for engine calculation
        /// </summary>
        public int RunCalcIntervalMs
        {
            get { return 500; }
        }

        /// <summary>
        /// Interval to increase realtime logging interval
        /// </summary>
        public long RealTimeLogIntervalInterval
        {
            get { return 1000; }
        }

        /// <summary>
        /// Client timeout. 
        /// </summary>
        public int ClientTimeoutThresholdMs
        {
            get { return _clientTimeoutThresholdMs; }
        }
        public int _clientTimeoutThresholdMs = 15000;

        /// <summary>
        /// Interval check for timeout connections
        /// </summary>
        public int ClientTimeoutThresholdIntervalMs
        {
            get { return _clientTimeoutThresholdIntervalMs; } 
        }
        public int _clientTimeoutThresholdIntervalMs = 2000;


        public Dictionary<string, string> KeyValues = new Dictionary<string, string>();
    }
}
