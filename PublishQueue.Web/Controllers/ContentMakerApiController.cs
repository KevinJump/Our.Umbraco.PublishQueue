using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Umbraco.Web.WebApi;

namespace PublishQueue.Web.Controllers
{
    public class ContentMakerApiController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public void GetMoreContent()
        {

            var root = Services.ContentService.GetRootContent();

            if (root.Any())
            {
                var rootParent = root.First().Id;

                for(int n=0;n<100;n++)
                {
                    var name = $"Test Content {n}";
                    var node = Services.ContentService.CreateContentWithIdentity(name, rootParent, "contentItem");

                    node.SetValue("title", name);
                    node.SetValue("content", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce semper posuere malesuada.");

                    Services.ContentService.Save(node);
                }
            }
        }
    }
}