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
    public class DutyShiftMasterController : Controller
    {
        public readonly IDutyShiftMasterService dutyShiftMasterService;
        public readonly IDutyShiftService dutyShiftService;
        public readonly ISubModuleItemService subModuleItemService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        public DutyShiftMasterController(IDutyShiftMasterService dutyShiftMasterService, ISubModuleItemService subModuleItemService, IRoleSubModuleItemService roleSubModuleItemService, IDutyShiftService dutyShiftService)
        {
            this.dutyShiftMasterService = dutyShiftMasterService;
            this.subModuleItemService = subModuleItemService;
            this.roleSubModuleItemService = roleSubModuleItemService;
            this.dutyShiftService = dutyShiftService;
        }

        string cacheKey = "permission:dutyShiftMaster" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        
        // GET: /DutyShiftMaster/
        public ActionResult Index()
        {
            const string url = "/DutyShiftMaster/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId) ;

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set(cacheKey, permission, 240);
                    return View("DutyShiftMaster");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            
            return View("~/Views/Shared/NoPermission.cshtml");
        }


        public ActionResult DutyShiftMaster()
        {
            return View();
        }



        [HttpPost]
        public JsonResult CreateDutyShiftMaster(DutyShiftMaster dutyShiftMaster)
        {
            var isSuccess = false;
            var message = string.Empty;
            var isNew = dutyShiftMaster.Id == 0 ? true : false;
            const string url = "/DutyShiftMaster/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(dutyShiftMaster))
                    {
                        if (this.dutyShiftMasterService.CreateDutyShiftMaster(dutyShiftMaster))
                        {
                            isSuccess = true;
                            message = Resources.ResourceDutyShiftMaster.MsgDutyShiftMasterSaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceDutyShiftMaster.MsgDutyShiftMasterSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceDutyShiftMaster.MsgDuplicateDutyShiftMaster;
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
                    if (this.dutyShiftMasterService.UpdateDutyShiftMaster(dutyShiftMaster))
                    {
                        isSuccess = true;
                        message = Resources.ResourceDutyShiftMaster.MsgDutyShiftMasterUpdateSuccessful;
                    }
                    else
                    {
                        message = Resources.ResourceDutyShiftMaster.MsgDutyShiftMasterUpdateFailed;
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

        private bool CheckIsExist(Model.DutyShiftMaster dutyShiftMaster)
        {
            return this.dutyShiftMasterService.CheckIsExist(dutyShiftMaster);
        }

        [HttpPost]
        public JsonResult DeleteDutyShiftMaster(DutyShiftMaster dutyShiftMaster)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/DutyShiftMaster/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);
            
            if (permission.DeleteOperation == true)
            {
                isSuccess = this.dutyShiftMasterService.DeleteDutyShiftMaster(dutyShiftMaster.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceDutyShiftMaster.MsgDutyShiftMasterDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceDutyShiftMaster.MsgDutyShiftMasterDeleteFailed;
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

        public JsonResult GetDutyShiftMasterList()
        {
            var dutyShiftMasterListObj = this.dutyShiftMasterService.GetAllDutyShiftMaster();
            List<DutyShiftMasterModel> dutyShiftMasterVMList = new List<DutyShiftMasterModel>();

            foreach (var dutyShiftMaster in dutyShiftMasterListObj)
            {
                dutyShiftMasterVMList.Add(PrepareDutyShiftMasterModel(dutyShiftMaster));
            }

            return Json(dutyShiftMasterVMList, JsonRequestBehavior.AllowGet);
        }

        private static DutyShiftMasterModel PrepareDutyShiftMasterModel(DutyShiftMaster dutyShiftMaster)
        {
            DutyShiftMasterModel dutyShiftMasterTemp = new DutyShiftMasterModel();
            dutyShiftMasterTemp.Id = dutyShiftMaster.Id;
            dutyShiftMasterTemp.Name = dutyShiftMaster.Name;
            if (dutyShiftMaster.IsActive != null)
                dutyShiftMasterTemp.IsActive = dutyShiftMaster.IsActive.Value;
            return dutyShiftMasterTemp;
        }

        public JsonResult GetActiveDutyShiftMasterList()
        {
            var dutyShiftMasterListObj = this.dutyShiftMasterService.GetAllDutyShiftMaster().Where(x => x.IsActive == true);
            List<DutyShiftMasterModel> dutyShiftMasterVMList = new List<DutyShiftMasterModel>();

            foreach (var dutyShiftMaster in dutyShiftMasterListObj)
            {
                dutyShiftMasterVMList.Add(PrepareDutyShiftMasterModel(dutyShiftMaster));
            }

            return Json(dutyShiftMasterVMList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDutyShiftListbySchoolId(int id)
        {
            var dutyShiftListObj = this.dutyShiftService.GetAllDutyShift().Where(x => x.SchoolId == id);
            List<DutyShiftModel> dutyShiftVMList = new List<DutyShiftModel>();

            foreach (var dutyShift in dutyShiftListObj)
            {
                DutyShiftModel dutyShiftTemp = new DutyShiftModel();
                dutyShiftTemp.Id = dutyShift.Id;
                dutyShiftTemp.DutyShiftMasterName = dutyShift.DutyShiftMaster.Name;

                dutyShiftVMList.Add(dutyShiftTemp);
            }
            return Json(dutyShiftVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDutyShiftMaster(int id)
        {
            var dutyShiftMaster = this.dutyShiftMasterService.GetDutyShiftMaster(id);
            return Json(dutyShiftMaster);
        }

    }    
}