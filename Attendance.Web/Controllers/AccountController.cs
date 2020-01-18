using Attendance.CachingService;
using Attendance.Model;
using Attendance.Service;
using Attendance.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace Attendance.Web.Controllers
{
    public class AccountController : Controller
    {
        public readonly IUserService userService;
        public readonly ISecurityService securityService;
        public readonly IRoleService roleService;
        public readonly IUserPermissionService userPermissionService;
        public readonly ISchoolService schoolService;
        public readonly ISubModuleService subModuleService;
        public readonly ISubModuleItemService subModuleItemService;
        public readonly IWorkflowactionSettingService workflowactionSettingService;
        public readonly INotificationSettingService notificationSettingService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        public AccountController(IUserService userService, ISecurityService securityService, ISubModuleItemService subModuleItemService,
            IRoleService roleService, ISubModuleService subModuleService, IUserPermissionService userPermissionService, ISchoolService schoolService, IWorkflowactionSettingService workflowactionSettingService, INotificationSettingService notificationSettingService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.userService = userService;
            this.securityService = securityService;
            this.roleService = roleService;
            this.subModuleService = subModuleService;
            this.userPermissionService = userPermissionService;
            this.schoolService = schoolService;
            this.workflowactionSettingService = workflowactionSettingService;
            this.notificationSettingService = notificationSettingService;
            this.roleSubModuleItemService = roleSubModuleItemService;
            this.subModuleItemService = subModuleItemService;
        }

        // GET: /Account/
        public ActionResult Index()
        {
            return View("Login");
        }

        [Route("Login")]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [Route("Register")]
        public ActionResult Register(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult VeiwLoginAfterSessionExpire()
        {
            ViewBag.RedirectMessage = "Login redirected after session timeout";
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            UserSession.DestroySessionAfterUserLogout();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult RecoverPassword()
        {
            return View("PasswordRecovery");
        }

        [HttpPost]
        //public JsonResult CheckPasswordRecovery(User user)
        //{
        //    var userObj = userService.GetUserByEmail(user.Email);

        //    string message = "";
        //    var messageSendStatus = false;

        //    if (userObj == null)
        //        message += "Invalid Email!\n";
        //    else
        //    {
        //        AccountHelper.ResetPassword(securityService, userService, userObj, out messageSendStatus, out message);
        //    }

        //    return Json(new
        //    {
        //        messageSendStatus = messageSendStatus,
        //        message = message
        //    }, JsonRequestBehavior.AllowGet);
        //}

        private string GenerateRandomPassword()
        {
            return System.Web.Security.Membership.GeneratePassword(8, 4);
        }

        public ActionResult ViewChangePassword()
        {
            return View("ChangePassword");
        }

        public ActionResult UpdatePassword(User user)
        {
            User aUser = new User();
            aUser = userService.GetUser(user.Id);
            //Encrypt password
            aUser.Password = securityService.GenerateHashWithSalt(user.Password, user.LoginName);
            if (userService.UpdateUser(aUser))
            {
                UserSession.DestroySessionAfterUserLogout();
                return Json(new
                {
                    isSuccess = true,
                    UserId = aUser.Id,
                }, JsonRequestBehavior.AllowGet);
            }
            else
                return null;
        }

        public ActionResult SwitchRole()
        {
            return View("SwitchRole");
        }

        

        [HttpPost]
        public ActionResult CheckLogin(User user, long timeZoneOffset, string language)
        {
            bool isSuccess = false;
            string userId = "";
            bool isAdmin = false;
            string message = string.Empty;
            int roleId = 0;

            user.Password = securityService.GenerateHashWithSalt(user.Password, user.LoginName);
            var aUser = userService.AuthenticateUser(user);

            if (aUser != null)
            {
                if (aUser.RoleId > 0)
                {
                    isSuccess = true;
                    message = "Login Successful.";
                    userId = aUser.Id.ToString();
                    if (aUser.Role.Level > 0) isAdmin = true;
                    roleId = aUser.RoleId;

                    UserSession.SetUserFromSession(UserHelper.PrepareUserModel(roleService, aUser));
                    UserSession.SetTimeZoneOffset(timeZoneOffset);
                    UserSession.SetCurrentUICulture(language);

                    var schoolIds = UserPermissionHelper.GetSchoolIdByAccess(userPermissionService, schoolService);
                    UserSession.SetUserSchoolAccess(schoolIds);
                }
            }
            else
            {
                message = "Failed to Authenticate";
            }

            return Json(new
            {
                isSuccess = isSuccess,
                message = message,
                userId = userId,
                RoleId = roleId,
                IsAdmin= isAdmin
            }, JsonRequestBehavior.AllowGet);
        }

        private bool CheckEmailExists(User user)
        {
            return userService.CheckEmailExists(user);
        }

       
        
    }
}