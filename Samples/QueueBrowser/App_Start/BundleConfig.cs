using System.Web;
using System.Web.Optimization;

namespace QueueBrowser
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/content/css")
                .Include(
                    "~/Content/bootstrap.css",
                    "~/Content/bootstrap-theme.css",

                    "~/Content/angular-toastr.css",
                    "~/Content/bootstrap-dialog.css",

                    "~/Content/alert.css",
                    "~/Content/badge.css",
                    "~/Content/metric.css",

                    "~/Content/site.css"
                )
            );

            bundles.Add(new ScriptBundle("~/bundles/modernizr")
                .Include(
                    "~/Scripts/modernizr-{version}.js"
                )
            );

            bundles.Add(new ScriptBundle("~/bundles/scripts")
                .Include(
                    "~/Scripts/jquery-{version}.js",
                    
                    /* angular */
                    "~/Scripts/angular.js",
                    "~/Scripts/angular-animate.js",
                    "~/Scripts/angular-messages.js",
                    "~/Scripts/angular-route.js",
                    "~/Scripts/angular-sanitize.js",
                    /* bootstrap */
                    "~/Scripts/bootstrap.js",

                    /* other */
                    "~/Scripts/lodash.js",
                    "~/Scripts/jquery.signalR-{version}.js",
                    "~/Scripts/angular-signalr-hub.js",
                    "~/Scripts/moment.js",
                    "~/Scripts/angular-moment.js",
                    "~/Scripts/bootstrap-dialog.js",
                    "~/Scripts/angular-toastr.tpls.js",
                    "~/Scripts/angular-ui/ui-bootstrap-tpls.js"
                )
            );

            bundles.Add(new ScriptBundle("~/bundles/app")

                .Include("~/app/app.js")

                .IncludeDirectory("~/app/models/", "*.js")
                .IncludeDirectory("~/app/services/", "*.js")

                .IncludeDirectory("~/app/queue/", "*.js")
                .IncludeDirectory("~/app/logging/", "*.js")
            );

#if !DEBUG
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
