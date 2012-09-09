using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinkoCommon.Interface;
using PinkoExpressionCommon;

namespace PinkoWorkerCommon.Providers
{
    public class PinkoMarketEnvManager : IPinkoMarketEnvManager
    {
        /// <summary>
        /// Get market environment
        /// </summary>
        public PinkoMarketEnvManager()
        {
            PinkoMarketEnv = new PinkoMarketEnvironment();
        }

        /// <summary>
        /// Get market environment
        /// </summary>
        public IPinkoMarketEnvironment GetMarketEnv(string marketEvnId)
        {
            return PinkoMarketEnv;
        }

        //[Dependency]
        public IPinkoMarketEnvironment PinkoMarketEnv { get; set; }
    }
}
