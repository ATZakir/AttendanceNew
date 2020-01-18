using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Attendance.CachingService;
using Attendance.ClientModel;
using Attendance.Model;
using Attendance.Service;
using Attendance.Web.Helpers;
using WebGrease.Css.Extensions;

namespace Attendance.Web.Controllers
{
    public class AdminDashboardController : Controller
    {
        public readonly ISubModuleItemService subModuleItemService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        public readonly IUserService userService;

        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();
        protected long timeZoneOffset = UserSession.GetTimeZoneOffset();

        public AdminDashboardController(ISubModuleItemService subModuleItemService, IRoleSubModuleItemService roleSubModuleItemService, IUserService userService)
        {
            this.subModuleItemService = subModuleItemService;
            this.roleSubModuleItemService = roleSubModuleItemService;
            this.userService = userService;
        }

        // GET: AdminDashboard
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HomePage()
        {
            Session["selectedmoduleid"] = 99;
            ViewBag.homepage = 99;
            ViewBag.RoleId = UserSession.GetUserFromSession().RoleId;
            return View();
        }

        public ActionResult DashboardPage(int? schoolId = null)
        {
            if(schoolId == null)
                ViewBag.schoolId = UserSession.GetCurrentUserSchool();
            else
                ViewBag.schoolId = schoolId;

            Session["selectedmoduleid"] = 98;
            ViewBag.homepage = 98;
            var usr = UserSession.GetUserFromSession();
            ViewBag.RoleId = usr.RoleId;
            return View();
        }

        public ActionResult DashboardPageAdmin()
        {
            Session["selectedmoduleid"] = 98;
            ViewBag.homepage = 98;
            var usr = UserSession.GetUserFromSession();
            ViewBag.RoleId = usr.RoleId;
            if (usr.Level != 0)
                return View();
            else
                return View("~/Views/Shared/NoPermission.cshtml");
        }

        public JsonResult SetModuleInSession(string id)
        {
            Helpers.UserSession.SetModuleClicked(id);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SubModules(int id)
        {
            ViewBag.ModuleId = id;
            Session["SelectedModuleId"] = id;
            ViewBag.RoleId = UserSession.GetUserFromSession().RoleId;
            return View();
        }

    }
}