using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkDao
{
    /// <summary>
    /// Holds static collection to help test that types are managed properly
    /// </summary>
    public static class PinkoMessageTypes
    {
        public static Type[] SupportedTypes = new[]
            {
                typeof(PinkoMsgClientConnect), 
                typeof(PinkoMsgClientPing),
                typeof(PinkoMsgPing),
                typeof(PinkoMsgRoleHeartbeat),
                typeof(PinkoMsgCalculateExpression),
                typeof(PinkoMsgCalculateExpressionResult),
                typeof(PinkoMsgClientTimeout)

            };
    }
}
