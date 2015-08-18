using System;
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
            var manager = new QueueManager();
            var q = manager.Load("test-queue");

            q.Should().NotBeNull();
            q.Name.Should().Be("test-queue");

            q.Configuration.Should().NotBeNull();
            q.Repository.Should().NotBeNull();


            manager.Queues.Count.Should().Be(1);
        }
    }
}
