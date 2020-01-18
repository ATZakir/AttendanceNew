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
    public class DutyReasonController : Controller
    {
        public readonly IDutyReasonService dutyReasonService;
        public readonly ISubModuleItemService subModuleItemService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        public DutyReasonController(IDutyReasonService dutyReasonService, ISubModuleItemService subModuleItemService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.dutyReasonService = dutyReasonService;
            this.subModuleItemService = subModuleItemService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        string cacheKey = "permission:dutyReason" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        
        // GET: /DutyReason/
        public ActionResult Index()
        {
            const string url = "/DutyReason/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId) ;

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set(cacheKey, permission, 240);
                    return View("DutyReason");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            
            return View("~/Views/Shared/NoPermission.cshtml");
        }


        public ActionResult DutyReason()
        {
            return View();
        }



        [HttpPost]
        public JsonResult CreateDutyReason(DutyReason dutyReason)
        {
            var isSuccess = false;
            var message = string.Empty;
            var isNew = dutyReason.Id == 0 ? true : false;
            const string url = "/DutyReason/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(dutyReason))
                    {
                        if (this.dutyReasonService.CreateDutyReason(dutyReason))
                        {
                            isSuccess = true;
                            message = Resources.ResourceDutyReason.MsgDutyReasonSaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceDutyReason.MsgDutyReasonSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceDutyReason.MsgDuplicateDutyReason;
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
                    if (this.dutyReasonService.UpdateDutyReason(dutyReason))
                    {
                        isSuccess = true;
                        message = Resources.ResourceDutyReason.MsgDutyReasonUpdateSuccessful;
                    }
                    else
                    {
                        message = Resources.ResourceDutyReason.MsgDutyReasonUpdateFailed;
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

        private bool CheckIsExist(Model.DutyReason dutyReason)
        {
            return this.dutyReasonService.CheckIsExist(dutyReason);
        }

        [HttpPost]
        public JsonResult DeleteDutyReason(DutyReason dutyReason)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/DutyReason/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);
            
            if (permission.DeleteOperation == true)
            {
                isSuccess = this.dutyReasonService.DeleteDutyReason(dutyReason.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceDutyReason.MsgDutyReasonDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceDutyReason.MsgDutyReasonDeleteFailed;
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

        public JsonResult GetDutyReasonList()
        {
            var dutyReasonListObj = this.dutyReasonService.GetAllDutyReason();
            List<DutyReasonModel> dutyReasonVMList = new List<DutyReasonModel>();

            foreach (var dutyReason in dutyReasonListObj)
            {
                dutyReasonVMList.Add(PrepareDutyReasonModel(dutyReason));
            }

            return Json(dutyReasonVMList, JsonRequestBehavior.AllowGet);
        }

        private static DutyReasonModel PrepareDutyReasonModel(DutyReason dutyReason)
        {
            DutyReasonModel dutyReasonTemp = new DutyReasonModel();
            dutyReasonTemp.Id = dutyReason.Id;
            dutyReasonTemp.Name = dutyReason.Name;
            if (dutyReason.IsActive != null)
                dutyReasonTemp.IsActive = dutyReason.IsActive.Value;
            return dutyReasonTemp;
        }

        public JsonResult GetActiveDutyReasonList()
        {
            var dutyReasonListObj = this.dutyReasonService.GetAllDutyReason().Where(x => x.IsActive == true);
            List<DutyReasonModel> dutyReasonVMList = new List<DutyReasonModel>();

            foreach (var dutyReason in dutyReasonListObj)
            {
                dutyReasonVMList.Add(PrepareDutyReasonModel(dutyReason));
            }

            return Json(dutyReasonVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDutyReason(int id)
        {
            var dutyReason = this.dutyReasonService.GetDutyReason(id);
            return Json(dutyReason);
        }

    }    
}