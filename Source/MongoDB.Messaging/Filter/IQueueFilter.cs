using MongoDB.Driver;
using System.Threading.Tasks;

namespace MongoDB.Messaging.Filter
{
    public interface IQueueFilter
    {
        Task<FilterDefinition<Message>> GetQueueFilterAsync();
    }
}