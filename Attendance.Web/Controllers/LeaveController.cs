using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Attendance.Model;
using Attendance.Service;
using Attendance.CachingService;
using Attendance.Web.Helpers;
using Attendance.ClientModel;

namespace Attendance.Web.Controllers
{
    public class LeaveController : Controller
    {
        #region prop

        public readonly IDistrictService districtService;
        public readonly ILeaveService leaveService;
        public readonly IEmployeeAttendanceSummaryService employeeAttendanceSummaryService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        protected long timeZoneOffset = UserSession.GetTimeZoneOffset();

        string cacheKey = "permission:leave" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        #endregion

        #region ctor

        public LeaveController(ILeaveService leaveService, IDistrictService districtService, IRoleSubModuleItemService roleSubModuleItemService, IEmployeeAttendanceService employeeAttendanceService, IEmployeeAttendanceSummaryService employeeAttendanceSummaryService)
        {
            this.leaveService = leaveService;
            this.districtService = districtService;
            this.roleSubModuleItemService = roleSubModuleItemService;
            this.employeeAttendanceSummaryService = employeeAttendanceSummaryService;
        }

        #endregion

        #region Utility

        private bool CreateAttendanceSummary(Leave leave)
        {
                int empId = leave.EmployeeId;
                string flag = "LA";
                return employeeAttendanceSummaryService.InsertEmployeeAttendanceSummary(empId, leave.StartDate, leave.EndDate, flag);
        }

        private bool CheckIsExist(Leave leave)
        {
            return this.leaveService.CheckIsExist(leave);
        }

        private LeaveModel PrepareLeaveModel(Leave leave)
        {
            LeaveModel leaveTemp = new LeaveModel();
            leaveTemp.Id = leave.Id;
            leaveTemp.AddressDuringLeave = leave.AddressDuringLeave;
            leaveTemp.ContactDuringLeave = leave.ContactDuringLeave;
            leaveTemp.Date = leave.Date;
            leaveTemp.EmployeeId = leave.EmployeeId;
            if (leaveTemp.EmployeeId > 0)
            {
                leaveTemp.EmployeeName = leave.Employee.FullName;
            }
            leaveTemp.EndDate = leave.EndDate.AddMinutes(timeZoneOffset).ToString("dd MMM yyyy");
            leaveTemp.LeaveReason = leave.LeaveReason;
            leaveTemp.LeaveTypeId = leave.LeaveTypeId;
            if (leaveTemp.LeaveTypeId > 0)
            {
                leaveTemp.LeaveTypeName = leave.LeaveType.Name;
            }
            leaveTemp.NoofDays = leave.NoOfDays;
            leaveTemp.RemovedBy = leave.RemovedBy;
            leaveTemp.RemovedOn = leave.RemovedOn;
            leaveTemp.StartDate = leave.StartDate.AddMinutes(timeZoneOffset).ToString("dd MMM yyyy");
            leaveTemp.Status = leave.Status;
            leaveTemp.SchoolId = leave.SchoolId;
            if (leaveTemp.SchoolId > 0)
            {
                leaveTemp.SchoolName = leave.School.Name;
                leaveTemp.UpazilaId = leave.School.UpazilaId;
                if (leaveTemp.UpazilaId > 0)
                {
                    //leaveTemp.UpazilaName = leave.School.Upazila.Name;
                    leaveTemp.DistrictId = leave.School.Upazila.DistrictId;
                    if (leaveTemp.DistrictId > 0)
                    {
                        //leaveTemp.DistrictName = leave.School.Upazila.District.Name;
                        leaveTemp.DivisionId = leave.School.Upazila.District.DivisionId;
                        if (leaveTemp.DivisionId > 0)
                        {
                            //leaveTemp.DivisionName = leave.School.Upazila.District.Division.Name;
                            leaveTemp.GeoName = leave.School.Upazila.District.Division.Name + " - " + leave.School.Upazila.District.Name + " - " + leave.School.Upazila.Name;
                        }
                    }
                }
            }
            return leaveTemp;
        }

        #endregion

        #region Action Methods


        // GET: /Leave/
        public ActionResult Index()
        {
            var url = Request.RawUrl;
            //            return View("Leave");
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
                        return View("Leave");
                    }
                    else
                        return View("Leave2");
                }
            }
            return View("~/Views/Shared/NoPermission.cshtml");
        }

        [HttpPost]
        public JsonResult CreateLeave(Leave leave)
        {
            const string url = "/Leave/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey);
            if (permission == null)
                permission = roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            var isSuccess = false;
            var message = string.Empty;
            var isNew = leave.Id == null ? true : false;

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(leave))
                    {
                        //for test purpose
                        leave.Id = UtilityService.GenerateOTP();
                        leave.Date = leave.Date.Date;
                        leave.StartDate = leave.StartDate.Date;
                        leave.EndDate = leave.EndDate.Date;
                        if (this.leaveService.CreateLeave(leave))
                        {
                            if (CreateAttendanceSummary(leave))
                            {
                                isSuccess = true;
                                message = Resources.ResourceLeave.MsgLeaveSaveSuccessful;
                            }
                        }
                        else
                        {
                            message = Resources.ResourceLeave.MsgLeaveSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceLeave.MsgDuplicateLeave;
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
                    if (this.leaveService.UpdateLeave(leave))
                    {
                        if (CreateAttendanceSummary(leave))
                        {
                            isSuccess = true;
                            message = Resources.ResourceLeave.MsgLeaveUpdateSuccessful;
                        }
                        
                    }
                    else
                    {
                        message = Resources.ResourceLeave.MsgLeaveUpdateFailed;
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


        [HttpPost]
        public JsonResult DeleteLeave(Leave leave)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/Leave/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                isSuccess = this.leaveService.DeleteLeave(leave.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceLeave.MsgLeaveDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceLeave.MsgLeaveDeleteFailed;
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

        public JsonResult GetLeaveList()
        {
            var leaveListObj = this.leaveService.GetAllLeave();
            List<LeaveModel> leaveVMList = new List<LeaveModel>();

            foreach (var leave in leaveListObj)
            {
                leaveVMList.Add(PrepareLeaveModel(leave));
            }
            return Json(leaveVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLeaveList2()
        {
            var schoolIds = UserSession.GetUserSchoolAccess();
            if (schoolIds == null || schoolIds.Count == 0) Json(new List<LeaveModel>(), JsonRequestBehavior.AllowGet);

            var leaveListObj = this.leaveService.GetAllLeave().Where(x => schoolIds.Contains(x.SchoolId.Value));
            List<LeaveModel> leaveVMList = new List<LeaveModel>();

            foreach (var leave in leaveListObj)
            {
                leaveVMList.Add(PrepareLeaveModel(leave));
            }
            return Json(leaveVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActiveLeaveList()
        {
            var leaveListObj = this.leaveService.GetAllLeave()/*.Where(x => x.IsActive == true)*/;
            List<LeaveModel> leaveVMList = new List<LeaveModel>();

            foreach (var leave in leaveListObj)
            {
                leaveVMList.Add(PrepareLeaveModel(leave));
            }
            return Json(leaveVMList, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetLeave(string id)
        {
            var leave = this.leaveService.GetLeave(id);
            return Json(leave);
        }

        #endregion
    }

}