using System;

namespace PinkoCommon.ExceptionTypes
{
    /// <summary>
    /// Broker message deserializer not found
    /// </summary>
    public class PinkoExceptionMsMqNotFound : Exception
    {
        /// <summary>
        /// Constructor - PinkoExceptionMsMqNotFound 
        /// </summary>
        public PinkoExceptionMsMqNotFound(string msg)
            : base(msg)
        {
        }
    }
}
