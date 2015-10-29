using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Messaging.Configuration;
using MongoDB.Messaging.Tests.Messages;
using MongoDB.Messaging.Tests.Subscription;
using Xunit;

namespace MongoDB.Messaging.Tests
{
    
    public class MessageQueueTest
    {

        [Fact]
        public void Configure()
        {
            var manager = new QueueManager();
            var messageQueue = new MessageQueue(manager);

            messageQueue.Configure(c => c
                 .Connection("Messaging")
                 .ControlQueue("control-name")
                 .Queue(q => q
                     .Name("queue-name")
                     .Description("description")
                     .Retry(1)
                     .Priority(MessagePriority.Normal)
                     .ResponseQueue("response-name")
                 )
                 .Queue(q => q
                     .Name("queue-blah")
                     .Description("description")
                     .Retry(5)
                     .Priority(MessagePriority.Normal)
                     .ResponseQueue("response-blah")
                 )
                 .Subscribe(s => s
                     .Queue("queue-name")
                     .Handler<MessageHandler>()
                     .PollTime(TimeSpan.FromSeconds(10))
                     .Workers(2)
                 )
                 .Subscribe(s => s
                     .Queue("queue-blah")
                     .PollTime(TimeSpan.FromSeconds(10))
                     .Handler(() => new MessageHandler())
                     .Workers(2)
                     .Timeout(TimeSpan.FromMinutes(30))
                     .TimeoutAction(TimeoutPolicy.Retry)
                 )
             );


            manager.Should().NotBeNull();
            manager.ConnectionName.Should().Be("Messaging");
            manager.ControlName.Should().Be("control-name");

            manager.Queues.Count.Should().Be(2);

            var subscriptions = manager.Subscriptions.ToList();
            subscriptions.Count.Should().Be(2);
        }

        [Fact]
        public async void Publish()
        {
            var userMessage = UserMessage.Tester();

            var message = await MessageQueue.Default.Publish(c => c
                .Queue("queue-name")
                .Name("UserMessage")
                .Description("Update User Data")
                .Data(userMessage)
            );

            message.Id.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async void Schedule()
        {
            var userMessage = UserMessage.Tester();
            var date = DateTime.Now.AddHours(1);

            var message = await MessageQueue.Default.Schedule(c => c
                .Schedule(date)
                .Queue("queue-name")
                .Data(userMessage)
            );

            message.Id.Should().NotBeNullOrEmpty();
            message.State.Should().Be(MessageState.Scheduled);
            message.Scheduled.Should().Be(date);
        }
    }
}
