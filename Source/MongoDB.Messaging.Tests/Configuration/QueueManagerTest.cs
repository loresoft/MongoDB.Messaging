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
            string nameToListen = "test-queue-listen", nameToWrite = "test-queue-write";

            var manager = new QueueManager();
            var q = manager.Load(nameToListen, nameToWrite);

            q.Should().NotBeNull();
            q.NameToListen.Should().Be(nameToListen);
            q.NameToWrite.Should().Be(nameToWrite);

            q.Configuration.Should().NotBeNull();
            q.RepositoryToListen.Should().NotBeNull();
            q.RepositoryToWrite.Should().NotBeNull();

            manager.Queues.Count.Should().Be(1);
        }
    }
}