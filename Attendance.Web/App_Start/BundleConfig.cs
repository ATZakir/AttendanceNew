using System.Web;
using System.Web.Optimization;

namespace Attendance.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //                        "~/Scripts/jquery-{version}.js"));



            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/mainScripts").Include(
                "~/Scripts/jquery-{version}.js",
                      "~/Scripts/bootstrap.js",
                      "~/App/main.js",
                      "~/Scripts/Vendor/owl.carousel.js",
                      "~/Scripts/Vendor/contact-form.js",
                      "~/Scripts/Vendor/scrollUp.min.js",
                      "~/Scripts/Vendor/jquery.doubleScroll.js",
                      "~/Scripts/moment.min.js",
                      "~/Scripts/Vendor/wow.min.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/userMainScripts").Include(
                "~/Scripts/User/vendor/jquery-2.2.4.min.js",
                "~/Scripts/User/vendor/bootstrap.min.js",
                "~/Scripts/User/easing.min.js",
                "~/Scripts/User/hoverIntent.js",
                "~/Scripts/User/superfish.js",
                "~/Scripts/User/jquery.magnific-popup.min.js",
                "~/Scripts/User/jquery-ui.js",
                "~/Scripts/User/owl.carousel.min.js",
                "~/Scripts/User/jquery.nice-select.min.js",
                "~/Scripts/User/mail-script.js",
                "~/Scripts/moment.min.js",
                "~/Scripts/User/main.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/angularScripts").Include(
                "~/Scripts/angular.js",
                "~/Scripts/angular-animate.js",
                "~/Scripts/angular-touch.js",
                "~/Scripts/angular-aria.js",
                "~/Scripts/angular-messages.js",
                "~/Scripts/angular-resource.js",
                "~/Scripts/angular-sanitize.js",
                "~/Scripts/angular-touch.js",
                "~/Scripts/angular-resource.js",
                "~/Scripts/angular-parse-ext.js",
                "~/Scripts/angular-loader.js",
                "~/Scripts/angular-aria.js",
                "~/Scripts/Vendor/angular-avatar.min.js",
                "~/Scripts/Vendor/angular-material.min.js",
                "~/Scripts/Vendor/angular-block-ui.min.js",
                "~/Scripts/Vendor/ui-bootstrap.js",
                "~/Scripts/Vendor/ui-bootstrap-tpls.js",
                "~/Scripts/Vendor/angular-strap.js",
                "~/Scripts/Vendor/angular-strap.tpl.js",
                "~/Scripts/Vendor/dialogs.min.js",
                "~/Scripts/Vendor/bootbox.js",
                "~/Scripts/Vendor/ngBootbox.js",
                "~/Scripts/Vendor/Chart.min.js",
                "~/Scripts/Vendor/sortable.js",
                "~/Scripts/Vendor/angular-chart.js",
                "~/Scripts/Vendor/multiple-select.js",
                "~/Scripts/Vendor/autocomplete.js"
            ));
            

            bundles.Add(new ScriptBundle("~/bundles/appScript").Include(
                "~/Scripts/App/app.js",
                "~/Scripts/App/Directive/CommonDirective.js"
            ));
            


            bundles.Add(new StyleBundle("~/Content/userCSS").Include(
                "~/Content/normalize.css",
                "~/Content/site.css",
                "~/Content/responsive.css"));

            bundles.Add(new StyleBundle("~/Content/userMainCSS").Include(
                "~/Content/bootstrap.css",
                "~/Content/font-awesome.css",
                "~/Content/linearicons.css",
                "~/Content/angular-block-ui.css",
                "~/Content/angular-material.css",
                "~/Content/magnific-popup.css",
                "~/Content/animate.css",
                "~/Content/User/css/jquery-ui.css",
                "~/Content/normalize.css",
                "~/Content/site.css",
                "~/Content/responsive.css"));

            bundles.Add(new StyleBundle("~/Content/adminMainCSS").Include(
                "~/Content/bootstrap.css",
                "~/Content/font-awesome.css",
                "~/Content/linearicons.css",
                "~/Content/angular-block-ui.css",
                "~/Content/angular-material.css",
                "~/Content/magnific-popup.css",
                "~/Content/animate.css",
                "~/Content/User/css/jquery-ui.css",
                "~/Content/normalize.css",
                "~/Content/adminStyle.css",
                "~/Content/adminResponsive.css"));


            bundles.Add(new StyleBundle("~/Content/userLoginLayoutCSS").Include(
                "~/Content/userLoginLayoutStyle.css"));

            //=================== for adminView layout ===============
//            bundles.Add(new StyleBundle("~/Content/vendorCSS").Include(
//                "~/Content/bootstrap.min.css",
//                "~/Content/font-awesome.css",
//                "~/Content/angular-block-ui.min.css",
//                "~/Content/owl.carousel.css",
//                "~/Content/linearicons.css",
//                "~/Content/magnific-popup.css",
//                "~/Content/angular-material.min.css",
//                "~/Content/autocomplete.css",
//                "~/Content/dialogs.min.css",
//                "~/Content/multiple-select.css",
//                "~/Content/animate.css"));

            //=================== for adminView layout ===============
            bundles.Add(new StyleBundle("~/Content/adminCSS").Include(
                "~/Content/normalize.css",
                "~/Content/bootstrap3/css/bootstrap.css",
                "~/Content/font-awesome.css",
                "~/Content/angular-block-ui.min.css",
                "~/Content/angular-material.min.css",
                "~/Content/autocomplete.css",
                "~/Content/dialogs.min.css",
                "~/Content/multiple-select.css",
                "~/Content/adminStyle.css",
                "~/Content/siteResponsive.css"));

//         BundleTable.EnableOptimizations = true;
        }
    }
}
