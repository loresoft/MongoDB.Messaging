using System;

namespace MongoDB.Messaging.SignalR
{
    /// <summary>
    /// An <see langword="interface"/> for sending <see cref="ChangeNotification"/> messages
    /// </summary>
    public interface IChangeNotificationClient
    {
        /// <summary>
        /// Send the specified <paramref name="notification"/> message.
        /// </summary>
        /// <param name="notification">The notification messages.</param>
        void SendChange(ChangeNotification notification);
    }
}