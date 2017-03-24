using System.Collections.Generic;
using Our.Umbraco.PublishQueue.Models;

namespace Our.Umbraco.PublishQueue.Persisitance
{
    public interface IPublishQueueRepository
    {
        void Clear();
        int Count();
        QueuedItem Dequeue();
        QueuedItem Enqueue(QueuedItem item, bool insertIfExists);
        IEnumerable<QueuedItem> List();
    }
}