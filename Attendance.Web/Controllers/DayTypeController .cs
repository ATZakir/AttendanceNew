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
    public class DayTypeController : Controller
    {
        public readonly IDayTypeService dayTypeService;
        public readonly ISubModuleItemService subModuleItemService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        public DayTypeController(IDayTypeService dayTypeService, ISubModuleItemService subModuleItemService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.dayTypeService = dayTypeService;
            this.subModuleItemService = subModuleItemService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        string cacheKey = "permission:dayType" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        
        // GET: /DayType/
        public ActionResult Index()
        {
            const string url = "/DayType/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId) ;

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set(cacheKey, permission, 240);
                    return View("DayType");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            
            return View("~/Views/Shared/NoPermission.cshtml");
        }


        public ActionResult DayType()
        {
            return View();
        }



        [HttpPost]
        public JsonResult CreateDayType(DayType dayType)
        {
            var isSuccess = false;
            var message = string.Empty;
            var dayTypeObj = dayTypeService.GetDayType(dayType.Id);
            var isNew = dayTypeObj == null;
            const string url = "/DayType/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(dayType))
                    {
                        if (this.dayTypeService.CreateDayType(dayType))
                        {
                            isSuccess = true;
                            message = Resources.ResourceDayType.MsgDayTypeSaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceDayType.MsgDayTypeSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceDayType.MsgDuplicateDayType;
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
                    if (this.dayTypeService.UpdateDayType(dayType))
                    {
                        isSuccess = true;
                        message = Resources.ResourceDayType.MsgDayTypeUpdateSuccessful;
                    }
                    else
                    {
                        message = Resources.ResourceDayType.MsgDayTypeUpdateFailed;
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

        private bool CheckIsExist(Model.DayType dayType)
        {
            return this.dayTypeService.CheckIsExist(dayType);
        }

        [HttpPost]
        public JsonResult DeleteDayType(DayType dayType)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/DayType/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);
            
            if (permission.DeleteOperation == true)
            {
                isSuccess = this.dayTypeService.DeleteDayType(dayType.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceDayType.MsgDayTypeDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceDayType.MsgDayTypeDeleteFailed;
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

        public JsonResult GetDayTypeList()
        {
            var dayTypeListObj = this.dayTypeService.GetAllDayType();
            List<DayTypeModel> dayTypeVMList = new List<DayTypeModel>();

            foreach (var dayType in dayTypeListObj)
            {
                dayTypeVMList.Add(PrepareDayTypeModel(dayType));
            }

            return Json(dayTypeVMList, JsonRequestBehavior.AllowGet);
        }

        private static DayTypeModel PrepareDayTypeModel(DayType dayType)
        {
            DayTypeModel dayTypeTemp = new DayTypeModel();
            dayTypeTemp.Id = dayType.Id;
            dayTypeTemp.DayType = dayType.DayType1;
            return dayTypeTemp;
        }

        public JsonResult GetActiveDayTypeList()
        {
            var dayTypeListObj = this.dayTypeService.GetAllDayType();
            List<DayTypeModel> dayTypeVMList = new List<DayTypeModel>();

            foreach (var dayType in dayTypeListObj)
            {
                dayTypeVMList.Add(PrepareDayTypeModel(dayType));
            }

            return Json(dayTypeVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDayType(int id)
        {
            var dayType = this.dayTypeService.GetDayType(id);
            return Json(dayType);
        }

    }    
}