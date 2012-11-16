using System;
using System.Collections.Generic;
using SignalR;
using SignalR.Hubs;

namespace PinkoMocks
{
    public class HubContextMock : IHubContext
    {
        /// <summary>
        /// Constructor - HubContextMock 
        /// </summary>
        public HubContextMock()
        {
            //var dict = new Dictionary<string, StubMock>();
            //Clients = dict;

            //dict["SignalRId_0"] = new StubMock();
        }

        /// <summary>
        /// Gets a dynamic object that represents all clients connected to the hub.
        /// </summary>
        public dynamic Clients { get; private set; }

        /// <summary>
        /// Gets the <see cref="T:SignalR.IGroupManager"/> the hub.
        /// </summary>
        public IGroupManager Groups { get; private set; }
    }

    //internal class StubMock
    //{
    //    public Action<string, string, string, string, string> expressionResponse = (clientCtx, expression, resultValue, errorCode, errorDescription) => { };
    //}

}
