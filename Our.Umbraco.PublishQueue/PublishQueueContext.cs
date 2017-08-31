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
        public int SchedulePeriod { get; internal set; }

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


            // get the disabled schedule job setting, which is false by default
            var disabledString = string.IsNullOrEmpty(ConfigurationManager.AppSettings["publishQueue.disableScheduledQueue"]) ?
                "false" : ConfigurationManager.AppSettings["publishQueue.disableScheduledQueue"];

            if (!bool.TryParse(disabledString, out bool disabled))
            {
                ScheduleDisabled = false;
            }
            else
            {
                ScheduleDisabled = disabled;
            }
            

            // get the scheduled period, which defaults to 120 seconds - if the value
            // is missing or currupt.
            var periodString = string.IsNullOrEmpty(ConfigurationManager.AppSettings["publishQueue.checkPeriod"]) ?
                "120" : ConfigurationManager.AppSettings["publishQueue.checkPeriod"];

            if (!int.TryParse(periodString, out int period))
            {
                SchedulePeriod = 120;
            }
            else
            {
                SchedulePeriod = period;
            }

        }
    }
}
