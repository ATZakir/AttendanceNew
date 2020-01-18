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
    public class DeviceSetupController : Controller
    {
        public readonly IDistrictService districtService;
        public readonly IDeviceSetupService deviceSetupService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        protected long timeZoneOffset = UserSession.GetTimeZoneOffset();

        string cacheKey = "permission:deviceSetup" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        // GET: /DeviceSetup/
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
                    if (UserSession.IsSchoolUser())
                    {
                        ViewBag.SchoolId = UserSession.GetCurrentUserSchool();
                        return View("DeviceSetup");
                    }                        
                    else
                        return View("DeviceSetup2");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            return View("~/Views/Shared/NoPermission.cshtml");
        }

        public DeviceSetupController(IDeviceSetupService deviceSetupService, IDistrictService districtService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.deviceSetupService = deviceSetupService;
            this.districtService = districtService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        [HttpPost]
        public JsonResult CreateDeviceSetup(DeviceSetup deviceSetup)
        {
            const string url = "/DeviceSetup/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey);
            if (permission == null)
                permission = roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            var isSuccess = false;
            var message = string.Empty;
            var isNew = deviceSetup.Id == 0 ? true : false;

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(deviceSetup))
                    {
                        if (this.deviceSetupService.CreateDeviceSetup(deviceSetup))
                        {
                            isSuccess = true;
                            message = Resources.ResourceDeviceSetup.MsgDeviceSetupSaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceDeviceSetup.MsgDeviceSetupSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceDeviceSetup.MsgDuplicateDeviceSetup;
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
                    if (this.deviceSetupService.UpdateDeviceSetup(deviceSetup))
                    {
                        isSuccess = true;
                        message = Resources.ResourceDeviceSetup.MsgDeviceSetupUpdateSuccessful;
                    }
                    else
                    {
                        message = Resources.ResourceDeviceSetup.MsgDeviceSetupUpdateFailed;
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
        private bool CheckIsExist(DeviceSetup deviceSetup)
        {
            return this.deviceSetupService.CheckIsExist(deviceSetup);
        }

        [HttpPost]
        public JsonResult DeleteDeviceSetup(DeviceSetup deviceSetup)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/DeviceSetup/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                isSuccess = this.deviceSetupService.DeleteDeviceSetup(deviceSetup.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceDeviceSetup.MsgDeviceSetupDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceDeviceSetup.MsgDeviceSetupDeleteFailed;
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

        public JsonResult GetDeviceSetupList()
        {
            var deviceSetupListObj = this.deviceSetupService.GetAllDeviceSetup();
            List<DeviceSetupModel> deviceSetupVMList = new List<DeviceSetupModel>();

            foreach (var deviceSetup in deviceSetupListObj)
            {
                deviceSetupVMList.Add(PrepareDeviceSetupModel(deviceSetup));
            }
            return Json(deviceSetupVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDeviceSetupList2()
        {
            var schoolIds = UserSession.GetUserSchoolAccess();
            if (schoolIds == null || schoolIds.Count == 0) Json(new List<DeviceSetupModel>(), JsonRequestBehavior.AllowGet);

            var deviceSetupListObj = this.deviceSetupService.GetAllDeviceSetup().Where(x => schoolIds.Contains(x.SchoolId.Value));
            List<DeviceSetupModel> deviceSetupVMList = new List<DeviceSetupModel>();

            foreach (var deviceSetup in deviceSetupListObj)
            {
                deviceSetupVMList.Add(PrepareDeviceSetupModel(deviceSetup));
            }
            return Json(deviceSetupVMList, JsonRequestBehavior.AllowGet);
        }

        private DeviceSetupModel PrepareDeviceSetupModel(DeviceSetup deviceSetup)
        {
            DeviceSetupModel deviceSetupTemp = new DeviceSetupModel();
            deviceSetupTemp.Id = deviceSetup.Id;
            deviceSetupTemp.Name = deviceSetup.Name;
            deviceSetupTemp.Code = deviceSetup.Code;
            deviceSetupTemp.Gate = deviceSetup.Gate;
            deviceSetupTemp.IPAddress = deviceSetup.IPAddress;
            deviceSetupTemp.Location = deviceSetup.Location;
            deviceSetupTemp.Port = deviceSetup.Port;
            deviceSetupTemp.TypeId = deviceSetup.TypeId;
            if (deviceSetupTemp.TypeId > 0)
            {
                deviceSetupTemp.DeviceTypeName = deviceSetup.DeviceType.Name;
            }
            deviceSetupTemp.SchoolId = deviceSetup.SchoolId;
            if (deviceSetupTemp.SchoolId > 0)
            {
                deviceSetupTemp.SchoolName = deviceSetup.School.Name;
                deviceSetupTemp.UpazilaId = deviceSetup.School.UpazilaId;
                if (deviceSetupTemp.UpazilaId > 0)
                {
                    //deviceSetupTemp.UpazilaName = deviceSetup.School.Upazila.Name;
                    deviceSetupTemp.DistrictId = deviceSetup.School.Upazila.DistrictId;
                    if (deviceSetupTemp.DistrictId > 0)
                    {
                        //deviceSetupTemp.DistrictName = deviceSetup.School.Upazila.District.Name;
                        deviceSetupTemp.DivisionId = deviceSetup.School.Upazila.District.DivisionId;
                        if (deviceSetupTemp.DivisionId > 0)
                        {
                            //deviceSetupTemp.DivisionName = deviceSetup.School.Upazila.District.Division.Name;
                            deviceSetupTemp.GeoName = deviceSetup.School.Upazila.District.Division.Name + " - " + deviceSetup.School.Upazila.District.Name + " - " + deviceSetup.School.Upazila.Name;
                        }
                    }
                }
            }
            if (deviceSetup.IsActive != null)
                deviceSetupTemp.IsActive = deviceSetup.IsActive.Value;
            return deviceSetupTemp;
        }

        public JsonResult GetActiveDeviceSetupList()
        {
            var deviceSetupListObj = this.deviceSetupService.GetAllDeviceSetup()/*.Where(x => x.IsActive == true)*/;
            List<DeviceSetupModel> deviceSetupVMList = new List<DeviceSetupModel>();

            foreach (var deviceSetup in deviceSetupListObj)
            {
                deviceSetupVMList.Add(PrepareDeviceSetupModel(deviceSetup));
            }
            return Json(deviceSetupVMList, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult GetDeviceSetup(int id)
        {
            var deviceSetup = this.deviceSetupService.GetDeviceSetup(id);
            return Json(deviceSetup);
        }
    }

}