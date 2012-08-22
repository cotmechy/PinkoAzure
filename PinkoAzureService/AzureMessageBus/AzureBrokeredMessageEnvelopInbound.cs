using Microsoft.Practices.ObjectBuilder2;
using Microsoft.ServiceBus.Messaging;
using PinkoCommon;
using PinkoCommon.Interface;
using PinkoWorkerCommon.Utility;

namespace PinkoAzureService.AzureMessageBus
{
    public class AzureBrokeredMessageEnvelopInbound : PinkoServiceMessageEnvelop
    {
        /// <summary>
        /// Constructor - AzureBrokeredMessage 
        /// </summary>
        public AzureBrokeredMessageEnvelopInbound(IPinkoApplication pinkoApplication, BrokeredMessage azureMsg)
            : base(pinkoApplication)
        {
            _message = azureMsg;

            Message = _message.GetBody();
            ReplyTo = string.IsNullOrEmpty(_message.ReplyTo) ? string.Empty : _message.ReplyTo;

            azureMsg.Properties.ForEach(x => PinkoProperties[x.Key] = x.Value.ToString());
        }

        /// <summary>
        /// Original Azure broker message
        /// </summary>
        private readonly BrokeredMessage _message;
    }
}
