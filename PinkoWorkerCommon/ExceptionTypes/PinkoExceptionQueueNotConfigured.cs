using System;

namespace PinkoWorkerCommon.ExceptionTypes
{
    /// <summary>
    /// Missing Queue/topic configuration
    /// </summary>
    public class PinkoExceptionQueueNotConfigured : Exception
    {
        /// <summary>
        /// Constructor - PinkoQueueNotConfiguredException 
        /// </summary>
        public PinkoExceptionQueueNotConfigured(string queueName)
            : base(string.Format("Queue '{0}' not configured. Add queue configuration in namespace PinkoServices.PinkoConfiguration QueueConfiguration", queueName))
        {
        }
    }
}
