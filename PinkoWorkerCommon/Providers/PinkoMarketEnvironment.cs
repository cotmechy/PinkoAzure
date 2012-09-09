using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinkoExpressionCommon;

namespace PinkoWorkerCommon.Providers
{
    public class PinkoMarketEnvironment : IPinkoMarketEnvironment
    {
        /// <summary>
        /// Constructor - PinkoMarketEnvironment 
        /// </summary>
        public PinkoMarketEnvironment()
        {
            PinkoDataAccessLayer = new PinkoDataAccessLayer();
        }

        public string MarketEnvId
        {
            get { return MockMarketEnvId; }
        }

        public IPinkoDataAccessLayer PinkoDataAccessLayer { get; set; }
        public static string MockMarketEnvId = "MockMarketEnvId";
    }
}
