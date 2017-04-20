using Our.Umbraco.PublishQueue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.PublishQueue
{
    public class PublishQueue
    {
        public static event PublishQueueEventHandler ProcessCustomAction;

        internal static bool FireCustomAction(PublishQueueEventArgs e)
        {
            if (ProcessCustomAction != null)
                ProcessCustomAction(e);

            return e.Sucess;
        }
    }

    public delegate void PublishQueueEventHandler(PublishQueueEventArgs e);

    public class PublishQueueEventArgs
    {
        public QueuedItem Item { get; set; }
        public bool Sucess { get; set; }
    }
}
