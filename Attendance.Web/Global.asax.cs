using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;


namespace Attendance.Web
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
            Bootstrapper.Run();

//            log4net.Config.XmlConfigurator.Configure();
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown  

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs  
            Exception ex = Server.GetLastError();
            HttpContext httpContext = HttpContext.Current;
            //Session["CurrentError"] = "Global: " + Server.GetLastError().Message;
            // Server.Transfer("~/Error");

            if (httpContext != null)
            {
                //var exceptionMessage = HttpUtility.UrlEncode(ex.Message);
                //RequestContext requestContext = ((MvcHandler)httpContext.CurrentHandler).RequestContext;
                //httpContext.Response.Write(ex.Message.ToString());
                var myString = ex.Message.Replace(Environment.NewLine, " ");
                //httpContext.Response.Redirect("~/Error");
                try
                {
                    httpContext.Response.Write(myString);
                    //httpContext.Response.Redirect("~/Error?msg=" + myString);
                }
                catch (Exception)
                {
                }
            }

            // Clear the error from the server
            Server.ClearError();
        }


        void Session_Start(object sender, EventArgs e)
        {
            if (Session["LoggedInUser"] == null)
            {
                var tmpURi = Request.Url;
                var tmpPort = (tmpURi.Port > 0) ? ":" + tmpURi.Port : "";
                var rootUrl = tmpURi.Scheme + "://" + tmpURi.Host + tmpPort + "/";
                if (!(tmpURi.AbsoluteUri.Equals(rootUrl) || tmpURi.AbsoluteUri.Contains("Home") || tmpURi.AbsoluteUri.Contains("LeagueJoin") || tmpURi.AbsoluteUri.Contains("Register") || tmpURi.AbsoluteUri.Contains("PaymentReturn") || tmpURi.AbsoluteUri.Contains("MatchPaymentReturn") || tmpURi.AbsoluteUri.Contains("IPNHandler") || tmpURi.AbsoluteUri.Contains("SMSHandler") || tmpURi.AbsoluteUri.Contains("SubscriptionHandler") || tmpURi.AbsoluteUri.Contains("SaveMobilePayment") || tmpURi.AbsoluteUri.Contains("MatchSchedule"))) 
                {
                    Response.Redirect(rootUrl);
                }
            }
        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends.   
            // Note: The Session_End event is raised only when the sessionstate mode  
            // is set to InProc in the Web.config file. If session mode is set to StateServer   
            // or SQLServer, the event is not raised.  

        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["CurrentUICulture"] != null)
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)HttpContext.Current.Session["CurrentUICulture"]);
            }

            if (HttpContext.Current.Session != null && Session["LoggedInUser"] != null)
            {
                var tmpURi = Request.Url;
                var tmpPort = (tmpURi.Port > 0) ? ":" + tmpURi.Port : "";
                var rootUrl = tmpURi.Scheme + "://" + tmpURi.Host + tmpPort + "/"; 
                 var dashBoardUrl = tmpURi.Scheme + "://" + tmpURi.Host + tmpPort + "/User/Dashboard";
                if (tmpURi.AbsoluteUri.Equals(rootUrl) || (!tmpURi.AbsoluteUri.ToLower().Contains("admindashboard") && tmpURi.AbsoluteUri.ToLower().Contains("home")) || tmpURi.AbsoluteUri.ToLower().Contains("register") || tmpURi.AbsoluteUri.ToLower().Contains("Login") || tmpURi.AbsoluteUri.ToLower().Contains("Login"))
                {
                    Response.Redirect(dashBoardUrl);
                }
            }
        }
    }
}
