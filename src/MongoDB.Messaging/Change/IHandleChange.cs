using System;
using System.Threading.Tasks;

namespace MongoDB.Messaging.Change
{
    /// <summary>
    /// An <see langword="interface"/> to implement to receive change notifications.
    /// </summary>
    public interface IHandleChange
    {
        /// <summary>
        /// Handle a MongoDB change record.
        /// </summary>
        /// <param name="change">The change record.</param>
        /// <remarks>
        /// This method is called on a thread-pool background thread.
        /// </remarks>
        void HandleChange(ChangeRecord change);
    }
}