using Our.Umbraco.PublishQueue.Models;
using Our.Umbraco.PublishQueue.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Our.Umbraco.PublishQueue
{
    public static class ContentServiceExtensions
    {
        /// <summary>
        /// Add a publish request to the queue for a single content item
        /// </summary>
        public static void QueueForPublish(this IContentService contentService, 
            IContent item, int userId = 0)
        {
            QueueForPublish(contentService, item.Key, item.Name, userId);
        }

        /// <summary>
        /// Add a publish request to the queue for a single content item
        /// </summary>
        public static void QueueForPublish(this IContentService contentService, 
            Guid nodeKey, string name, int userId = 0)
        {
            QueueItem(nodeKey, name, userId, QueueActions.Publish);
        }

        /// <summary>
        /// Add a unpublish request to the queue for a single content item
        /// </summary>
        public static void QueueForUnPublish(this IContentService contentService, 
            IContent item, int userId = 0)
        {
            QueueForUnPublish(contentService, item.Key, item.Name, userId);
        }

        /// <summary>
        /// Add a unpublish request to the queue for a single content item
        /// </summary>
        public static void QueueForUnPublish(this IContentService contentService, 
            Guid nodeKey, string name, int userId = 0)
        {
            QueueItem(nodeKey, name, userId, QueueActions.Unpublish);
        }

        /// <summary>
        /// Add a and item to the queue - with a specifed action.
        /// </summary>
        private static void QueueItem(Guid nodeKey, string name, int userId, QueueActions action)
        {
            PublishQueueContext.Current.QueueService.Enqueue(nodeKey, name, userId, (int)action);
        }

        private static void QueueItem(Guid nodeKey, string name, int userId, int action)
        {
            PublishQueueContext.Current.QueueService.Enqueue(nodeKey, name, userId, action);
        }
    }
}
