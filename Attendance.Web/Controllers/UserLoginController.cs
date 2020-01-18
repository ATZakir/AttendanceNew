using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Attendance.Web.Controllers
{
    [RoutePrefix("")]
    public class UserLoginController : Controller
    {
        // GET: /school
        [Route("School")]
        public ActionResult SchoolUserLogin()
        {
            return View();
        }


        // GET: /Admin
        [Route("Admin")]
        public ActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public void SetCurrentUiCultureWithoutLogin(string language)
        {
            if (language == "bn")
            {
//                System.Web.HttpContext.Current.Session["CurrentUICulture"] = "bn-BD";
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("bn-BD");
            }
            else
            {
//                System.Web.HttpContext.Current.Session["CurrentUICulture"] = "en-US";
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            }
        }
    }
}