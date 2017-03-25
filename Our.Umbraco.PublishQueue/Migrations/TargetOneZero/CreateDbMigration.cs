using Our.Umbraco.PublishQueue.Models;
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
    [Migration("0.0.1", 1, "Our.Umbraco.PublishQueue")]
    public class CreateDbMigration : MigrationBase
    {
        private readonly UmbracoDatabase _db =
            ApplicationContext.Current.DatabaseContext.Database;

        public CreateDbMigration(ISqlSyntaxProvider sqlSyntax, ILogger logger) : base(sqlSyntax, logger)
        {
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }

        public override void Up()
        {
            if (!_db.TableExist("PublishQueue"))
                _db.CreateTable<QueuedItem>(false);
        }
    }
}
