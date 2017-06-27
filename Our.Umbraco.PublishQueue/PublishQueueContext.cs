using Our.Umbraco.PublishQueue.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Services;

namespace Our.Umbraco.PublishQueue
{
    public class PublishQueueContext
    {
        private static PublishQueueContext _instance;
        public static PublishQueueContext Current
        {
            get
            {
                if (_instance == null)
                    _instance = new PublishQueueContext();

                return _instance;
            }
        }

        // flag to say if the scheduled job is disabled. 
        public bool ScheduleDisabled { get; internal set; }

        public PublishQueueService QueueService { get; internal set; }

        public static void EnsureContext(DatabaseContext dbContext, 
            IContentService contentService, ILogger logger)
        {
            _instance = new PublishQueueContext(dbContext, contentService, logger);
        }

        public PublishQueueContext() { }

        public PublishQueueContext(DatabaseContext dbContext, 
            IContentService contentService, ILogger logger)
        {
            QueueService = new PublishQueueService(dbContext, contentService, logger);

            bool temp;

            if (bool.TryParse(ConfigurationManager.AppSettings["publishQueue.disableScheduledQueue"], out temp))
            {
                ScheduleDisabled = temp;
            }

        }
    }
}
