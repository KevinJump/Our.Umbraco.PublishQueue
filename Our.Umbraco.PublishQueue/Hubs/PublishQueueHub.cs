using Microsoft.AspNet.SignalR;
using Our.Umbraco.PublishQueue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Our.Umbraco.PublishQueue.Hubs
{
    public class PublishQueueHub : Hub
    {
        public QueueStatus QueueSize()
        {
            return PublishQueueContext.Current.QueueService.GetStatus();
        }
    }
}
