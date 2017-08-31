using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Umbraco.Core.Logging;
using Umbraco.Web.Scheduling;

namespace Our.Umbraco.PublishQueue
{

#if !TARGETSEVENFIVE
    public class QueueCheckRecurringTask : RecurringTaskBase
    {
        public QueueCheckRecurringTask(IBackgroundTaskRunner<RecurringTaskBase> runner, int delayMilliseconds, int periodMilliseconds)
            : base(runner, delayMilliseconds, periodMilliseconds)
        {
        }

        public override bool IsAsync => false;
        public override bool RunsOnShutdown => false;

        public override Task<bool> PerformRunAsync(CancellationToken token)
        {
            return Task.FromResult(true);
        }

        public override bool PerformRun()
        {
            try
            {
                LogHelper.Debug<QueueCheckRecurringTask>("Processing Queue to Schedule.");
                PublishQueueContext.Current.QueueService.Process();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Warn<QueueCheckRecurringTask>("Error running scheduled queue check: {0}", () => ex.Message);
                return false;
            }
        }
    }
#endif

}
