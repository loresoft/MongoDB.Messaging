using System;
using System.Linq.Expressions;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Messaging.Storage;
using Xunit;

namespace MongoDB.Messaging.Tests.Storage
{
    
    public class QueueRepositoryTest
    {
        [Fact]
        public async void SaveFind()
        {
            var repo = GetRepository();

            var message = new Message();
            message.Name = "SaveFind";

            message.Id.Should().BeNullOrEmpty();

            var m = await repo.Save(message);
            m.Should().Be(message);

            message.Id.Should().NotBeNullOrEmpty();

            string key = message.Id;

            var one = await repo.Find(key);
            one.Id.Should().Be(key);
        }

        [Fact]
        public async void FindOne()
        {
            var repo = GetRepository();
            var message = new Message();
            message.Name = "FindOne";

            await repo.Save(message);

            Expression<Func<Message, bool>> criteria = (m) => m.State == MessageState.None && m.Result == MessageResult.None;

            var one = await repo.FindOne(criteria);
            one.Should().NotBeNull();
        }

        [Fact]
        public async void FindAll()
        {
            var repo = GetRepository();
            var message = new Message();
            message.Name = "FindAll";

            await repo.Save(message);

            Expression<Func<Message, bool>> criteria = (m) => m.State == MessageState.None && m.Result == MessageResult.None;

            var list = await repo.FindAll(criteria);
            list.Should().NotBeNullOrEmpty();
        }


        [Fact]
        public async void SaveDelete()
        {
            var repo = GetRepository();

            var message = new Message();
            message.Name = "SaveDelete";

            message.Id.Should().BeNullOrEmpty();

            var m = await repo.Save(message);
            m.Should().Be(message);

            message.Id.Should().NotBeNullOrEmpty();

            string key = message.Id;

            long count = await repo.Delete(key);
            count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async void DeleteOne()
        {
            var repo = GetRepository();
            var message = new Message();
            message.Name = "DeleteOne";

            await repo.Save(message);

            Expression<Func<Message, bool>> criteria = (m) => m.State == MessageState.None && m.Result == MessageResult.None;

            long count = await repo.DeleteOne(criteria);
            count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async void DeleteAll()
        {
            var repo = GetRepository();
            var message = new Message();
            message.Name = "DeleteAll";

            await repo.Save(message);

            Expression<Func<Message, bool>> criteria = (m) => m.State == MessageState.None && m.Result == MessageResult.None;

            long count = await repo.DeleteAll(criteria);
            count.Should().BeGreaterThan(0);
        }


        [Fact]
        public async void Count()
        {
            var repo = GetRepository();
            var message = new Message();
            message.Name = "Count";

            await repo.Save(message);

            Expression<Func<Message, bool>> criteria = (m) => m.State == MessageState.None && m.Result == MessageResult.None;

            long count = await repo.Count(criteria);
            count.Should().BeGreaterThan(0);
        }


        [Fact]
        public async void SaveWithNoId()
        {
            var repo = GetRepository();

            var message = new Message();
            message.Name = "SaveWithNoId";

            message.Id.Should().BeNullOrEmpty();

            var m = await repo.Save(message);
            m.Should().Be(message);

            message.Id.Should().NotBeNullOrEmpty();
        }


        [Fact]
        public async void SaveWithNewId()
        {
            var repo = GetRepository();

            var message = new Message();
            message.Id = ObjectId.GenerateNewId().ToString();
            message.Name = "SaveWithNewId";

            var m = await repo.Save(message);
            m.Should().Be(message);

            message.Id.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async void SaveWithNewIdThenUpdate()
        {
            var repo = GetRepository();

            var message = new Message();
            message.Name = "SaveWithNewIdThenUpdate";

            message.Id.Should().BeNullOrEmpty();

            // create
            var m = await repo.Save(message);
            m.Should().Be(message);

            message.Id.Should().NotBeNullOrEmpty();

            message.Description = "updated message";

            // update
            await repo.Save(message);

        }


        [Fact]
        public async void EnqueueDequeue()
        {
            var repo = GetRepository();

            var message = new Message();
            message.Name = "EnqueueDequeue";

            var m = await repo.Enqueue(message);
            m.Should().Be(message);

            message.Id.Should().NotBeNullOrEmpty();
            message.State.Should().Be(MessageState.Queued);

            string key = message.Id;

            var dequeued = await repo.Dequeue();
            dequeued.Should().NotBeNull();
            dequeued.Id.Should().Be(key);
            dequeued.State.Should().Be(MessageState.Processing);
            dequeued.StartTime.Should().BeAfter(DateTime.MinValue);
            dequeued.Status.Should().NotBeNullOrEmpty();


            await repo.UpdateStatus(key, "Loading Test Data ...");

            await repo.MarkComplete(key, MessageResult.Successful, "Test Complete");

            var completed = await repo.Find(key);
            completed.State.Should().Be(MessageState.Complete);
            completed.Result.Should().Be(MessageResult.Successful);
            completed.EndTime.Should().BeAfter(DateTime.MinValue);
            completed.Status.Should().NotBeNullOrEmpty();
        }


        private static QueueRepository GetRepository()
        {
            var database = MongoFactory.GetDatabaseFromConnectionName("Messaging");
            var collection = database.GetCollection<Message>("test-queue");
            var repo = new QueueRepository(collection);
            return repo;
        }
    }
}