using System.Web;
using System.Web.Optimization;

namespace FoodCourt
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular/lib").Include(
                       "~/Scripts/angular.js",
                       "~/Scripts/angular-ui/ui-bootstrap.js",
                       "~/Scripts/angular-ui/ui-bootstrap-tpls.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular/app")
                .IncludeDirectory("~/Scripts/app", "*.js", true));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/imports.css",
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap.overrides.css",
                      "~/Scripts/angular-csp.css",
                      "~/Content/main.css"));
        }
    }
}
