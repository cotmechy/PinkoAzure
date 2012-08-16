using PinkoCommon.Interface;
using PinkoExpressionCommon;

namespace PinkoMocks
{
    public class PinkoMarketEnvManagerMock : IPinkoMarketEnvManager
    {
        /// <summary>
        /// Get market environment
        /// </summary>
        public IPinkoMarketEnvironment GetMarketEnv(string marketEvnId)
        {
            return PinkoMarketEnv;
        }

        public IPinkoMarketEnvironment PinkoMarketEnv { get; set; }
    }
}
