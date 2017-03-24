using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.PublishQueue.Models
{
    public class QueueStatus
    {
        public int Count { get; set; }
        public bool Processing { get; set; }
        public int Total { get; set; }
        public string LastAction { get; set; }
    }
}
