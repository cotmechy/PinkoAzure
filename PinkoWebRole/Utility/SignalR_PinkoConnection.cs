using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using PinkoWorkerCommon.Extensions;
using SignalR;

namespace PinkoWebRole.Utility
{
    /// <summary>
    /// https://github.com/SignalR/SignalR/wiki/QuickStart-Persistent-Connections
    /// </summary>
    public class SignalrPinkoConnection : PersistentConnection
    {
        /// <summary>
        /// Constructor - SignalrPinkoConnection 
        /// </summary>
        public SignalrPinkoConnection()
        {
            Debug.WriteLine(this.VerboseIdentity());
        }

        /// <summary>
        /// OnReceivedAsync
        /// </summary>
        protected override Task OnReceivedAsync(IRequest request, string connectionId, string data)
        {
            Debug.WriteLine("{2}: OnReceivedAsync(): ConnectionId: {0}, Data: {1} - SignalR Name: {3}"
                                                , connectionId
                                                , data
                                                , this.VerboseIdentity()
                                                , request.Verbose()
                                                );

            return Connection.Broadcast(data);
        }
    }


    /// <summary>
    /// SignalrPinkoConnectionExtensions
    /// </summary>
    public static class SignalrPinkoConnectionExtensions
    {

        /// <summary>
        /// SignalrPinkoConnection
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Verbose(this SignalrPinkoConnection obj)
        {
            return string.Format("{0} "
                                             , obj.VerboseIdentity()
                                             );
        }
    }

}