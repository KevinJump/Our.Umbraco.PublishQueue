using Our.Umbraco.PublishQueue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Our.Umbraco.PublishQueue.Persisitance
{
    public class PublishQueueRepository : IPublishQueueRepository
    {
        private readonly object _lock = new object();
        private readonly DatabaseContext _dbContext;
        private readonly ILogger _logger; 

        public PublishQueueRepository(DatabaseContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public int Count()
        {
            using(var db = _dbContext.Database)
            {
                return db.ExecuteScalar<int>("SELECT COUNT(*) FROM PublishQueue;");
            }
        }

        public QueuedItem Dequeue()
        {
            lock(_lock)
            {
                using (var db = _dbContext.Database)
                {
                    var item = db.FirstOrDefault<QueuedItem>("SELECT * FROM PublishQueue WHERE schedule < @0 ORDER BY PRIORITY DESC, SUBMITTED, schedule;", DateTime.Now);
                    // var item = db.FirstOrDefault<QueuedItem>("SELECT * FROM PublishQueue ORDER BY Submitted");
                    if (item != null)
                    {
                        db.Delete<QueuedItem>(item.Id);
                    }

                    return item;
                }
            }
        }

        public QueuedItem Enqueue(QueuedItem item, bool insertIfExists = false) {

            lock (_lock)
            {
                using(var db = _dbContext.Database)
                {
                    if (insertIfExists || !Contains(item.NodeKey))
                    {
                        if (item.Schedule == DateTime.MinValue)
                            item.Schedule = DateTime.Now;

                        item.Attempt++;
                        db.Insert(item);
                    }
                    return item;
                }
            }
        }

        public bool Contains(Guid key)
        {
            using(var db = _dbContext.Database)
            {
                var item = db.FirstOrDefault<QueuedItem>("SELECT * FROM PublishQueue WHERE NodeKey = @0", key);
                return (item != null);
            }
        }

        public void Clear()
        {
            lock(_lock)
            {
                using (var db = _dbContext.Database)
                {
                    foreach(var item in List())
                    {
                        db.Delete(item);
                    }
                }
            }

        }

        public IEnumerable<QueuedItem> List()
        {
            using (var db = _dbContext.Database)
            {
                return db.Fetch<QueuedItem>("SELECT * FROM PublishQueue ORDER BY PRIORITY DESC, SUBMITTED, schedule;");
                // return db.Fetch<QueuedItem>("SELECT * FROM PublishQueue ORDER BY Submitted");
            }

        }

        public IEnumerable<QueuedItem> List(int limit)
        {
            using (var db = _dbContext.Database)
            {
                return db.Fetch<QueuedItem>("SELECT TOP(@0) * FROM PublishQueue ORDER BY PRIORITY DESC, SUBMITTED, schedule;", limit);
                // return db.Fetch<QueuedItem>("SELECT * FROM PublishQueue ORDER BY Submitted");
            }

        }
    }
}
