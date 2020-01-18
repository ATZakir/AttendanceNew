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
    public class DistrictController : Controller
    {
        public readonly IDivisionService divisionService;
        public readonly IDistrictService districtService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        protected long timeZoneOffset = UserSession.GetTimeZoneOffset();

        string cacheKey = "permission:district" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        // GET: /District/
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
                    return View("District");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            return View("~/Views/Shared/NoPermission.cshtml");
        }

        public DistrictController(IDistrictService districtService, IDivisionService divisionService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.districtService = districtService;
            this.divisionService = divisionService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        [HttpPost]
        public JsonResult CreateDistrict(District district)
        {
            const string url = "/District/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey);
            if (permission == null)
                permission = roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            var isSuccess = false;
            var message = string.Empty;
            var isNew = district.Id == 0 ? true : false;

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(district))
                    {
                        if (this.districtService.CreateDistrict(district))
                        {
                            isSuccess = true;
                            message = Resources.ResourceDistrict.MsgDistrictSaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceDistrict.MsgDistrictSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceDistrict.MsgDuplicateDistrict;
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
                    if (this.districtService.UpdateDistrict(district))
                    {
                        isSuccess = true;
                        message = Resources.ResourceDistrict.MsgDistrictUpdateSuccessful;
                    }
                    else
                    {
                        message = Resources.ResourceDistrict.MsgDistrictUpdateFailed;
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
        private bool CheckIsExist(District district)
        {
            return this.districtService.CheckIsExist(district);
        }

        [HttpPost]
        public JsonResult DeleteDistrict(District district)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/District/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                isSuccess = this.districtService.DeleteDistrict(district.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceDistrict.MsgDistrictDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceDistrict.MsgDistrictDeleteFailed;
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

        public JsonResult GetDistrictList()
        {
            var districtListObj = this.districtService.GetAllDistrict();
            List<DistrictModel> districtVMList = new List<DistrictModel>();

            foreach (var district in districtListObj)
            {
                districtVMList.Add(PrepareDistrictModel(district));
            }
            return Json(districtVMList, JsonRequestBehavior.AllowGet);
        }

        private DistrictModel PrepareDistrictModel(District district)
        {
            DistrictModel districtTemp = new DistrictModel();
            districtTemp.Id = district.Id;
            districtTemp.Name = district.Name;
            districtTemp.DivisionName = divisionService.GetDivision(Convert.ToInt32(district.DivisionId)).Name;
            districtTemp.DivisionId = district.DivisionId;
            if (district.IsActive != null)
                districtTemp.IsActive = district.IsActive.Value;
            return districtTemp;
        }

        public JsonResult GetActiveDistrictList()
        {
            var districtListObj = this.districtService.GetAllDistrict().Where(x => x.IsActive == true);
            List<DistrictModel> districtVMList = new List<DistrictModel>();

            foreach (var district in districtListObj)
            {
                districtVMList.Add(PrepareDistrictModel(district));
            }
            return Json(districtVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDistrictListByDivision(int id)
        {
            var districtListObj = this.districtService.GetAllDistrict().Where(x => x.IsActive == true && x.DivisionId==id || x.Id==0);
            List<DistrictModel> districtVMList = new List<DistrictModel>();

            foreach (var district in districtListObj)
            {
                districtVMList.Add(PrepareDistrictModel(district));
            }
            return Json(districtVMList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDistrictListByDivisionWithoutAll(int id)
        {
            var districtListObj = this.districtService.GetAllDistrictByDivisionId(id);
            List<DistrictModel> districtVMList = new List<DistrictModel>();

            foreach (var district in districtListObj)
            {
                districtVMList.Add(PrepareDistrictModel(district));
            }
            return Json(districtVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDistrict(int id)
        {
            var district = this.districtService.GetDistrict(id);
            return Json(district);
        }
    }

}