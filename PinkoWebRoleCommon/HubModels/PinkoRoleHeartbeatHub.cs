using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SignalR.Hubs;

namespace PinkoWebRoleCommon.HubModels
{
    /// <summary>
    /// PinkoRoleHeartbeatHub
    /// </summary>
    public class PinkoRoleHeartbeatHub : Hub
    {
        /// <summary>
        /// 
        /// </summary>
        private static string _responderMachine = Environment.MachineName;
        public string ResponderMachine
        {
            get { return _responderMachine; }
            set { _responderMachine = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        private DateTime _responderDateTime = DateTime.Now;
        public DateTime ResponderDateTime
        {
            get { return _responderDateTime; }
            set { _responderDateTime = value; }
        }
    }


    /// <summary>
    /// PinkoRoleHeartbeatHubExtensions
    /// </summary>
    public static class PinkoRoleHeartbeatHubExtensions
    {

        /// <summary>
        /// PinkoRoleHeartbeatHub
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Verbose(this PinkoRoleHeartbeatHub obj)
        {
            return string.Format("({2}) PinkoRoleHeartbeatHub: ResponderMachine: {0} - ResponderDateTime: {1}"
                                            , obj.ResponderMachine
                                            , obj.ResponderDateTime
                                            , obj.GetHashCode()
                                            );
        }
    }
}
