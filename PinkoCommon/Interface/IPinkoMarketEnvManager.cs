
using PinkoExpressionCommon;

namespace PinkoCommon.Interface
{
    /// <summary>
    /// Market Environment manager
    /// </summary>
    public interface IPinkoMarketEnvManager
    {
        /// <summary>
        /// Get market environment
        /// </summary>
        IPinkoMarketEnvironment GetMarketEnv(string marketEvnId);
    }
}
