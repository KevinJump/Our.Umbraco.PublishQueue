using Our.Umbraco.PublishQueue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseModelDefinitions;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Our.Umbraco.PublishQueue.Migrations.TargetOneZero
{
    [Migration("0.0.2", 1, "Our.Umbraco.PublishQueue")]
    public class PriorityAndScheduled : MigrationBase
    {
        private readonly UmbracoDatabase _db =
            ApplicationContext.Current.DatabaseContext.Database;


        public PriorityAndScheduled(ISqlSyntaxProvider sqlSyntax, ILogger logger) 
            : base(sqlSyntax, logger)
        {
        }

        public override void Down()
        {
        }

        public override void Up()
        {
            var columns = SqlSyntax.GetColumnsInSchema(_db).ToArray();

            if (columns.Any(x => x.TableName.InvariantEquals("PublishQueue")
                && x.ColumnName.InvariantEquals("priority")) == false)
            {
                Create.Column("priority").OnTable("PublishQueue").AsInt32().WithDefaultValue(0);
            }

            if (columns.Any(x => x.TableName.InvariantEquals("PublishQueue")
                && x.ColumnName.InvariantEquals("schedule")) == false)
            {
                Create.Column("schedule").OnTable("PublishQueue").AsDateTime().WithDefault(SystemMethods.CurrentDateTime);
            }


            var dbIndexes = SqlSyntax.GetDefinedIndexes(_db)
              .Select(x => x.Item2).ToArray();


            if (dbIndexes.InvariantContains("IX_publishQueueNodeKey") == false)
            {
                // create an index on the node key - for speed?
                Create.Index("IX_publishQueueNodeKey")
                    .OnTable("PublishQueue")
                    .OnColumn("nodeKey")
                    .Unique();
                    // .Ascending();
            }
        }
    }
}
