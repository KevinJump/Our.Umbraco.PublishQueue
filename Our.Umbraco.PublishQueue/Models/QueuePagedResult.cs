using Our.Umbraco.PublishQueue.Models;
using System.Collections.Generic;

namespace Our.Umbraco.PublishQueue.Models
{
    public class QueuePagedResult
    {
        public long CurrentPage { get; set; }
        public long ItemsPerPage { get; set; }
        public long TotalItems { get; set; }
        public long TotalPages { get; set; }
        public IEnumerable<QueuedItem> Items { get; set; }
    }


}
