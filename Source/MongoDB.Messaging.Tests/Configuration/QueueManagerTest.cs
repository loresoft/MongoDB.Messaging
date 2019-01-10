using FluentAssertions;
using MongoDB.Messaging.Configuration;
using Xunit;

namespace MongoDB.Messaging.Tests.Configuration
{

    public class QueueManagerTest
    {
        [Fact]
        public void LoadQueueTest()
        {
            // Initialize values 
            string name = "test-queue";

            var manager = new QueueManager();
            var q = manager.Load(name);

            q.Should().NotBeNull();
            q.Name.Should().Be(name);

            q.Configuration.Should().NotBeNull();
            q.Repository.Should().NotBeNull();

            manager.Queues.Count.Should().Be(1);
        }
    }
}