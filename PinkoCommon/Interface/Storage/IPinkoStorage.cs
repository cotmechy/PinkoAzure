using PinkDao;

namespace PinkoCommon.Interface.Storage
{
    /// <summary>
    /// Encapsulate Storage
    /// </summary>
    public interface IPinkoStorage
    {
        /// <summary>
        /// Save current expression
        /// </summary>
        bool SaveExpression(string bucket, string partition, string rowKey, PinkoMsgCalculateExpression expression);
    }
}
