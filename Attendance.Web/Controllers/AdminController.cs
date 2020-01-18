using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Attendance.Web.Helpers;

namespace Attendance.Web.Controllers
{

    public class AdminController : Controller
    {
        // GET: Admin
        //        public ActionResult Index()
        //        {
        //            return View("Login");
        //        }

        public ActionResult Index()
        {
            Session["selectedmoduleid"] = 99;
            return View("Admin");
        }

        public ActionResult Admin()
        {
            return RedirectToAction("Index");
        }

        public ActionResult School()
        {
            Session["selectedmoduleid"] = 99;
            return View("School");
        }

        public ActionResult Login()
        {
            return View();
        }
        public ActionResult PushNotification()
        {
            return View();
        }
    }
}