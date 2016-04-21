using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using MongoDB.Messaging.Locks;
using MongoDB.Messaging.Storage;
using Xunit;

namespace MongoDB.Messaging.Tests.Locks
{
    public class DistributedLockTests
    {
        [Fact]
        public void AcquireBlock()
        {

            var lockName = "AcquireBlock" + DateTime.Now.Ticks;

            var collection = GetCollection();
            collection.Should().NotBeNull();

            var locker = new DistributedLock(collection, TimeSpan.FromMinutes(5));
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
        public void AcquireWait()
        {

            var lockName = "AcquireWait" + DateTime.Now.Ticks;

            var collection = GetCollection();
            collection.Should().NotBeNull();

            var locker = new DistributedLock(collection);
            locker.Should().NotBeNull();

            var result = locker.Acquire(lockName, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
            result.Should().BeTrue();

            var status = locker.Status(lockName);
            status.Should().NotBeNull();
            status.IsLocked.Should().BeTrue();

            var waited = locker.Acquire(lockName, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
            waited.Should().BeTrue();

        }

        [Fact]
        public void AcquireExpire()
        {

            var lockName = "AcquireExpire" + DateTime.Now.Ticks;

            var collection = GetCollection();
            collection.Should().NotBeNull();

            var locker = new DistributedLock(collection);
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

            var locker = new DistributedLock(collection, TimeSpan.FromMinutes(5));
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
            var collection = database.GetCollection<LockData>("distributed-lock");

            return collection;
        }

    }
}
