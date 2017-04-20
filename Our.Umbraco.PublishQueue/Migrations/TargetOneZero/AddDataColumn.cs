using Our.Umbraco.PublishQueue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.DatabaseModelDefinitions;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Our.Umbraco.PublishQueue.Migrations.TargetOneZero
{
    [Migration("0.0.3", 1, "Our.Umbraco.PublishQueue")]
    public class AddDataColumn : MigrationBase
    {
        public AddDataColumn(ISqlSyntaxProvider sqlSyntax, ILogger logger) 
            : base(sqlSyntax, logger)
        {
        }

        public override void Down()
        {
        }

        public override void Up()
        {
            var columns = SqlSyntax.GetColumnsInSchema(Context.Database).ToArray();

            if (columns.Any(x => x.TableName.InvariantEquals("PublishQueue")
                && x.ColumnName.InvariantEquals("data")) == false)
            {
                Create.Column("data").OnTable("PublishQueue").AsString().Nullable();
            }

        }
    }
}
