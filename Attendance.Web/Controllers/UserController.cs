using Newtonsoft.Json;
using Attendance.CachingService;
using Attendance.ClientModel;
using Attendance.Model;
using Attendance.Service;
using Attendance.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Attendance.Web.Controllers
{
    public class UserController : Controller
    {
        public readonly IUserService userService;
        public readonly IUserPermissionService userPermissionService;
        public readonly ISecurityService securityService;
        public readonly IRoleService roleService;
        public readonly ISchoolService schoolService;
        public readonly IDistrictService districtService;
        public readonly IUpazilaService upazilaService;
        public readonly ISubModuleItemService subModuleItemService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        public UserController(IUserService userService, ISubModuleItemService subModuleItemService,
            IRoleSubModuleItemService roleSubModuleItemService, ISecurityService securityService, IRoleService roleService, IUserPermissionService userPermissionService, ISchoolService schoolService, IDistrictService districtService, IUpazilaService upazilaService)
        {
            this.userService = userService;
            this.subModuleItemService = subModuleItemService;
            this.roleSubModuleItemService = roleSubModuleItemService;
            this.securityService = securityService;
            this.roleService = roleService;
            this.userPermissionService = userPermissionService;
            this.schoolService = schoolService;
            this.districtService = districtService;
            this.upazilaService = upazilaService;
        }

        RoleSubModuleItem permission = null;

        public ActionResult TopTournamentAndMatchUser()
        {
            return View("TopTournamentAndMatchUser");
        }
        public ActionResult Index()
        {
            string cacheKey = "permission:user" + Helpers.UserSession.GetUserFromSession().RoleId;
            const string url = "/User/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set(cacheKey, permission, 240);
                    return View("User");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }

            return View("~/Views/Shared/NoPermission.cshtml");
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CreateUser(User user)
        {
            string cacheKey = "permission:user" + Helpers.UserSession.GetUserFromSession().RoleId;
            const string url = "/User/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            var isSuccess = false;
            var message = string.Empty;
            var isNew = user.Id == 0 ? true : false;

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(user))
                    {
                        user.LoginName = user.LoginName;
                        user.Password = securityService.GenerateHashWithSalt(user.Password, user.LoginName);
                        if (user.UserPermissions != null)
                        {
                            foreach (var permission in user.UserPermissions)
                            {
                                permission.Id = Guid.NewGuid();
                                permission.UserId = user.Id;
                            }
                        }
                        if (this.userService.CreateUser(user))
                        {
                            isSuccess = true;
                            message = "User saved successfully!";
                        }
                        else
                        {
                            message = "User could not saved!";
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = "Can't save. Same employee name found!";
                    }
                }
                else
                {
                    message = Resources.ResourceCommon.MsgNoPermissionToCreate;
                }
            }
            else
            {
                if (permission.UpdateOperation == true)
                {
                    var userObj = this.userService.GetUser(user.Id);
                    if (userObj != null)
                    {
                        //delete
                        if (userObj.UserPermissions.Any())
                        {
                            foreach (var userPermission in userObj.UserPermissions.ToList())
                            {
                                this.userPermissionService.DeleteUserPermission(userPermission.Id);
                            }
                        }
                        //create
                        if (user.UserPermissions.Any())
                        {
                            foreach (var userPermission in user.UserPermissions.ToList())
                            {
                                userPermission.Id = Guid.NewGuid();
                                userPermission.UserId = user.Id;
                                this.userPermissionService.CreateUserPermission(userPermission);
                            }
                        }
                    }

                    if (userObj != null)
                    {
                        userObj.Password = user.Password;
                        if (!string.IsNullOrEmpty(user.Password))
                        {
                            userObj.Password = securityService.GenerateHashWithSalt(user.Password, user.LoginName);
                        }
  
                        userObj.LoginName = user.LoginName;
                        userObj.EmployeeId = user.EmployeeId;
                        userObj.IsActive = user.IsActive;
                        userObj.Email = user.Email;
                        userObj.RoleId = user.RoleId;

                        if (this.userService.UpdateUser(userObj))
                        {
                            isSuccess = true;
                            message = "User updated successfully!";
                        }
                        else
                        {
                            message = "User could not updated!";
                        }
                    }
                }
                else
                {
                    message = Resources.ResourceCommon.MsgNoPermissionToUpdate;
                }
            }

            return Json(new
            {
                isSuccess = isSuccess,
                message = message,
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeleteUser(User user)
        {
            string cacheKey = "permission:user" + Helpers.UserSession.GetUserFromSession().RoleId;
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/User/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                var userObj = this.userService.GetUser(user.Id);
                if (userObj != null)
                {
                    if (userObj.UserPermissions != null)
                    {
                        foreach (var userPermission in userObj.UserPermissions.ToList())
                        {
                            this.userPermissionService.DeleteUserPermission(userPermission.Id);
                        }
                    }
                }

                isSuccess = this.userService.DeleteUser(user.Id);
                if (isSuccess)
                {
                    message = "User deleted successfully!";
                }
                else
                {
                    message = "User can't be deleted!";
                }
            }
            else
            {
                message = Resources.ResourceCommon.MsgNoPermissionToDelete;
            }

            return Json(new
            {
                isSuccess = isSuccess,
                message = message
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserList()
        {
            var userListObj = userService.GetAllUser();
            List<UserModel> userVmList = new List<UserModel>();

            if ( UserSession.GetUserFromSession().RoleId == 1)
            {
                foreach (var user in userListObj)
                {
                    UserModelMaper(user, userVmList);
                }
            }
            else if (UserSession.GetUserFromSession().RoleId == 2)
            {
                var userListForAdmin = userListObj.Where(s =>
                    s.LoginName == UserSession.GetUserFromSession().LoginName && s.LoginName != "SuperAdmin" || s.RoleId > 1).ToList();

                foreach (var user in userListForAdmin)
                {
                    UserModelMaper(user, userVmList);
                }
            }
            else
            {
                var userListForAdmin = userListObj.Where(s =>
                    s.LoginName == UserSession.GetUserFromSession().LoginName && s.RoleId == UserSession.GetUserFromSession().RoleId).ToList();

                foreach (var user in userListForAdmin)
                {
                    UserModelMaper(user, userVmList);
                }
            }

            return Json(userVmList.OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
        }

        private void UserModelMaper(User user, List<UserModel> userVmList)
        {
            UserModel userTemp = new UserModel();

            userTemp = UserHelper.PrepareUserModel(roleService, user, districtService, upazilaService,
                schoolService);

            if (userTemp.RoleId != 1)
                userVmList.Add(userTemp);
            else if (UserSession.IsAdmin())
                userVmList.Add(userTemp);
        }

        public JsonResult GetUser(int id)
        {
            var user = userService.GetUser(id);

            UserModel userTemp = new UserModel();

            userTemp = UserHelper.PrepareUserModel(roleService, user);

            return Json(userTemp, JsonRequestBehavior.AllowGet);
        }

        private bool CheckIsExist(Model.User user)
        {
            return this.userService.CheckIsExist(user);
        }
    }
}