using Microsoft.Practices.Unity;
using PinkoCommon.Interface;
using PinkoExpressionCommon;

namespace PinkoMocks
{
    public class PinkoMarketEnvManagerMock : IPinkoMarketEnvManager
    {

        /// <summary>
        /// Constructor - PinkoMarketEnvManagerMock 
        /// </summary>
        public PinkoMarketEnvManagerMock()
        {
            PinkoMarketEnv = new PinkoMarketEnvironmentMock();
        }

        /// <summary>
        /// Get market environment
        /// </summary>
        public IPinkoMarketEnvironment GetMarketEnv(string marketEvnId)
        {
            return PinkoMarketEnv;
        }

        [Dependency]
        public IPinkoMarketEnvironment PinkoMarketEnv { get; set; }
    }
}
