using System;
using System.Threading;
using FluentAssertions;
using MongoDB.Driver;
using MongoDB.Messaging.Locks;
using MongoDB.Messaging.Storage;
using Xunit;

namespace MongoDB.Messaging.Tests.Locks
{
    public class ThrottleLockTests
    {
        [Fact]
        public void AcquireBlock()
        {

            var lockName = "AcquireBlock" + DateTime.Now.Ticks;

            var collection = GetCollection();
            collection.Should().NotBeNull();

            var locker = new ThrottleLock(collection, TimeSpan.FromMinutes(5));
            locker.Should().NotBeNull();

            var result = locker.Acquire(lockName);
            result.Should().BeTrue();

            var status = locker.Status(lockName);
            status.Should().NotBeNull();
            status.IsLocked.Should().BeTrue();

            var blocked = locker.Acquire(lockName);
            blocked.Should().BeFalse();

        }

        [Fact]
        public void AcquireExpire()
        {

            var lockName = "AcquireExpire" + DateTime.Now.Ticks;

            var collection = GetCollection();
            collection.Should().NotBeNull();

            var locker = new ThrottleLock(collection);
            locker.Should().NotBeNull();

            var result = locker.Acquire(lockName, TimeSpan.FromMilliseconds(5));
            result.Should().BeTrue();

            var status = locker.Status(lockName);
            status.Should().NotBeNull();
            status.IsLocked.Should().BeTrue();

            // wait for expire
            Thread.Sleep(5);

            var blocked = locker.Acquire(lockName, TimeSpan.FromMilliseconds(5));
            blocked.Should().BeTrue();

        }

        [Fact]
        public void AcquireRelease()
        {

            var lockName = "AcquireRelease" + DateTime.Now.Ticks;

            var collection = GetCollection();
            collection.Should().NotBeNull();

            var locker = new ThrottleLock(collection, TimeSpan.FromMinutes(5));
            locker.Should().NotBeNull();

            var result = locker.Acquire(lockName);
            result.Should().BeTrue();

            var status = locker.Status(lockName);
            status.Should().NotBeNull();
            status.IsLocked.Should().BeTrue();

            locker.Release(lockName);

            status = locker.Status(lockName);
            status.Should().NotBeNull();
            status.IsLocked.Should().BeFalse();
        }

        private static IMongoCollection<LockData> GetCollection()
        {
            var database = MongoFactory.GetDatabaseFromConnectionName("Messaging");
            var collection = database.GetCollection<LockData>("throttle-lock");

            return collection;
        }

    }
}