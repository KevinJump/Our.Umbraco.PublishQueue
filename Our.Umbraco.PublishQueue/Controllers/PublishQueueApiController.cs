using Our.Umbraco.PublishQueue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

using Our.Umbraco.PublishQueue;
using Microsoft.AspNet.SignalR;
using Our.Umbraco.PublishQueue.Hubs;

namespace Our.Umbraco.PublishQueue.Controllers
{
    [PluginController("Queue")]
    public class PublishQueueApiController : UmbracoAuthorizedJsonController
    {
        [HttpGet]
        public IEnumerable<QueuedItem> GetItems()
        {
        }

        [HttpGet]
        public QueueStatus GetStatus()
        {
            return PublishQueueContext.Current.QueueService.GetStatus();
        }

        [HttpGet]
        public int ProcessQueue(int throttle = 100)
        {
            return PublishQueueContext.Current.QueueService.Process(throttle);
        }

        [HttpGet]
        public int ClearQueue()
        {
            var size = PublishQueueContext.Current.QueueService.Count();
            PublishQueueContext.Current.QueueService.Clear();
            return size;
        }

        [HttpGet]
        public int EnqueueTree(int id, bool all = true, bool unpub = false)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<PublishQueueHub>();

            var contentService = Services.ContentService;
            var node = contentService.GetById(id);
            var count = 0;

            if (unpub || node.Published)
            {
                contentService.QueueForPublish(node);
                count++;
            }


            var children = contentService.GetDescendants(id).ToList();

            if (all)
            {
                foreach (var child in children)
                {
                    hubContext.Clients.All.Add(new QueueStatus()
                    {
                        Count = count,
                        Total = children.Count,
                        Processing = false,
                        LastAction = "Added: " + child.Name
                    });

                    if (unpub || child.Published)
                    {
                        contentService.QueueForPublish(child);
                        count++;
                    }
                }
            }

            return count;
        }
    }
      
}
