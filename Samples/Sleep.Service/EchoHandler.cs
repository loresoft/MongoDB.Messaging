using System;
using System.Threading;
using MongoDB.Messaging;
using MongoDB.Messaging.Subscription;
using Sleep.Messages;

namespace Sleep.Service
{
    public class EchoHandler : IMessageSubscriber
    {
        public MessageResult Process(ProcessContext context)
        {
            // get message data
            var sleepMessage = context.Data<EchoMessage>();

            Console.WriteLine("Echo Received: '{0}', Id: {1}", sleepMessage.Text, context.Message.Id);

            Thread.Sleep(5000);

            if (sleepMessage.Throw)
                throw new InvalidOperationException("This is a test echo exception");

            return MessageResult.Successful;
        }

        public void Dispose()
        {

        }
    }
}