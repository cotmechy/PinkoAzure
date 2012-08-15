using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace PinkoMobilePilot
{
    public class WebRole : RoleEntryPoint
    {
        /// <summary>
        /// OnStart
        /// </summary>
        /// <returns></returns>
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            Trace.TraceInformation("OnStart(): {0}", GetHashCode());
            return base.OnStart();
        }

        /// <summary>
        /// OnStop
        /// </summary>
        public override void OnStop()
        {
            Trace.TraceInformation("OnStop(): {0}", GetHashCode());
            base.OnStop();
        }
    }
}
