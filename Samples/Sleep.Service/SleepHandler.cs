using System;
using System.Threading;
using MongoDB.Messaging;
using MongoDB.Messaging.Subscription;
using Sleep.Messages;

namespace Sleep.Service
{
    public class SleepHandler : IMessageSubscriber
    {
        public MessageResult Process(ProcessContext context)
        {
            // get message data
            var sleepMessage = context.Data<SleepMessage>();

            Console.WriteLine("Sleep Messeage Received: '{0}', Time: {1}, Id: {2}", 
                sleepMessage.Text, sleepMessage.Time, context.Message.Id);

            Thread.Sleep(sleepMessage.Time);

            if (sleepMessage.Throw)
                throw new InvalidOperationException("This is a test sleep exception");

            return MessageResult.Successful;
        }

        public void Dispose()
        {

        }
    }
}