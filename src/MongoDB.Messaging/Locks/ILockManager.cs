using System;

namespace MongoDB.Messaging.Locks
{
    /// <summary>
    /// An <see langword="interface"/> defining a MongoDB lock manager.
    /// </summary>
    public interface ILockManager
    {
        /// <summary>
        /// Acquire a lock with the specified <paramref name="name" />.
        /// </summary>
        /// <param name="name">The name of the lock.</param>
        /// <returns><c>true</c> if the lock was acquired; otherwise <c>false</c></returns>
        bool Acquire(string name);

        /// <summary>
        /// Acquire a lock with the specified <paramref name="name" /> and <paramref name="expiration" />.
        /// </summary>
        /// <param name="name">The name of the lock.</param>
        /// <param name="expiration">The amount of time before the lock will expire and be free to be acquired again.</param>
        /// <returns><c>true</c> if the lock was acquired; otherwise <c>false</c></returns>
        bool Acquire(string name, TimeSpan expiration);

        /// <summary>
        /// Gets the status of a lock the specified <paramref name="name" />.
        /// </summary>
        /// <param name="name">The name of the lock.</param>
        /// <returns>An object with status data about the lock.</returns>
        LockData Status(string name);

        /// <summary>
        /// Renew the expiration of a lock with the specified <paramref name="name" />.
        /// </summary>
        /// <param name="name">The name of the lock.</param>
        void Renew(string name);

        /// <summary>
        /// Renew the expiration of a lock with the specified <paramref name="name" /> and <paramref name="expiration" />.
        /// </summary>
        /// <param name="name">The name of the lock.</param>
        /// <param name="expiration">The amount of time before the lock will expire and be free to be acquired again.</param>
        void Renew(string name, TimeSpan expiration);

        /// <summary>
        /// Release a lock with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the lock.</param>
        void Release(string name);
    }
}