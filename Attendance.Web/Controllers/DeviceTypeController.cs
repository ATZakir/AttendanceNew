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
    public class DeviceTypeController : Controller
    {
        public readonly IDeviceTypeService deviceTypeService;
        public readonly ISubModuleItemService subModuleItemService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        public DeviceTypeController(IDeviceTypeService deviceTypeService, ISubModuleItemService subModuleItemService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.deviceTypeService = deviceTypeService;
            this.subModuleItemService = subModuleItemService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        string cacheKey = "permission:deviceType" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        
        // GET: /DeviceType/
        public ActionResult Index()
        {
            const string url = "/DeviceType/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId) ;

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set(cacheKey, permission, 240);
                    return View("DeviceType");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            
            return View("~/Views/Shared/NoPermission.cshtml");
        }


        public ActionResult DeviceType()
        {
            return View();
        }



        [HttpPost]
        public JsonResult CreateDeviceType(DeviceType deviceType)
        {
            var isSuccess = false;
            var message = string.Empty;
            var isNew = deviceType.Id == 0 ? true : false;
            const string url = "/DeviceType/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(deviceType))
                    {
                        if (this.deviceTypeService.CreateDeviceType(deviceType))
                        {
                            isSuccess = true;
                            message = Resources.ResourceDeviceType.MsgDeviceTypeSaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceDeviceType.MsgDeviceTypeSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceDeviceType.MsgDuplicateDeviceType;
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
                    if (this.deviceTypeService.UpdateDeviceType(deviceType))
                    {
                        isSuccess = true;
                        message = Resources.ResourceDeviceType.MsgDeviceTypeUpdateSuccessful;
                    }
                    else
                    {
                        message = Resources.ResourceDeviceType.MsgDeviceTypeUpdateFailed;
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

        private bool CheckIsExist(Model.DeviceType deviceType)
        {
            return this.deviceTypeService.CheckIsExist(deviceType);
        }

        [HttpPost]
        public JsonResult DeleteDeviceType(DeviceType deviceType)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/DeviceType/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);
            
            if (permission.DeleteOperation == true)
            {
                isSuccess = this.deviceTypeService.DeleteDeviceType(deviceType.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceDeviceType.MsgDeviceTypeDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceDeviceType.MsgDeviceTypeDeleteFailed;
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

        public JsonResult GetDeviceTypeList()
        {
            var deviceTypeListObj = this.deviceTypeService.GetAllDeviceType();
            List<DeviceTypeModel> deviceTypeVMList = new List<DeviceTypeModel>();

            foreach (var deviceType in deviceTypeListObj)
            {
                deviceTypeVMList.Add(PrepareDeviceTypeModel(deviceType));
            }

            return Json(deviceTypeVMList, JsonRequestBehavior.AllowGet);
        }

        private static DeviceTypeModel PrepareDeviceTypeModel(DeviceType deviceType)
        {
            DeviceTypeModel deviceTypeTemp = new DeviceTypeModel();
            deviceTypeTemp.Id = deviceType.Id;
            deviceTypeTemp.Name = deviceType.Name;
            if (deviceType.IsActive != null)
                deviceTypeTemp.IsActive = deviceType.IsActive.Value;
            return deviceTypeTemp;
        }

        public JsonResult GetActiveDeviceTypeList()
        {
            var deviceTypeListObj = this.deviceTypeService.GetAllDeviceType().Where(x => x.IsActive == true);
            List<DeviceTypeModel> deviceTypeVMList = new List<DeviceTypeModel>();

            foreach (var deviceType in deviceTypeListObj)
            {
                deviceTypeVMList.Add(PrepareDeviceTypeModel(deviceType));
            }

            return Json(deviceTypeVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDeviceType(int id)
        {
            var deviceType = this.deviceTypeService.GetDeviceType(id);
            return Json(deviceType);
        }

    }    
}