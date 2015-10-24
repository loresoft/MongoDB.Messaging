using System;
using MongoDB.Driver;
using MongoDB.Messaging.Security;

namespace MongoDB.Messaging.Locks
{
    /// <summary>
    /// An <see langword="abstract"/> implementation of <see cref="ILockManager"/>.
    /// </summary>
    public abstract class LockManager : ILockManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedLock"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="lockExpiration">The default amount of time before the lock will expire and be free to be acquired again.</param>
        protected LockManager(IMongoCollection<LockData> collection, TimeSpan lockExpiration)
        {
            Collection = collection;
            LockExpiration = lockExpiration;
        }

        /// <summary>
        /// Gets or sets the default amount of time before the lock will expire and be free to be acquired again
        /// </summary>
        /// <value>
        /// The default amount of time before the lock will expire and be free to be acquired again
        /// </value>
        public TimeSpan LockExpiration { get; set; }

        /// <summary>
        /// Gets the underlying MongoDB collection for storing lock data.
        /// </summary>
        /// <value>
        /// The underlying MongoDB collection for storing lock data.
        /// </value>
        protected IMongoCollection<LockData> Collection { get; }

        /// <summary>
        /// Acquire a lock with the specified <paramref name="name" />.
        /// </summary>
        /// <param name="name">The name of the lock.</param>
        /// <returns>
        ///   <c>true</c> if the lock was acquired; otherwise <c>false</c>
        /// </returns>
        public bool Acquire(string name)
        {
            return Acquire(name, LockExpiration);
        }

        /// <summary>
        /// Acquire a lock with the specified <paramref name="name" /> and <paramref name="expiration" />.
        /// </summary>
        /// <param name="name">The name of the lock.</param>
        /// <param name="expiration">The amount of time before the lock will expire and be free to be acquired again.</param>
        /// <returns>
        ///   <c>true</c> if the lock was acquired; otherwise <c>false</c>
        /// </returns>
        public abstract bool Acquire(string name, TimeSpan expiration);

        /// <summary>
        /// Gets the status of a lock the specified <paramref name="name" />.
        /// </summary>
        /// <param name="name">The name of the lock.</param>
        /// <returns></returns>
        public virtual LockData Status(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var instance = Collection
                .Find(m => m.Id == name)
                .FirstOrDefaultAsync()
                .Result;

            return instance;
        }

        /// <summary>
        /// Renew the expiration of a lock with the specified <paramref name="name" />.
        /// </summary>
        /// <param name="name">The name of the lock.</param>
        /// <returns></returns>
        public void Renew(string name)
        {
            Renew(name, LockExpiration);
        }

        /// <summary>
        /// Renew the expiration of a lock with the specified <paramref name="name" /> and <paramref name="expiration" />.
        /// </summary>
        /// <param name="name">The name of the lock.</param>
        /// <param name="expiration">The amount of time before the lock will expire and be free to be acquired again.</param>
        /// <returns></returns>
        public virtual void Renew(string name, TimeSpan expiration)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var builder = Builders<LockData>.Filter;
            var filter = builder.Eq(m => m.Id, name);

            var update = Builders<LockData>.Update
                .Set(p => p.Updated, DateTime.UtcNow)
                .Set(p => p.Expire, DateTime.UtcNow.Add(expiration));

            var options = new UpdateOptions { IsUpsert = false };

            Collection.UpdateOneAsync(filter, update, options).Wait();
        }

        /// <summary>
        /// Release a lock with the specified <paramref name="name"/>.  Note, release is typically not used on a throttle lock.
        /// </summary>
        /// <param name="name">The name of the lock.</param>
        public abstract void Release(string name);

        /// <summary>
        /// Verifies the lock record exists.
        /// </summary>
        /// <param name="name">The name of the lock.</param>
        protected virtual void VerifyLock(string name)
        {
            var builder = Builders<LockData>.Filter;
            var filter = builder.Eq(m => m.Id, name);

            // only set on insert
            var update = Builders<LockData>.Update
                .SetOnInsert(p => p.Id, name)
                .SetOnInsert(p => p.IsLocked, false)
                .SetOnInsert(p => p.Created, DateTime.UtcNow)
                .SetOnInsert(p => p.Updated, DateTime.UtcNow);

            var options = new UpdateOptions { IsUpsert = true };

            Collection.UpdateOneAsync(filter, update, options).Wait();
        }
    }
}