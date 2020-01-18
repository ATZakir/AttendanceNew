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
    public class UpazilaController : Controller
    {
        public readonly IDistrictService districtService;
        public readonly IUpazilaService upazilaService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        protected long timeZoneOffset = UserSession.GetTimeZoneOffset();

        string cacheKey = "permission:upazila" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        // GET: /Upazila/
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
                    return View("Upazila");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            return View("~/Views/Shared/NoPermission.cshtml");
        }

        public UpazilaController(IUpazilaService upazilaService, IDistrictService districtService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.upazilaService = upazilaService;
            this.districtService = districtService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        [HttpPost]
        public JsonResult CreateUpazila(Upazila upazila)
        {
            const string url = "/Upazila/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey);
            if (permission == null)
                permission = roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            var isSuccess = false;
            var message = string.Empty;
            var isNew = upazila.Id == 0 ? true : false;

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(upazila))
                    {
                        if (this.upazilaService.CreateUpazila(upazila))
                        {
                            isSuccess = true;
                            message = Resources.ResourceUpazila.MsgUpazilaSaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceUpazila.MsgUpazilaSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceUpazila.MsgDuplicateUpazila;
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
                    if (this.upazilaService.UpdateUpazila(upazila))
                    {
                        isSuccess = true;
                        message = Resources.ResourceUpazila.MsgUpazilaUpdateSuccessful;
                    }
                    else
                    {
                        message = Resources.ResourceUpazila.MsgUpazilaUpdateFailed;
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
        private bool CheckIsExist(Upazila upazila)
        {
            return this.upazilaService.CheckIsExist(upazila);
        }

        [HttpPost]
        public JsonResult DeleteUpazila(Upazila upazila)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/Upazila/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                isSuccess = this.upazilaService.DeleteUpazila(upazila.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceUpazila.MsgUpazilaDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceUpazila.MsgUpazilaDeleteFailed;
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

        public JsonResult GetUpazilaList()
        {
            var upazilaListObj = this.upazilaService.GetAllUpazila();
            List<UpazilaModel> upazilaVMList = new List<UpazilaModel>();

            foreach (var upazila in upazilaListObj)
            {
                upazilaVMList.Add(PrepareUpazilaModel(upazila));
            }
            return Json(upazilaVMList, JsonRequestBehavior.AllowGet);
        }

        private UpazilaModel PrepareUpazilaModel(Upazila upazila)
        {
            UpazilaModel upazilaTemp = new UpazilaModel();
            upazilaTemp.Id = upazila.Id;
            upazilaTemp.Name = upazila.Name;
            upazilaTemp.DistrictId = upazila.DistrictId;
            if (upazilaTemp.DistrictId > 0)
            {
                var district = districtService.GetDistrict(Convert.ToInt32(upazila.DistrictId));
                upazilaTemp.DistrictName = district.Name;
                upazilaTemp.DivisionId = district.DivisionId;
                upazilaTemp.DivisionName = district.Division.Name;
            }
            if (upazila.IsActive != null)
                upazilaTemp.IsActive = upazila.IsActive.Value;
            return upazilaTemp;
        }

        public JsonResult GetActiveUpazilaList()
        {
            var upazilaListObj = this.upazilaService.GetAllUpazila().Where(x => x.IsActive == true);
            List<UpazilaModel> upazilaVMList = new List<UpazilaModel>();

            foreach (var upazila in upazilaListObj)
            {
                upazilaVMList.Add(PrepareUpazilaModel(upazila));
            }
            return Json(upazilaVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllUpazilaListByDistrict(int id)
        {
            var upazilaListObj = this.upazilaService.GetAllUpazila().Where(x => x.DistrictId==id && 
            x.IsActive == true || x.Id == 0);
            List<UpazilaModel> upazilaVMList = new List<UpazilaModel>();

            foreach (var upazila in upazilaListObj)
            {
                upazilaVMList.Add(PrepareUpazilaModel(upazila));
            }
            return Json(upazilaVMList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllUpazilaListByDistrictWithoutAll(int id)
        {
            var upazilaListObj = this.upazilaService.GetAllUpazila().Where(x => x.DistrictId == id 
            && x.IsActive == true );
            List<UpazilaModel> upazilaVMList = new List<UpazilaModel>();

            foreach (var upazila in upazilaListObj)
            {
                upazilaVMList.Add(PrepareUpazilaModel(upazila));
            }
            return Json(upazilaVMList, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetUpazila(int id)
        {
            var upazila = this.upazilaService.GetUpazila(id);
            return Json(upazila);
        }
    }

}