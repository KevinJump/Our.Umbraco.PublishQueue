using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Our.Umbraco.PublishQueue.Models
{
    [TableName("PublishQueue")]
    [PrimaryKey("id", autoIncrement = true)]
    public class QueuedItem
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("nodeKey")]
        public Guid NodeKey { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("userId")]
        public int UserId { get; set; }

        [Column("submitted")]
        public DateTime Submitted { get; set; }

        [Column("attempt")]
        public int Attempt { get; set; }

        [Column("processing")]
        public bool Processing { get; set; }

        [Column("action")]
        public int Action { get; set; }

        // 0.0.2 priority and schedule 
        [Column("priority")]
        public int Priority { get; set; }

        [Column("schedule")]
        public DateTime Schedule { get; set; }

        // 0.0.3 Custom data?
        [Column("data")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string data { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum QueueActions
    {
        None = 0,
        Publish = 1,
        Save,
        Unpublish,
        Delete,
        Reserved = 10
    }
}
