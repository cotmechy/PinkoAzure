using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkDao
{
    /// <summary>
    /// Client ping message 
    /// </summary>
    public class PinkoMsgClientPing
    {
        public DateTime PingTime = DateTime.Now;
        public PinkoDataFeedIdentifier DataFeedIdentifier = new PinkoDataFeedIdentifier();
    }
}
