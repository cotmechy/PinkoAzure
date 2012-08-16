using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinkoExpressionCommon;

namespace PinkoMocks
{
    public class PinkoMarketEnvironmentMock : IPinkoMarketEnvironment
    {
        /// <summary>
        /// Constructor - PinkoMarketEnvironmentMock 
        /// </summary>
        public PinkoMarketEnvironmentMock()
        {
            PinkoDataAccessLayer = new PinkoDataAccessLayerMock();
        }

        public string MarketEnvId
        {
            get { return MockMarketEnvId; }
        }

        public IPinkoDataAccessLayer PinkoDataAccessLayer { get; set; }
        public static string MockMarketEnvId = "MockMarketEnvId";
    }
}
