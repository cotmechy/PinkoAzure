using System.Collections.Generic;
using PinkDao;
using PinkoCommon.Interface.Storage;

namespace PinkoAzureService
{
    public class AzureTableStorage : IPinkoStorage
    {
        /// <summary>
        /// Save current expression
        /// </summary>
        public bool SaveExpression(string bucket, string partition, string rowKey, PinkoMsgCalculateExpression expression)
        {
            var key = bucket + partition + rowKey;
            var exists = MockStorage.ContainsKey(key);

            MockStorage[key] = expression;

            return exists;
        }

        /// <summary>
        /// Remove Expression
        /// </summary>
        public bool RemoveExpression(string bucket, string partition, string rowKey, PinkoMsgCalculateExpression expression)
        {
            //var key = bucket + partition + rowKey;
            //var exists = MockStorage.ContainsKey(key);

            //MockStorage[key] = expression;

            return true;
        }

        /// <summary>
        /// Mock Storage
        /// </summary>
        public Dictionary<string, PinkoMsgCalculateExpression> MockStorage = new Dictionary<string, PinkoMsgCalculateExpression>();

    }
}
