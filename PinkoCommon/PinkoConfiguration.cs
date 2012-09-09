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
            QueueConfiguration[PinkoMessageBusToWorkerAllRolesTopic] = new Tuple<string, bool>(PinkoMessageBusToWorkerAllRolesTopic, true);
            QueueConfiguration[PinkoMessageBusToWorkerCalcEngineActionTopic] = new Tuple<string, bool>(PinkoMessageBusToWorkerCalcEngineActionTopic, true);

            // Set bus connection screen

            KeyValues["Issuer"] = "== Get from secure notes ==";
            KeyValues["SecretKey"] = "== Get from secure notes ==";

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
        /// Calc engine send real time data to specific clients. Client use selectors.
        /// </summary>
        public string PinkoMessageBusToWebFeedToClientTopic
        {
            get { return "PinkoMessageBusFeedToClientTopic"; }
        }

        /// <summary>
        /// Broadcast to all web roles
        /// </summary>
        public string PinkoMessageBusToWebAllRolesTopic
        {
            get { return "PinkoMessageBusAllWebRolesTopic";  }
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
            get { return 10000; }
        }

        /// <summary>
        /// Main bus for workers 
        /// </summary>
        public string PinkoMessageBusToWorkerAllRolesTopic
        {
            get { return "PinkoMessageBusToWorkerAllRolesTopic"; }
        }

        /// <summary>
        /// To worker role with calcengineId selector
        /// </summary>
        public string PinkoMessageBusToWorkerCalcEngineActionTopic
        {
            get { return "PinkoMessageBusToWorkerCalcEngineActionTopic"; }
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

        public Dictionary<string, string> KeyValues = new Dictionary<string, string>();
    }
}
