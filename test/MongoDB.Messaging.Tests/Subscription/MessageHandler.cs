using System;
using MongoDB.Messaging.Subscription;

namespace MongoDB.Messaging.Tests.Subscription
{
    public class MessageHandler : IMessageSubscriber
    {
        public MessageResult Process(ProcessContext context)
        {
            return MessageResult.None;
        }

        public void Dispose()
        {
        }
    }
}