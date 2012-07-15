using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkDao
{
    /// <summary>
    /// PinkoRoleHeartbeat
    /// </summary>
    public class PinkoRoleHeartbeat
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
}
