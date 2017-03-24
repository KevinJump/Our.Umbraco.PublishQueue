using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Web.WebApi;

namespace Our.Umbraco.PublishQueue.Controllers
{
    public class PublishQueueScheduleController : UmbracoApiController
    {
        [HttpGet]
        public int ProcessQueue(int throttle = 250)
        {
            if (!PublishQueueContext.Current.ScheduleDisabled)
                return PublishQueueContext.Current.QueueService.Process(throttle);
            else
                Logger.Debug<PublishQueueScheduleController>("Scheduled queue disabled in config");

            return -1;
        }
    }
}
