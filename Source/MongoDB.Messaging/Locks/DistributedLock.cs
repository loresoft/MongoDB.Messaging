using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Messaging.Security;

namespace MongoDB.Messaging.Locks
{
    /// <summary>
    /// Distributed Lock manager provides synchronized access to a resources over a network
    /// </summary>
    public class DistributedLock : LockManager
    {
        private readonly Lazy<int> _process;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedLock"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public DistributedLock(IMongoCollection<LockData> collection)
            : this(collection, TimeSpan.FromMinutes(5))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedLock"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="lockExpiration">The default amount of time before the lock will expire and be free to be acquired again.</param>
        public DistributedLock(IMongoCollection<LockData> collection, TimeSpan lockExpiration)
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
            return Acquire(name, expiration, TimeSpan.Zero);
        }

        /// <summary>
        /// Acquire a lock with the specified <paramref name="name" /> and <paramref name="expiration" />.
        /// </summary>
        /// <param name="name">The name of the lock.</param>
        /// <param name="expiration">The amount of time before the lock will expire and be free to be acquired again.</param>
        /// <param name="wait">The wait.</param>
        /// <returns>
        ///   <c>true</c> if the lock was acquired; otherwise <c>false</c></returns>
        public bool Acquire(string name, TimeSpan expiration, TimeSpan wait)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            VerifyLock(name);

            // all locks that are not locked or expired
            Expression<Func<LockData, bool>> filter = m => (m.Id == name)
                && (m.IsLocked == false || (m.Expire.HasValue && m.Expire < DateTime.UtcNow));

            var update = Builders<LockData>.Update
                .Set(m => m.UserName, UserHelper.Current())
                .Set(m => m.MachineName, Environment.MachineName)
                .Set(m => m.Process, _process.Value)
                .Set(p => p.IsLocked, true)
                .Set(p => p.Updated, DateTime.UtcNow)
                .Set(p => p.Expire, DateTime.UtcNow.Add(expiration));

            var options = new FindOneAndUpdateOptions<LockData, LockData>();
            options.IsUpsert = false;
            options.ReturnDocument = ReturnDocument.After;


            var timeout = DateTime.UtcNow.Add(wait);
            do
            {
                var instance = Collection.FindOneAndUpdateAsync(filter, update, options).Result;
                if (instance?.IsLocked == true)
                    return true;

                // keep trying till timeout
                var delay = (int) (wait.TotalMilliseconds * .1);
                Thread.Sleep(delay);

            } while (timeout > DateTime.UtcNow);


            return false;
        }


        /// <summary>
        /// Release a lock with the specified <paramref name="name"/>.
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
                .Set(p => p.Updated, DateTime.UtcNow);

            var options = new UpdateOptions { IsUpsert = false };

            Collection.UpdateOneAsync(filter, update, options).Wait();
        }
    }
}
