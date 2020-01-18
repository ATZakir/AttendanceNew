using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Attendance.Model;
using Attendance.Service;
using Attendance.CachingService;
using Attendance.Web.Helpers;
using Attendance.ClientModel;

namespace Attendance.Web.Controllers
{
    public class RoleController : Controller
    {
        public readonly IRoleService roleService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        public RoleController(IRoleService roleService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.roleService = roleService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        string cacheKey = "permission:role" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;
        protected long timeZoneOffset = UserSession.GetTimeZoneOffset();
        // GET: /Role/
        public ActionResult Index()
        {
            var url = Request.RawUrl;

            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey);
            if (permission == null)
                permission = roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set("permission:role" + Helpers.UserSession.GetUserFromSession().RoleId, permission, 240);
                    return View("Role");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            return View("~/Views/Shared/NoPermission.cshtml");
        }

        public ActionResult Role()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CreateRole(Role role)
        {
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey);
            string url = string.Empty;
            url = "/Role/Index";

            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            var isSuccess = false;
            var message = string.Empty;
            var isNew = role.Id == 0 ? true : false;
            //var isNew = roleService.GetRole(role.Id);

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(role))
                    {
                        if (this.roleService.CreateRole(role))
                        {
                            isSuccess = true;
                            message = "Role saved successfully";
                        }
                        else
                        {
                            message = "Role could not be saved!";
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = "Can't save. Same role name found!";
                    }
                }
                else
                {
                    message = "You don't have permission to create";
                }
            }
            else
            {
                if (permission.UpdateOperation == true)
                {
                    //if (!CheckIsExist(role))
                    {
                        if (this.roleService.UpdateRole(role))
                        {
                            isSuccess = true;
                            message = "Role updated successfully!";
                        }
                        else
                        {
                            message = "Role could not be updated!";
                        }
                    }
                //    else
                //    {
                //        isSuccess = false;
                //        message = "Can't save. Same role name found!";
                //    }
                }
                else
                {
                    message = "You don't have permission to update";
                }
            }
            return Json(new
            {
                isSuccess = isSuccess,
                message = message,
            }, JsonRequestBehavior.AllowGet);
        }

        private bool CheckIsExist(Model.Role role)
        {
            //if (role.Id > 0)
            //{
            //    if (this.roleService.GetRole(role.Id).Name == role.Name) return false;
            //}
            return this.roleService.CheckIsExist(role);
        }

        [HttpPost]
        public JsonResult DeleteRole(Role role)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/Role/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                isSuccess = this.roleService.DeleteRole(role.Id);
                if (isSuccess)
                {
                    message = "Role deleted successfully!";

                }
                else
                {
                    message = "Role can't be deleted!";
                }
            }
            else
            {
                message = "You don't have permission to delete";
            }
            return Json(new
            {
                isSuccess = isSuccess,
                message = message
            }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetRoleList()
        {
            var roleListObj = this.roleService.GetAllRole();
            List<RoleModel> roleVMList = new List<RoleModel>();

            foreach (var role in roleListObj)
            {
                if (role.Level <= UserSession.GetUserFromSession().Level)
                    roleVMList.Add(PrepareRoleModel(role));
                else if (UserSession.IsAdmin())
                    roleVMList.Add(PrepareRoleModel(role));
            }
            return Json(roleVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActiveRoleList()
        {
            var roleListObj = this.roleService.GetAllRole().Where(x => x.IsActive == true);
            List<RoleModel> roleVMList = new List<RoleModel>();

            foreach (var role in roleListObj)
            {
                if (role.Id != 1)
                    roleVMList.Add(PrepareRoleModel(role));
                else if (UserSession.IsAdmin())
                    roleVMList.Add(PrepareRoleModel(role));
            }
            return Json(roleVMList, JsonRequestBehavior.AllowGet);
        }

        private static RoleModel PrepareRoleModel(Role role)
        {
            RoleModel roleTemp = new RoleModel();
            roleTemp.Id = role.Id;
            roleTemp.Name = role.Name;
            if (role.IsActive != null)
                roleTemp.IsActive = role.IsActive.Value;
            roleTemp.Level = role.Level.Value;
            return roleTemp;
        }

        public JsonResult GetRole(int id)
        {
            var role = this.roleService.GetRole(id);
            return Json(role);
        }
    }


}