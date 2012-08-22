using PinkoCommon;
using PinkoCommon.Interface;

namespace PinkoWebRoleCommon.Extensions
{
    /// <summary>
    /// IPinkoApplicationExtensions
    /// </summary>
    public static class PinkoApplicationExtensions
    {
        /// <summary>
        /// FactorWebEnvelop
        /// </summary>
        public static PinkoServiceMessageEnvelop FactorWebEnvelop(this IPinkoApplication obj, string clientId, string webRoleId)
        {
            var envelop = new PinkoServiceMessageEnvelop(obj);

            envelop.PinkoProperties[PinkoMessagePropTag.ClientId] = clientId;
            envelop.PinkoProperties[PinkoMessagePropTag.WebRoleId] = webRoleId;

            return envelop;
        }

        /// <summary>
        /// FactorWebEnvelop
        /// </summary>
        public static PinkoServiceMessageEnvelop FactorWebEnvelop(this IPinkoApplication obj, string clientId, string webRoleId, object msg)
        {
            var envelop = obj.FactorWebEnvelop(clientId, webRoleId);

            envelop.Message = msg;

            return envelop;
        }
    }


}
