using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkDao
{
    /// <summary>
    /// Idenitifers related to the data feed
    /// </summary>
    public struct PinkoDataFeedIdentifier
    {
        public string ClientId;
        public string RoutingId;
        public string SubscribtionId;
        public string CalEngineId;
    }

    /// <summary>
    /// PinkoDataFeedIdentifierExtensions
    /// </summary>
    public static class PinkoDataFeedIdentifierExtensions
    {

        /// <summary>
        /// PinkoDataFeedIdentifier
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Verbose(this PinkoDataFeedIdentifier obj)
        {
            return string.Format("PinkoDataFeedIdentifier: ClientId: {0} - RoutingId: {1} - CalEngineId: {2} - SubscribtionId: {3}",
                                        obj.ClientId, 
                                        obj.RoutingId,
                                        obj.CalEngineId,
                                        obj.SubscribtionId
                                );
        }
    }

}
