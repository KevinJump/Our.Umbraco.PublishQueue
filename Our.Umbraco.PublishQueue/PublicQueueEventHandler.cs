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

using Umbraco.Web.Scheduling;

using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;
using Our.Umbraco.PublishQueue.Controllers;
using System.Web;
using System.Web.Mvc;

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
            ApplyMigrations(applicationContext, "Our.Umbraco.PublishQueue", new SemVersion(0, 0, 7));

            PublishQueueContext.EnsureContext(
                applicationContext.DatabaseContext,
                applicationContext.Services.ContentService,
                applicationContext.ProfilingLogger.Logger
                );

            ContentTreeController.MenuRendering += ContentTreeController_MenuRendering;

            global::Umbraco.Web.UI.JavaScript.ServerVariablesParser.Parsing += ServerVariablesParser_Parsing;

            if (UmbracoVersion.Current.Major == 7 && UmbracoVersion.Current.Minor > 6)
            {
                // we can't setup a scheduled job here. because we need 7.6+ to do this in the background
                applicationContext.ProfilingLogger.Logger.Info<PublishQueue>("Scheduled job needs to be setup in umbracoSettings.Config prior to umbraco 7.6+");
            }
            else
            {
                SetupScheduledTask(applicationContext.ProfilingLogger.Logger);
            }
        }

        private void ServerVariablesParser_Parsing(object sender, Dictionary<string, object> e)
        {
            // adds a sys variable  we can get in javascript 
            if (HttpContext.Current != null)
            {
                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

                e.Add("publishQueue", new Dictionary<string, object>
                {
                    { "QueueService", urlHelper.GetUmbracoApiServiceBaseUrl<PublishQueueApiController>(
                        controller => controller.GetApiEndpoint() ) }
                });
            }

        }

        private void ContentTreeController_MenuRendering(TreeControllerBase sender, MenuRenderingEventArgs e)
        {

            if (sender.TreeAlias == "content")
            {
                bool showMenuItem = false;
                //check if current user is an admin - different in 7.7+
                sender.Security.CurrentUser.UserType.Alias.InvariantEquals("admin");
                //sender.Security.CurrentUser.Groups.Any(x => x.Alias.InvariantEquals("admin"));
                if (!showMenuItem && int.TryParse(e.NodeId, out int nodeId))
                {
                    var permissions = sender.Services.UserService.GetPermissions(sender.Security.CurrentUser, nodeId);
                    if (permissions.Any(x => x.AssignedPermissions.InvariantContains("U")))
                    {
                        showMenuItem = true;
                    }
                }
                if (showMenuItem)
                {
                    var publishItem = new MenuItem("queue", "Send to Publication Queue");
                    publishItem.Icon = "indent";
                    publishItem.SeperatorBefore = true;
                    publishItem.AdditionalData.Add("actionView",
                        "/App_Plugins/PublishQueue/sendtoqueue.html?id={id}");

                    e.Menu.Items.Insert(e.Menu.Items.Count - 1, publishItem);
                }

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
            catch (Exception ex)
            {
                applicationContext.ProfilingLogger
                    .Logger.Error<PublicQueueEventHandler>("Error running migration", ex);
            }
        }

        private BackgroundTaskRunner<IBackgroundTask> _taskRunner;

        private void SetupScheduledTask(ILogger logger)
        {
#if !TARGETSEVENFIVE
            if (!PublishQueueContext.Current.ScheduleDisabled)
            {
                var period = PublishQueueContext.Current.SchedulePeriod;

                logger.Info<PublishQueue>("Setting up background Processing task to run every {0} seconds", () => period);
                logger.Info<PublishQueue>("To disable background processing set [publishQueue.disableScheduledQueue] to false in appSettings section of web.config");
                logger.Info<PublishQueue>("to change the period use [publishQueue.checkPeriod] setting in appSettings");

                _taskRunner = new BackgroundTaskRunner<IBackgroundTask>("Queue Job", logger);
                if (_taskRunner != null)
                {
                    var queueCheck = new QueueCheckRecurringTask(
                        _taskRunner,
                        period * 1000,
                        period * 1000);

                    _taskRunner.Add(queueCheck);
                }
            }
#endif
        }
    }
}
