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
        /// <summary>
        ///  get the end point - we call this in ApplicationStarted, when we
        ///  are putting the url of our controller into the umbraco
        ///  sys veriables collection, then we can unhardwire the urls 
        ///  in our angular services 
        /// </summary>
        [HttpGet]
        public bool GetApiEndpoint()
        {
            return true;
        }

        [HttpGet]
        public QueuePagedResult GetItems(int page)
        {
            // only return the top 100. because it gets messy with more than that. 
            // return PublishQueueContext.Current.QueueService.List(100);

            return PublishQueueContext.Current.QueueService.GetPaged(page, 25);
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

        [HttpPost]
        public int ClearQueue()
        {
            var size = PublishQueueContext.Current.QueueService.Count();
            PublishQueueContext.Current.QueueService.Clear();
            return size;
        }

        [HttpPost]
        public int EnqueueTree(int id, EnqueueOptions options)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<PublishQueueHub>();

            var contentService = Services.ContentService;
            var node = contentService.GetById(id);
            var count = 0;
            //always queue the first item chosen
            contentService.QueueForPublish(node);
            count++;

            var children = contentService.GetDescendants(id).OrderBy(f => f.Level).ToList();

            if (options.IncludeChildren)
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

                    if (options.IncludeUnpublished || child.Published)
                    {
                        contentService.QueueForPublish(child);
                        count++;
                    }
                }
            }

            return count;
        }

        public class EnqueueOptions
        {
            public bool IncludeChildren { get; set; }
            public bool IncludeUnpublished { get; set; }
        }
    }

}
