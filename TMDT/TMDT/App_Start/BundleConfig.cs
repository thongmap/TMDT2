using System.Web;
using System.Web.Optimization;

namespace TMDT
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/public/js/jquery-1.10.2.min.js",
                        "~/public/js/jquery-ui.js",
                        "~/public/js/bootstrap.min.js",
                        "~/public/js/bootstrap-slider.min.js"
                        ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/public/css/bootstrap.min.css",
                      "~/public/css/font-awesome.min.css",
                      "~/public/css/bootstrap-theme.css",
                      "~/public/css/jquery-ui.css",
                      "~/public/css/bootstrap-slider.min.css",
                      "~/public/css/style.css",
                      "~/public/css/bootstrap-social.css",
                      "~/public/css/cssprogress.css"
                      ));

        }
    }
}
