using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Messaging.Security;

namespace MongoDB.Messaging.Locks
{
    /// <summary>
    /// Throttle Lock Manager controls how frequent a process can run. 
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class ThrottleLock : LockManager
    {
        private readonly Lazy<int> _process;


        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedLock"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public ThrottleLock(IMongoCollection<LockData> collection)
            : this(collection, TimeSpan.FromMinutes(5))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedLock"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="lockExpiration">The default amount of time before the lock will expire and be free to be acquired again.</param>
        public ThrottleLock(IMongoCollection<LockData> collection, TimeSpan lockExpiration)
            : base(collection, lockExpiration)
        {
            // cache process id
            _process = new Lazy<int>(() => Process.GetCurrentProcess().Id);
        }


        /// <summary>
        /// Acquire a lock with the specified <paramref name="name" /> and <paramref name="expiration" />.
        /// </summary>
        /// <param name="name">The name of the lock.</param>
        /// <param name="expiration">The amount of time before the lock will expire and be free to be acquired again.</param>
        /// <returns>
        ///   <c>true</c> if the lock was acquired; otherwise <c>false</c>
        /// </returns>
        public override bool Acquire(string name, TimeSpan expiration)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            VerifyLock(name);

            // all locks that are expired
            Expression<Func<LockData, bool>> filter = m => (m.Id == name)
                && (m.Expire == null || m.Expire < DateTime.UtcNow);

            var update = Builders<LockData>.Update
                .Set(m => m.UserName, UserHelper.Current())
                .Set(m => m.MachineName, Environment.MachineName)
                .Set(m => m.Process, _process.Value)
                .Set(p => p.IsLocked, true)
                .Set(p => p.Updated, DateTime.UtcNow)
                .Set(p => p.Expire, DateTime.UtcNow.Add(expiration));

            var options = new FindOneAndUpdateOptions<LockData, LockData>
            {
                IsUpsert = false,
                ReturnDocument = ReturnDocument.After
            };


            var instance = Collection.FindOneAndUpdateAsync(filter, update, options).Result;
            return instance?.IsLocked == true;
        }


        /// <summary>
        /// Release a lock with the specified <paramref name="name"/>.  Note, release is typically not used on a throttle lock.
        /// </summary>
        /// <param name="name">The name of the lock.</param>
        public override void Release(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var builder = Builders<LockData>.Filter;
            var filter = builder.Eq(m => m.Id, name);

            var update = Builders<LockData>.Update
                .Set(p => p.IsLocked, false)
                .Set(p => p.Updated, DateTime.UtcNow)
                .Unset(p => p.Expire);

            var options = new UpdateOptions { IsUpsert = false };

            Collection.UpdateOneAsync(filter, update, options).Wait();
        }
    }
}