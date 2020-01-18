using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Attendance.Model;
using Attendance.Service;
using System.Globalization;
using Attendance.CachingService;
using Attendance.Web.Helpers;
using Attendance.ClientModel;

namespace Attendance.Web.Controllers
{
    public class ModuleController : Controller
    {
        public readonly IModuleService moduleService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();
        protected long timeZoneOffset = UserSession.GetTimeZoneOffset();

        string cacheKey = "permission:module"/*+ Helpers.UserSession.GetUserFromSession().RoleId*/;
        RoleSubModuleItem permission = null;

        public ModuleController(IModuleService moduleService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.moduleService = moduleService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        // GET: /Module/
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
                    cacheProvider.Set(cacheKey, permission, 240);
                    return View("Module");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            return View("~/Views/Shared/NoPermission.cshtml");
        }

  
        public ActionResult Module()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CreateModule(Module module)
        {
            const string url = "/Module/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey);
            if (permission == null)
                permission = roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            var isSuccess = false;
            var message = string.Empty;
            var isNew = module.Id == 0 ? true : false;

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(module))
                    {

                        if (this.moduleService.CreateModule(module))
                        {
                            isSuccess = true;
                            message = "Module saved successfully!";
                        }
                        else
                        {
                            message = "Module could not be saved!";
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = "Can't save. Same module name found!";
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
                    if (this.moduleService.UpdateModule(module))
                    {
                        isSuccess = true;
                        message = "Module updated successfully!";
                    }
                    else
                    {
                        message = "Module could not be updated!";
                    }
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
        private bool CheckIsExist(Model.Module module)
        {
            return this.moduleService.CheckIsExist(module);
        }

        [HttpPost]
        public JsonResult DeleteModule(Module module)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/Moduel/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                isSuccess = this.moduleService.DeleteModule(module.Id);
                if (isSuccess)
                {
                    message = "Module deleted successfully!";

                }
                else
                {
                    message = "Module can't be deleted!";
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

        

        public JsonResult GetAllModuleList()
        {
            var LoggedUser = UserSession.GetUserFromSession();
            if (LoggedUser != null)
            {
                var moduleListObj = this.moduleService.GetAllModule();
                List<ModuleModel> moduleVMList = new List<ModuleModel>();

                foreach (var module in moduleListObj)
                {
                    ModuleModel moduleTemp = new ModuleModel();
                    moduleTemp.Id = module.Id;
                    
                    moduleTemp.Name = module.Name;
                    moduleTemp.ImageName = module.ImageName;
                    moduleTemp.Ordering = module.Ordering;

                    if (module.IsActive != null)
                        moduleTemp.IsActive = module.IsActive.Value;

                    moduleVMList.Add(moduleTemp);
                }
                return Json(moduleVMList, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public JsonResult GetActiveModuleList()
        {
            var moduleListObj = this.moduleService.GetAllModule().Where(m=>m.IsActive==true);
            List<ModuleModel> moduleVMList = new List<ModuleModel>();

            foreach (var module in moduleListObj)
            {
                ModuleModel moduleTemp = new ModuleModel();
                moduleTemp.Id = module.Id;                    
                moduleTemp.Name = module.Name;
                moduleTemp.ImageName = module.ImageName;
                moduleTemp.Ordering = module.Ordering;

                if (module.IsActive != null)
                    moduleTemp.IsActive = module.IsActive.Value;

                moduleVMList.Add(moduleTemp);
            }
            return Json(moduleVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetModuleList()
        {
            var LoggedUser = UserSession.GetUserFromSession();
            if (LoggedUser != null)
            {
                var moduleListObj = this.moduleService.GetAllModuleByRoleId(LoggedUser.RoleId);
                List<ModuleModel> moduleVMList = new List<ModuleModel>();

                foreach (var module in moduleListObj)
                {
                    ModuleModel moduleTemp = new ModuleModel();
                    moduleTemp.Id = module.Id;
                    moduleTemp.Name = module.Name;
                    moduleTemp.ImageName = module.ImageName;
                    moduleTemp.Ordering = module.Ordering;

                    if (module.IsActive!=null)
                        moduleTemp.IsActive = module.IsActive.Value;

                    moduleVMList.Add(moduleTemp);
                }
                return Json(moduleVMList, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public JsonResult GetModuleListWithSubmodule()
        {
            var LoggedUser = UserSession.GetUserFromSession();
            if (LoggedUser != null)
            {
                var moduleListObj = this.moduleService.GetAllModuleByRoleId(LoggedUser.RoleId);
                List<ModuleModel> moduleVMList = new List<ModuleModel>();

                foreach (var module in moduleListObj)
                {
                    ModuleModel moduleTemp = new ModuleModel();
                    moduleTemp.Id = module.Id;
                    moduleTemp.Name = module.Name;
                    moduleTemp.ImageName = module.ImageName;
                    moduleTemp.Ordering = module.Ordering;

                    if (module.IsActive != null)
                        moduleTemp.IsActive = module.IsActive.Value;

                    if (module.SubModules.Any())
                    {
                        foreach (var moduleSubModule in module.SubModules)
                        {
                            var isSubModuleActive = moduleSubModule.IsActive ?? false;
                            if (isSubModuleActive)
                            {
                                var submodule = new SubModuleModel();
                                submodule.Id = moduleSubModule.Id;
                                submodule.Name = moduleSubModule.Name;
                                submodule.ModuleId = moduleSubModule.ModuleId;
                                submodule.Ordering = moduleSubModule.Ordering;


                                if (moduleSubModule.SubModuleItems.Any())
                                {
                                    foreach (var subModuleItem in moduleSubModule.SubModuleItems)
                                    {
                                        var isSubModuleItemActive = subModuleItem.IsActive ?? false;
                                        if (isSubModuleItemActive)
                                        {
                                            var subModuleItemVm = new SubModuleItemModel();
                                            subModuleItemVm.Id = subModuleItem.Id;
                                            subModuleItemVm.Ordering = subModuleItem.Ordering;
                                            subModuleItemVm.Name = subModuleItem.Name;
                                            subModuleItemVm.ModuleId = subModuleItem.SubModule.Module.Id;
                                            subModuleItemVm.SubModuleId = subModuleItem.SubModule.Id;

                                            submodule.SubModuleItems.Add(subModuleItemVm);

                                        }
                                    }
                                }

                                moduleTemp.SubModules.Add(submodule);
                            }
                            
                        }
                    }

                    moduleVMList.Add(moduleTemp);
                }
                return Json(moduleVMList, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public JsonResult GetModule(int id)
        {
            var module = this.moduleService.GetModule(id);
            return Json(module);
        }

    }


}