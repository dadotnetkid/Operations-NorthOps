using System.Web;
using System.Web.Optimization;

namespace NorthOps.Portal {
    public class BundleConfig {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/content/css/bootstrap.css",
                "~/content/css/AdminLTE.css",
                "~/content/css/skins/_all-skins.min.css",
                "~/content/css/font-awesome.css",
                "~/content/css/fontello.css",
                "~/content/css/custom.css"));



            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/content/js/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/content/js/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/content/js/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                      "~/content/js/bootstrap.js",
                      //"~/content/js/plugins/fastclick.js",
                      //"~/content/js/plugins/jquery.slimscroll.min.js",
                      //"~/content/js/bootstrap.js",
                      "~/content/js/adminlte.min.js",
                      "~/content/js/init.js",
                      "~/content/js/respond.js"));



#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
