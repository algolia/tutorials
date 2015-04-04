using Algolia.Search;
using PackageTrack.Web.Data;
using PackageTrack.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PackageTrack.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Create our Algolia client
            var algoliaClient = new AlgoliaClient("<APPLICATION_ID>", "<ADMIN_API_KEY>");

            // Create our index helper
            var indexHelper = new IndexHelper<Package>(algoliaClient, "packages", "Id");

            // Store our index helper in an application variable.
            // We don't want to create a new one each time
            // because it will impact performance.
            Application.Add("PackageIndexHelper", indexHelper);
        }
    }
}
