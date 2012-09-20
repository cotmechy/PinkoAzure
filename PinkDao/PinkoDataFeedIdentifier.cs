using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkDao
{
    /// <summary>
    /// Identifiers related to the data feed
    /// </summary>
    public class PinkoDataFeedIdentifier
    {
        public string SubscribtionId { get; set; }
        public string ClientCtx { get; set; }
        public string ClientId { get; set; }
        public string SignalRId { get; set; }
        public string WebRoleId { get; set; }
        public string MaketEnvId { get; set; }
        public string PreviousWebRoleId { get; set; }
    }


    /// <summary>
    /// PinkoDataFeedIdentifierExtensions
    /// </summary>
    public static class PinkoDataFeedIdentifierExtensions
    {
        /// <summary>
        /// CopyTo
        /// </summary>
        public static PinkoDataFeedIdentifier CopyTo(this PinkoDataFeedIdentifier src, PinkoDataFeedIdentifier dest)
        {
            dest.ClientCtx = src.ClientCtx;
            dest.ClientId = src.ClientId;
            dest.MaketEnvId = src.MaketEnvId;
            dest.SignalRId = src.SignalRId;
            dest.SubscribtionId = src.SubscribtionId;
            dest.WebRoleId = src.WebRoleId;
            dest.PreviousWebRoleId = src.PreviousWebRoleId;

            return dest;
        }

        /// <summary>
        /// PinkoDataFeedIdentifier
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Verbose(this PinkoDataFeedIdentifier obj)
        {
            return string.Format("PinkoDataFeedIdentifier: ClientId: {0} - SignalRId: {1} - WebRoleId: {2} - SubscribtionId: {3} - ClientCtx: {4} - PreviousWebRoleId: {5}",
                                        obj.ClientId,
                                        obj.SignalRId,
                                        obj.WebRoleId,
                                        obj.SubscribtionId,
                                        obj.ClientCtx,
                                        obj.PreviousWebRoleId
                                );
        }
    }

}
