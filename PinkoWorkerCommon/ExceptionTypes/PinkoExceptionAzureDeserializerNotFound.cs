using System;

namespace PinkoWorkerCommon.ExceptionTypes
{
    /// <summary>
    /// Broker message deserializer not found
    /// </summary>
    public class PinkoExceptionAzureDeserializerNotFound : Exception
    {
        /// <summary>
        /// Constructor - PinkoExceptionAzureDeserializerNotFound 
        /// </summary>
        public PinkoExceptionAzureDeserializerNotFound(string type)
            : base(string.Format("Message '{0}' Azure Brokered message not found. Add typed deserializer in AzureQueueClientExtensions.TypeDeserializerdict.",type))
        {
        }
    }
}
