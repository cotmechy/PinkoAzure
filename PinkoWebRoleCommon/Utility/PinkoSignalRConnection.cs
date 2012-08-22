using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PinkoCommon.Extensions;
using PinkoWebRoleCommon.Extensions;
using SignalR;

namespace PinkoWebRoleCommon.Utility
{
    /// <summary>
    /// https://github.com/SignalR/SignalR/wiki/QuickStart-Persistent-Connections
    /// </summary>
    public class PinkoSignalRConnection : PersistentConnection
    {
        /// <summary>
        /// Constructor - SignalrPinkoConnection 
        /// </summary>
        public PinkoSignalRConnection()
        {
            Trace.TraceInformation(this.VerboseIdentity());
        }

        /// <summary>
        /// OnReceivedAsync
        /// </summary>
        protected override Task OnReceivedAsync(IRequest request, string connectionId, string data)
        {
            Trace.TraceInformation("{2}: OnReceivedAsync(): ConnectionId: {0}, Data: {1} - SignalR Name: {3}"
                                                , connectionId
                                                , data
                                                , this.VerboseIdentity()
                                                , request.Verbose()
                                                );

            return Connection.Broadcast(data);
        }
    }


    /// <summary>
    /// PinkoSignalRConnectionExtensions
    /// </summary>
    public static class PinkoSignalRConnectionExtensions
    {

        /// <summary>
        /// PinkoSignalRConnection
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Verbose(this PinkoSignalRConnection obj)
        {
            return string.Format("{0} "
                                             , obj.VerboseIdentity()
                                             );
        }
    }
}
