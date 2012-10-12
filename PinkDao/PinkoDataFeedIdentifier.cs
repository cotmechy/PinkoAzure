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

        // Previous Ids
        public string PreviousWebRoleId { get; set; }
        public string PreviousSignalRId { get; set; }
    }


    /// <summary>
    /// PinkoDataFeedIdentifierExtensions
    /// </summary>
    public static class PinkoDataFeedIdentifierExtensions
    {
        /// <summary>
        /// Checks values are the same by comparing internal values.
        /// </summary>
        /// <param name="dest"> </param>
        /// <param name="src"></param>
        /// <returns>
        /// True: they are equal
        /// False: they not are equal
        /// </returns>
        public static bool IsEqual(this PinkoDataFeedIdentifier dest, PinkoDataFeedIdentifier src)
        {
            return dest.ClientId.Equals(src.ClientId)
                   &&
                   dest.MaketEnvId.Equals(src.MaketEnvId)
                   &&
                   dest.SignalRId.Equals(src.SignalRId)
                   &&
                   dest.WebRoleId.Equals(src.WebRoleId)
                   &&
                   dest.SubscribtionId.Equals(src.SubscribtionId)
                ;
        }


        /// <summary>
        /// CopyTo
        /// </summary>
        public static PinkoDataFeedIdentifier DeepClone(this PinkoDataFeedIdentifier src)
        {
            return src.CopyTo(new PinkoDataFeedIdentifier());
        }


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
            dest.PreviousSignalRId = src.PreviousSignalRId;

            return dest;
        }

        /// <summary>
        /// PinkoDataFeedIdentifier
        /// </summary>
        public static string Verbose(this PinkoDataFeedIdentifier obj)
        {
            return string.Format("PinkoDataFeedIdentifier: ClientId: {0} - " +
                                 "SignalRId: {1} - " +
                                 "WebRoleId: {2} - " +
                                 "SubscribtionId: {3} - " +
                                 "ClientCtx: {4} - " +
                                 "MaketEnvId: {7} - " +
                                 "PreviousWebRoleId: {5} - " +
                                 "PreviousSignalRId: {6}",
                                        obj.ClientId,
                                        obj.SignalRId,
                                        obj.WebRoleId,
                                        obj.SubscribtionId,
                                        obj.ClientCtx,
                                        obj.PreviousWebRoleId,
                                        obj.PreviousWebRoleId,
                                        obj.MaketEnvId
                                );
        }
    }

}
