using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinkoExpressionCommon;

namespace PinkoCommon.Extensions
{
    /// <summary>
    /// IPinkoMarketEnvironmentExtensions
    /// </summary>
    public static class PinkoMarketEnvironmentExtensions
    {
        /// <summary>
        /// Clone market environment
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IPinkoMarketEnvironment CloneEnv(this IPinkoMarketEnvironment obj)
        {
            // TODO:
            var clone = new PinkoMarketEnvironment()
                {
                    MarketEnvId = obj.MarketEnvId,
                    PinkoDataAccessLayer = obj.PinkoDataAccessLayer
                };

            return clone;
        }

        ///// <summary>
        ///// IPinkoMarketEnvironment
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public static string Verbose(this IPinkoMarketEnvironment obj)
        //{
        //    return string.Format("IPinkoMarketEnvironment: ", "");
        //}
    }
}
