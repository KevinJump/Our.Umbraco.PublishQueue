using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Logging;

using Our.Umbraco.PublishQueue;

namespace PublishQueue.Web.App_Code
{
    public class CustomQueueItems : ApplicationEventHandler
    {
        private ILogger _logger;

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            _logger = applicationContext.ProfilingLogger.Logger;

            Our.Umbraco.PublishQueue.PublishQueue.ProcessCustomAction += PublishQueue_ProcessCustomAction;
        }

        private void PublishQueue_ProcessCustomAction(PublishQueueEventArgs e)
        {
            // we get called when a custom event is at the top of the queue
            _logger.Info<CustomQueueItems>("Processing Queue Item: {0}", () => e.Item.data);

            e.Success = true;
            
        }
    }
}