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
            QueueConfiguration[PinkoMessageBusToAllWorkerRolesTopic] = new Tuple<string, bool>(PinkoMessageBusToAllWorkerRolesTopic, true);
            QueueConfiguration[PinkoMessageBusToWorkerCalcEngineTopic] = new Tuple<string, bool>(PinkoMessageBusToWorkerCalcEngineTopic, true);

            QueueConfiguration[PinkoMessageBusToAllWebRolesTopic] = new Tuple<string, bool>(PinkoMessageBusToAllWebRolesTopic, true);
            QueueConfiguration[PinkoMessageBusToWebRoleTopic] = new Tuple<string, bool>(PinkoMessageBusToWebRoleTopic, true);

            QueueConfiguration[PinkoMessageBusToWebRoleCalcResultTopic] = new Tuple<string, bool>(PinkoMessageBusToWebRoleCalcResultTopic, true);

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
        public string PinkoMessageBusToWebRoleTopic
        {
            get { return "PinkoMessageBusToWebRoleTopic"; }
        }

        /// <summary>
        /// Broadcast to all web roles
        /// </summary>
        public string PinkoMessageBusToAllWebRolesTopic
        {
            get { return "PinkoMessageBusToAllWebRolesTopic"; }
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
        public string PinkoMessageBusToAllWorkerRolesTopic
        {
            get { return "PinkoMessageBusToAllWorkerRolesTopic"; }
        }

        /// <summary>
        /// To worker role with calcengineId selector
        /// </summary>
        public string PinkoMessageBusToWorkerCalcEngineTopic
        {
            get { return "PinkoMessageBusToWorkerCalcEngineTopic"; }
        }

        /// <summary>
        /// To WebRole calculation results
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

        public Dictionary<string, string> KeyValues = new Dictionary<string, string>();
    }
}
