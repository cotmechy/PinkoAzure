using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using SignalR;

namespace PinkoWebRole.Utility
{
    /// <summary>
    /// https://github.com/SignalR/SignalR/wiki/QuickStart-Persistent-Connections
    /// </summary>
    public class SignalrPinkoConnection : PersistentConnection
    {
        /// <summary>
        /// OnReceivedAsync
        /// </summary>
        protected override Task OnReceivedAsync(IRequest request, string connectionId, string data)
        {
            Debug.WriteLine("**** OnReceivedAsync(): ConnectionId: {0}, Data: {1}", connectionId, data);

            return Connection.Broadcast(data);
        }
    }
}