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
            QueueConfiguration[PinkoMessageBusAllWorkerRolesTopic] = new Tuple<string, bool>(PinkoMessageBusAllWorkerRolesTopic, true);
            QueueConfiguration[PinkoMessageBusCalcEngineActionTopic] = new Tuple<string, bool>(PinkoMessageBusCalcEngineActionTopic, false);

            // Set bus connection screen
            //KeyValues["Microsoft.ServiceBus.ConnectionString"] = "Endpoint=sb://pinko-app-bus-dev.servicebus.windows.net/PinkoWorkerRoles/;SharedSecretIssuer=PinkoServiceIdentity;SharedSecretValue=a1zFOPUL38jCVT+fQ9Arrihdt2E8MW84LgDR3ZIYWcU=";
            //KeyValues["Microsoft.ServiceBus.ConnectionString"] = "Endpoint=sb://pinko-app-bus-dev.servicebus.windows.net;SharedSecretValue=rgJrSVkS7LkzvzkuVlP+mFMGEPI4QLdQ3rO91lan1aE=";   // /PANoEiC88Ox17lwbKfL7jmtAK1Xgdk+Z8S51+DtAtc=
            //KeyValues["Microsoft.ServiceBus.ConnectionString"] = "Endpoint=sb://pinko-app-bus-dev.servicebus.windows.net;SharedSecretIssuer=PinkoWorkerRoles;SharedSecretValue=FZuAdPzBbmGgqnEUh6zaJHaoBVcvP7lSzV5U5kbdeCo=";

            KeyValues["Issuer"] = "== Get from secure notes ==";
            KeyValues["SecretKey"] = "== Get from secure notes ==";

            //KeyValues["Issuer"] = "PinkoWorkerRoles";
            //KeyValues["SecretKey"] = "FZuAdPzBbmGgqnEUh6zaJHaoBVcvP7lSzV5U5kbdeCo=";

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
        /// Calc engine send real time data to specific clients. Clinet use selectors.
        /// </summary>
        public string PinkoMessageBusFeedToClientTopic
        {
            get { return "PinkoMessageBusFeedToClientTopic"; }
        }

        /// <summary>
        /// Broadcast to all web roles
        /// </summary>
        public string PinkoMessageBusAllWebRolesTopic
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
        /// Message queue interval check 
        /// </summary>
        public int MessageQueueCheckIntervalMs
        {
            get { return 10000; }
        }

        /// <summary>
        /// Main bus for workers 
        /// </summary>
        public string PinkoMessageBusAllWorkerRolesTopic
        {
            get { return "PinkoMessageBusCrossWebRolesQueue"; }
        }

        /// <summary>
        /// Topic brodcats to all clients
        /// </summary>
        public string PinkoMessageBusCalcEngineActionTopic
        {
            get { return "PinkoMessageBusWebRoleToClientsTopic"; }
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
