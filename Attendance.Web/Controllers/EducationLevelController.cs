using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Attendance.CachingService;
using Attendance.ClientModel;
using Attendance.Model;
using Attendance.Service;


namespace Attendance.Web.Controllers
{
    public class EducationLevelController : Controller
    {
        public readonly IEducationLevelService educationLevelService;
        public readonly ISubModuleItemService subModuleItemService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        public EducationLevelController(IEducationLevelService educationLevelService, ISubModuleItemService subModuleItemService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.educationLevelService = educationLevelService;
            this.subModuleItemService = subModuleItemService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        string cacheKey = "permission:educationLevel" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        
        // GET: /EducationLevel/
        public ActionResult Index()
        {
            const string url = "/EducationLevel/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId) ;

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set(cacheKey, permission, 240);
                    return View("EducationLevel");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            
            return View("~/Views/Shared/NoPermission.cshtml");
        }


        public ActionResult EducationLevel()
        {
            return View();
        }



        [HttpPost]
        public JsonResult CreateEducationLevel(EducationLevel educationLevel)
        {
            var isSuccess = false;
            var message = string.Empty;
            var isNew = educationLevel.Id == 0 ? true : false;
            const string url = "/EducationLevel/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(educationLevel))
                    {
                        if (this.educationLevelService.CreateEducationLevel(educationLevel))
                        {
                            isSuccess = true;
                            message = Resources.ResourceEducationLevel.MsgEducationLevelSaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceEducationLevel.MsgEducationLevelSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceEducationLevel.MsgDuplicateEducationLevel;
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
                    if (this.educationLevelService.UpdateEducationLevel(educationLevel))
                    {
                        isSuccess = true;
                        message = Resources.ResourceEducationLevel.MsgEducationLevelUpdateSuccessful;
                    }
                    else
                    {
                        message = Resources.ResourceEducationLevel.MsgEducationLevelUpdateFailed;
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

        private bool CheckIsExist(Model.EducationLevel educationLevel)
        {
            return this.educationLevelService.CheckIsExist(educationLevel);
        }

        [HttpPost]
        public JsonResult DeleteEducationLevel(EducationLevel educationLevel)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/EducationLevel/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);
            
            if (permission.DeleteOperation == true)
            {
                isSuccess = this.educationLevelService.DeleteEducationLevel(educationLevel.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceEducationLevel.MsgEducationLevelDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceEducationLevel.MsgEducationLevelDeleteFailed;
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

        public JsonResult GetEducationLevelList()
        {
            var educationLevelListObj = this.educationLevelService.GetAllEducationLevel();
            List<EducationLevelModel> educationLevelVMList = new List<EducationLevelModel>();

            foreach (var educationLevel in educationLevelListObj)
            {
                educationLevelVMList.Add(PrepareEducationLevelModel(educationLevel));
            }

            return Json(educationLevelVMList, JsonRequestBehavior.AllowGet);
        }

        private static EducationLevelModel PrepareEducationLevelModel(EducationLevel educationLevel)
        {
            EducationLevelModel educationLevelTemp = new EducationLevelModel();
            educationLevelTemp.Id = educationLevel.Id;
            educationLevelTemp.Name = educationLevel.Name;
            if (educationLevel.IsActive != null)
                educationLevelTemp.IsActive = educationLevel.IsActive.Value;
            return educationLevelTemp;
        }

        public JsonResult GetActiveEducationLevelList()
        {
            var educationLevelListObj = this.educationLevelService.GetAllEducationLevel().Where(x => x.IsActive == true);
            List<EducationLevelModel> educationLevelVMList = new List<EducationLevelModel>();

            foreach (var educationLevel in educationLevelListObj)
            {
                educationLevelVMList.Add(PrepareEducationLevelModel(educationLevel));
            }

            return Json(educationLevelVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEducationLevel(int id)
        {
            var educationLevel = this.educationLevelService.GetEducationLevel(id);
            return Json(educationLevel);
        }

    }    
}