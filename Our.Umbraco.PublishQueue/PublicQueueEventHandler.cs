using Microsoft.AspNet.SignalR;
using Owin;
using Semver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Web;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;

namespace Our.Umbraco.PublishQueue
{
    public class PublicQueueEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            UmbracoDefaultOwinStartup.MiddlewareConfigured += UmbracoDefaultOwinStartup_MiddlewareConfigured;
        }

        private void UmbracoDefaultOwinStartup_MiddlewareConfigured(object sender, OwinMiddlewareConfiguredEventArgs e)
        {
            if (UmbracoVersion.Current.Major <= 7 && UmbracoVersion.Current.Minor < 6)
            {
                LogHelper.Info<PublicQueueEventHandler>("Pre: 7.6 - Mapping Signal R");
                e.AppBuilder.MapSignalR("/umbraco/backoffice/signalr", new HubConfiguration { EnableDetailedErrors = true });
            }
        }


        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ApplyMigrations(applicationContext, "Our.Umbraco.PublishQueue", new SemVersion(0, 0, 1));

            PublishQueueContext.EnsureContext(
                applicationContext.DatabaseContext,
                applicationContext.Services.ContentService,
                applicationContext.ProfilingLogger.Logger
                );

            ContentTreeController.MenuRendering += ContentTreeController_MenuRendering;
        }

        private void ContentTreeController_MenuRendering(TreeControllerBase sender, MenuRenderingEventArgs e)
        {
            if (sender.TreeAlias == "content")
            {
                var publishItem = new MenuItem("queue", "Send to Publication Queue");
                publishItem.Icon = "indent";
                publishItem.SeperatorBefore = true;
                publishItem.AdditionalData.Add("actionView",
                    "/App_Plugins/PublishQueue/sendtoqueue.html?id={id}");

                e.Menu.Items.Insert(e.Menu.Items.Count - 1, publishItem);

            }
        }

        private void ApplyMigrations(ApplicationContext applicationContext, 
            string productName, SemVersion targetVersion)
        {
            var currentVersion = new SemVersion(0);

            var migrations = applicationContext.Services.MigrationEntryService.GetAll(productName);
            var latest = migrations.OrderByDescending(x => x.Version).FirstOrDefault();
            if (latest != null)
                currentVersion = latest.Version;

            if (targetVersion == currentVersion)
                return;

            var migrationRunner = new MigrationRunner(
                applicationContext.Services.MigrationEntryService,
                applicationContext.ProfilingLogger.Logger,
                currentVersion,
                targetVersion,
                productName);

            try
            {
                migrationRunner.Execute(applicationContext.DatabaseContext.Database);
            }
            catch(Exception ex)
            {
                applicationContext.ProfilingLogger
                    .Logger.Error<PublicQueueEventHandler>("Error running migration", ex);
            }
        }
    }
}
