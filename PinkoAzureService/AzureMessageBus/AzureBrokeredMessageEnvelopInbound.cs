using Microsoft.Practices.ObjectBuilder2;
using Microsoft.ServiceBus.Messaging;
using PinkoWorkerCommon.Utility;

namespace PinkoAzureService.AzureMessageBus
{
    public class AzureBrokeredMessageEnvelopInbound : PinkoServiceMessageEnvelop
    {
        /// <summary>
        /// Constructor - AzureBrokeredMessage 
        /// </summary>
        public AzureBrokeredMessageEnvelopInbound(BrokeredMessage azureMsg)
        {
            _message = azureMsg;

            Message = AzureQueueClientExtensions.GetBody(_message);
            ReplyTo = string.IsNullOrEmpty(_message.ReplyTo) ? string.Empty : _message.ReplyTo;
            ContentType = string.IsNullOrEmpty(_message.ContentType) ? string.Empty : _message.ContentType;

            azureMsg.Properties.ForEach(x => PinkoProperties[x.Key] = x.Value.ToString());
        }

        /// <summary>
        /// Original Azure broker message
        /// </summary>
        private BrokeredMessage _message;
    }
}
