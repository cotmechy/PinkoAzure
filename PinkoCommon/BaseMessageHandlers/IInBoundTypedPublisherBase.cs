using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinkoCommon.Interface;

namespace PinkoCommon.BaseMessageHandlers
{
    /// <summary>
    /// Generig
    /// </summary>
    public interface IInBoundTypedPublisherBase
    {
        /// <summary>
        /// Publish message into bus
        /// </summary>
        /// <returns></returns>
        void Publish(IBusMessageInbound message);
    }
}
