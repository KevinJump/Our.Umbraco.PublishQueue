using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Our.Umbraco.PublishQueue.Migrations.TargetOneZero
{
    [Migration("0.0.6", 1, "Our.Umbraco.PublishQueue")]
    public class EnlargeDataColumn : MigrationBase
    {
        private readonly UmbracoDatabase _db =
            ApplicationContext.Current.DatabaseContext.Database;


        public EnlargeDataColumn(ISqlSyntaxProvider sqlSyntax, ILogger logger)
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
                && x.ColumnName.InvariantEquals("data")) == true)
            {
                Logger.Info<EnlargeDataColumn>("Making the data column bigger");
                Alter.Table("PublishQueue").AlterColumn("data").AsString(2048).Nullable();
            }

        }
    }
}
