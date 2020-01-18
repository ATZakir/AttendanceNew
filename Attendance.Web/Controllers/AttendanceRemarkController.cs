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
    public class AttendanceRemarkController : Controller
    {
        public readonly IAttendanceRemarkService attendanceRemarksService;
        public readonly ISubModuleItemService subModuleItemService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        public AttendanceRemarkController(IAttendanceRemarkService attendanceRemarksService, ISubModuleItemService subModuleItemService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.attendanceRemarksService = attendanceRemarksService;
            this.subModuleItemService = subModuleItemService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        string cacheKey = "permission:attendanceRemarks" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        
        // GET: /AttendanceRemark/
        public ActionResult Index()
        {
            const string url = "/AttendanceRemark/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId) ;

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set(cacheKey, permission, 240);
                    return View("AttendanceRemark");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            
            return View("~/Views/Shared/NoPermission.cshtml");
        }

        public ActionResult AttendanceRemark()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CreateAttendanceRemark(AttendanceRemark attendanceRemarks)
        {
            var isSuccess = false;
            var message = string.Empty;
            var isNew = attendanceRemarks.Id == 0 ? true : false;
            const string url = "/AttendanceRemark/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(attendanceRemarks))
                    {
                        if (this.attendanceRemarksService.CreateAttendanceRemark(attendanceRemarks))
                        {
                            isSuccess = true;
                            message = Resources.ResourceAttendanceRemark.MsgAttendanceRemarkSaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceAttendanceRemark.MsgAttendanceRemarkSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceAttendanceRemark.MsgDuplicateAttendanceRemark;
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
                    if (this.attendanceRemarksService.UpdateAttendanceRemark(attendanceRemarks))
                    {
                        isSuccess = true;
                        message = Resources.ResourceAttendanceRemark.MsgAttendanceRemarkUpdateSuccessful;
                    }
                    else
                    {
                        message = Resources.ResourceAttendanceRemark.MsgAttendanceRemarkUpdateFailed;
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

        private bool CheckIsExist(AttendanceRemark attendanceRemarks)
        {
            return this.attendanceRemarksService.CheckIsExist(attendanceRemarks);
        }

        [HttpPost]
        public JsonResult DeleteAttendanceRemark(AttendanceRemark attendanceRemarks)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/AttendanceRemark/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);
            
            if (permission.DeleteOperation == true)
            {
                isSuccess = this.attendanceRemarksService.DeleteAttendanceRemark(attendanceRemarks.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceAttendanceRemark.MsgAttendanceRemarkDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceAttendanceRemark.MsgAttendanceRemarkDeleteFailed;
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

        public JsonResult GetAttendanceRemarkList()
        {
            var attendanceRemarksListObj = this.attendanceRemarksService.GetAllAttendanceRemark();
            List<AttendanceRemarkModel> attendanceRemarksVMList = new List<AttendanceRemarkModel>();

            foreach (var attendanceRemarks in attendanceRemarksListObj)
            {
                attendanceRemarksVMList.Add(PrepareAttendanceRemarkModel(attendanceRemarks));
            }

            return Json(attendanceRemarksVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActiveAttendanceRemarkList()
        {
            var attendanceRemarksListObj = this.attendanceRemarksService.GetAllAttendanceRemark().Where(x=>x.IsActive==true);
            List<AttendanceRemarkModel> attendanceRemarksVMList = new List<AttendanceRemarkModel>();

            foreach (var attendanceRemarks in attendanceRemarksListObj)
            {
                attendanceRemarksVMList.Add(PrepareAttendanceRemarkModel(attendanceRemarks));
            }

            return Json(attendanceRemarksVMList, JsonRequestBehavior.AllowGet);
        }

        private static AttendanceRemarkModel PrepareAttendanceRemarkModel(AttendanceRemark attendanceRemarks)
        {
            AttendanceRemarkModel attendanceRemarksTemp = new AttendanceRemarkModel();
            attendanceRemarksTemp.Id = attendanceRemarks.Id;
            attendanceRemarksTemp.Name = attendanceRemarks.Name;
            attendanceRemarksTemp.IsActive = attendanceRemarks.IsActive;
            return attendanceRemarksTemp;
        }

        public JsonResult GetAttendanceRemark(int id)
        {
            var attendanceRemarks = this.attendanceRemarksService.GetAttendanceRemark(id);
            return Json(attendanceRemarks);
        }
    }    
}