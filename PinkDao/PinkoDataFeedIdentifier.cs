namespace PinkDao
{
    /// <summary>
    /// Identifiers related to the data feed
    /// </summary>
    public class PinkoDataFeedIdentifier
    {
        public string SubscribtionId = string.Empty;
        public string ClientCtx = string.Empty;
        public string ClientId = string.Empty;
        public string SignalRId = string.Empty;
        public string WebRoleId = string.Empty;
        public string MaketEnvId = string.Empty;

        // Previous Ids
        public string PreviousWebRoleId = string.Empty;
        public string PreviousSignalRId = string.Empty;
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
            if (dest == null && null == src)
                return true;

            if (dest == null || null == src)
                return false;

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
        /// Clone partial fields
        /// </summary>
        public static PinkoDataFeedIdentifier PartialClone(this PinkoDataFeedIdentifier src, PinkoDataFeedIdentifier srcEx)
        {
            var newEx = src.DeepClone();

            newEx.SignalRId = srcEx.SignalRId;
            newEx.WebRoleId = srcEx.WebRoleId;

            newEx.PreviousWebRoleId = srcEx.PreviousWebRoleId;
            newEx.PreviousSignalRId = srcEx.PreviousSignalRId;

            return newEx;
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
