using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Our.Umbraco.PublishQueue.Migrations.TargetOneZero
{
    [Migration("0.0.3", 2, "Our.Umbraco.PublishQueue")]
    public class AddDashboard : MigrationBase
    {
        public AddDashboard(ISqlSyntaxProvider sqlSyntax, ILogger logger) 
            : base(sqlSyntax, logger)
        {
        }

        public override void Down()
        {
            
        }

        public override void Up()
        {
            var dashboardFile = IOHelper.MapPath(SystemFiles.DashboardConfig);

            Logger.Info<AddDashboard>("Adding Publish Queue dashboard :{0}", ()=> dashboardFile);

            XmlDocument dashboardXml = XmlHelper.OpenAsXmlDocument(dashboardFile);

            var found = dashboardXml.SelectNodes("//section[@alias='publicationQueue']");
            if (found == null || found.Count <= 0)
            {
                XmlDocument dashboard = new XmlDocument();
                dashboard.LoadXml(
                    "<section alias=\"publicationQueue\">" +
                    "   <areas>" +
                    "       <area>settings</area>" +
                    "   </areas>" +
                    "   <tab caption=\"Publication Queue\">" +
                    "       <control>/App_Plugins/PublishQueue/PublishQueueDashboard.html</control>" + 
                    "   </tab>" +
                    "</section>"
                );


                XmlNode node = dashboard.SelectSingleNode("./section");

                XmlNode importedSection = dashboardXml.ImportNode(node, true);
                dashboardXml.DocumentElement.AppendChild(importedSection);

                dashboardXml.Save(IOHelper.MapPath(SystemFiles.DashboardConfig));
            }


        }
    }
}
