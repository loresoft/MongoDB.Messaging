using System;

namespace QueueBrowser.Notifications
{
    public interface INotifyChange
    {
        void Change(ChangeNotification notification);
    }
}