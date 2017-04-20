using Microsoft.AspNet.SignalR;
using Our.Umbraco.PublishQueue.Hubs;
using Our.Umbraco.PublishQueue.Models;
using Our.Umbraco.PublishQueue.Persisitance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Our.Umbraco.PublishQueue.Services
{
    public class PublishQueueService
    {
        private readonly IContentService _contentService;
        private readonly ILogger _logger;
        private readonly PublishQueueRepository _queueRepo;

        private static Object _lock = new object();
        private static bool _processing = false;

        public PublishQueueService(
            DatabaseContext dbContext,
            IContentService contentService,
            ILogger logger)
        {
            _contentService = contentService;
            _logger = logger;
            _queueRepo = new PublishQueueRepository(dbContext, logger);
        }

        public bool IsProcessing()
        {
            return _processing;
        }

        public int Count()
        {
            return _queueRepo.Count();
        }
        
        public QueuedItem Dequeue()
        {
            return _queueRepo.Dequeue();
        }

        public QueuedItem Enqueue(QueuedItem item)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<PublishQueueHub>();
            var nitem = _queueRepo.Enqueue(item);
            hubContext.Clients.All.Progress(GetStatus());
            return nitem;

        }

        public QueuedItem Enqueue(Guid key, string name, int userId = 0, QueueActions action = QueueActions.Publish)
        {
            var item = new QueuedItem()
            {
                Name = name,
                NodeKey = key,
                Submitted = DateTime.Now,
                UserId = userId,
                Action = (int)action
            };

            return Enqueue(item);
        }

        public IEnumerable<QueuedItem> List()
        {
            return _queueRepo.List();
        }

        public void Clear()
        {
            _queueRepo.Clear();
        }

        public QueueStatus GetStatus()
        {
            return new QueueStatus()
            {
                Count = Count(),
                Processing = IsProcessing()
            };
        }

        public int Process(int throttle = 100)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<PublishQueueHub>();

            _logger.Info<PublishQueueService>("Processing the queue");

            if (_processing)
                return -1;

            lock(_lock)
            {
                if (_processing)
                    return -1;

                _processing = true;
                int count = 0;

                try
                {
                    _logger.Info<PublishQueueService>("Processing: (Throttle {0})", ()=> throttle);
                    while(Count() > 0 && count < throttle)
                    {
                        count++;

                        var item = Dequeue();
                        if (item != null)
                        {
                            bool success = false;

                            if (item.Action > (int)QueueActions.Reserved)
                            {
                                success = PublishQueue.FireCustomAction(
                                    new PublishQueueEventArgs
                                    {
                                        Item = item
                                    });
                            }
                            else
                            {
                                var contentNode = _contentService.GetById(item.NodeKey);
                                if (contentNode != null)
                                {
                                    switch ((QueueActions)item.Action)
                                    {
                                        case QueueActions.Publish:
                                            success = PublishItem(contentNode, item);
                                            break;
                                        case QueueActions.Unpublish:
                                            success = UnPublishItem(contentNode, item);
                                            break;
                                        case QueueActions.Save:
                                            success = SaveItem(contentNode, item);
                                            break;
                                        case QueueActions.Delete:
                                            success = DeleteItem(contentNode, item);
                                            break;
                                        default:
                                            _logger.Info<PublishQueueService>("NoAction Processed", () => contentNode.Name);
                                            success = true;
                                            break;
                                    }

                                }
                            }
                            if (!success && item.Attempt < 10)
                            {
                                Enqueue(item);
                            }
                        }

                        hubContext.Clients.All.Progress(GetStatus());

                    }
                }
                finally
                {
                    _processing = false; 
                }

                hubContext.Clients.All.Progress(GetStatus());
                return count;
            }
        }


        private bool PublishItem(IContent node, QueuedItem queuedItem)
        {
            var attempt = _contentService.SaveAndPublishWithStatus(node, queuedItem.UserId);
            if (attempt.Success)
            {
                _logger.Info<PublishQueueService>("Published: {0}", () => node.Name);
            }
            else
            {
                _logger.Warn<PublishQueueService>("Failed to publish: {0}", () => node.Name);
            }

            return attempt.Success;
        }

        private bool UnPublishItem(IContent node, QueuedItem queuedItem)
        {
            _logger.Info<PublishQueueService>("Unpublishing: {0}", () => node.Name);
            return _contentService.UnPublish(node, queuedItem.UserId);
        }

        private bool SaveItem(IContent node, QueuedItem queuedItem)
        {
            _logger.Info<PublishQueueService>("Saving: {0}", () => node.Name);
            _contentService.Save(node, queuedItem.UserId);
            return true;
        }

        public bool DeleteItem(IContent node, QueuedItem queuedItem)
        {
            _logger.Info<PublishQueueService>("Deleting: {0}", () => node.Name);
            _contentService.Delete(node, queuedItem.UserId);
            return true;
        }

    }
}
