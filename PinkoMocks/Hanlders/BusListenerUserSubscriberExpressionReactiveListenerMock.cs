using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinkDao;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.Interface;

namespace PinkoMocks.Hanlders
{
    public class BusListenerUserSubscriberExpressionReactiveListenerMock : InboundMessageReactiveListener<PinkoMsgCalculateExpression>
    {
        /// <summary>
        /// Handler
        /// </summary>
        public  IBusMessageOutbound ProcessRequest(IBusMessageInbound msg, PinkoMsgCalculateExpression expression)
        {
            throw new NotImplementedException();
        }
    }
}
