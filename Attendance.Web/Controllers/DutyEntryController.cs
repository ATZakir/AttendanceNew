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
    public class DutyEntryController : Controller
    {
        #region prop

        public readonly IDistrictService districtService;
        public readonly IDutyEntryService dutyEntryService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        public readonly IEmployeeAttendanceSummaryService employeeAttendanceSummaryService;
        public readonly ISchoolService schoolService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        protected long timeZoneOffset = UserSession.GetTimeZoneOffset();

        string cacheKey = "permission:dutyEntry" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;
        #endregion

        #region Ctor
        public DutyEntryController(IDutyEntryService dutyEntryService, IDistrictService districtService, IRoleSubModuleItemService roleSubModuleItemService, IEmployeeAttendanceSummaryService employeeAttendanceSummaryService, ISchoolService schoolService)
        {
            this.dutyEntryService = dutyEntryService;
            this.districtService = districtService;
            this.roleSubModuleItemService = roleSubModuleItemService;
            this.employeeAttendanceSummaryService = employeeAttendanceSummaryService;
            this.schoolService = schoolService;
        }

        #endregion

        #region Utility
        private bool CreateAttendanceSummary(DutyEntry dutyEntry)
        {
            int empId = dutyEntry.EmployeeId;
            string flag = "D";
            return employeeAttendanceSummaryService.InsertEmployeeAttendanceSummary(empId, dutyEntry.StartDate, dutyEntry.EndDate, flag);
        }

        private bool CheckIsExist(DutyEntry dutyEntry)
        {
            return this.dutyEntryService.CheckIsExist(dutyEntry);
        }

        private DutyEntryModel PrepareDutyEntryModel(DutyEntry dutyEntry)
        {
            DutyEntryModel dutyEntryTemp = new DutyEntryModel();
            dutyEntryTemp.Id = dutyEntry.Id;
            dutyEntryTemp.Date = dutyEntry.Date;
            dutyEntryTemp.EndDate = dutyEntry.EndDate.Value.AddMinutes(timeZoneOffset).ToString("dd MMM yyyy");
            dutyEntryTemp.InTime = dutyEntry.InTime;
            dutyEntryTemp.Location = dutyEntry.Location;
            dutyEntryTemp.OutTime = dutyEntry.OutTime;
            dutyEntryTemp.Remarks = dutyEntry.Remarks;
            dutyEntryTemp.RemovedBy = dutyEntry.RemovedBy;
            dutyEntryTemp.RemovedOn = dutyEntry.RemovedOn;
            dutyEntryTemp.StartDate = dutyEntry.StartDate.Value.AddMinutes(timeZoneOffset).ToString("dd MMM yyyy");
            dutyEntryTemp.EmployeeId = dutyEntry.EmployeeId;
            if (dutyEntryTemp.EmployeeId > 0)
            {
                dutyEntryTemp.EmployeeName = dutyEntry.Employee.FullName;
            }
            dutyEntryTemp.ReasonId = dutyEntry.ReasonId;
            if (dutyEntryTemp.ReasonId > 0)
            {
                dutyEntryTemp.DutyReasonName = dutyEntry.DutyReason.Name;
            }
            dutyEntryTemp.SchoolId = dutyEntry.SchoolId;
            if (dutyEntryTemp.SchoolId > 0)
            {
                dutyEntryTemp.SchoolName = dutyEntry.School.Name;
                dutyEntryTemp.UpazilaId = dutyEntry.School.UpazilaId;
                if (dutyEntryTemp.UpazilaId > 0)
                {
                    //dutyEntryTemp.UpazilaName = dutyEntry.School.Upazila.Name;
                    dutyEntryTemp.DistrictId = dutyEntry.School.Upazila.DistrictId;
                    if (dutyEntryTemp.DistrictId > 0)
                    {
                        //dutyEntryTemp.DistrictName = dutyEntry.School.Upazila.District.Name;
                        dutyEntryTemp.DivisionId = dutyEntry.School.Upazila.District.DivisionId;
                        if (dutyEntryTemp.DivisionId > 0)
                        {
                            //dutyEntryTemp.DivisionName = dutyEntry.School.Upazila.District.Division.Name;
                            dutyEntryTemp.GeoName = dutyEntry.School.Upazila.District.Division.Name + " - " + dutyEntry.School.Upazila.District.Name + " - " + dutyEntry.School.Upazila.Name;
                        }
                    }
                }
            }
            return dutyEntryTemp;
        }

        private string UtcTimeSpanToLocal(TimeSpan timeString)
        {
            //DateTime dtUtc = DateTime.UtcNow.Date.Add(timeString);
            //DateTime dt = dtUtc.ToLocalTime();
            //TimeSpan ts = dt.TimeOfDay;
            var toDayDate = DateTime.UtcNow;
            DateTime s = toDayDate.Date + timeString;
            string date = s.AddMinutes(timeZoneOffset).ToString("hh:mm tt");
            return date;
        }

        private DutyEntryModel PrepareDutyEntryModelForSchool(DutyEntry dutyEntry)
        {
            DutyEntryModel dutyEntryTemp = new DutyEntryModel();
            dutyEntryTemp.Id = dutyEntry.Id;
            dutyEntryTemp.Date = dutyEntry.Date;
            dutyEntryTemp.EmployeeId = dutyEntry.EmployeeId;
            dutyEntryTemp.InTime = dutyEntry.InTime;
            if (dutyEntry.InTime != null) dutyEntryTemp.InTimeString = UtcTimeSpanToLocal((TimeSpan) dutyEntry.InTime);

            dutyEntryTemp.OutTime = dutyEntry.OutTime;
            if (dutyEntry.OutTime != null)
                dutyEntryTemp.OutTimeString = UtcTimeSpanToLocal((TimeSpan) dutyEntry.OutTime);

            if (dutyEntry.StartDate != null)
                dutyEntryTemp.StartDate = dutyEntry.StartDate.Value.AddMinutes(timeZoneOffset).ToString("dd/MM/yyyy");
            if (dutyEntry.EndDate != null)
                dutyEntryTemp.EndDate = dutyEntry.EndDate.Value.AddMinutes(timeZoneOffset).ToString("dd/MM/yyyy");

            dutyEntryTemp.ReasonId = dutyEntry.ReasonId;
            dutyEntryTemp.Location = dutyEntry.Location;
            dutyEntryTemp.Remarks = dutyEntry.Remarks;
            dutyEntryTemp.RemovedBy = dutyEntry.RemovedBy;
            dutyEntryTemp.RemovedOn = dutyEntry.RemovedOn;
            
            
            if (dutyEntryTemp.EmployeeId > 0)
            {
                dutyEntryTemp.EmployeeName = dutyEntry.Employee.FullName;
            }
            dutyEntryTemp.ReasonId = dutyEntry.ReasonId;
            if (dutyEntryTemp.ReasonId > 0)
            {
                dutyEntryTemp.DutyReasonName = dutyEntry.DutyReason.Name;
            }
            dutyEntryTemp.SchoolId = dutyEntry.SchoolId;
            if (dutyEntryTemp.SchoolId > 0)
            {
                dutyEntryTemp.SchoolName = dutyEntry.School.Name;
                dutyEntryTemp.UpazilaId = dutyEntry.School.UpazilaId;
                if (dutyEntryTemp.UpazilaId > 0)
                {
                    //dutyEntryTemp.UpazilaName = dutyEntry.School.Upazila.Name;
                    dutyEntryTemp.DistrictId = dutyEntry.School.Upazila.DistrictId;
                    if (dutyEntryTemp.DistrictId > 0)
                    {
                        //dutyEntryTemp.DistrictName = dutyEntry.School.Upazila.District.Name;
                        dutyEntryTemp.DivisionId = dutyEntry.School.Upazila.District.DivisionId;
                        if (dutyEntryTemp.DivisionId > 0)
                        {
                            //dutyEntryTemp.DivisionName = dutyEntry.School.Upazila.District.Division.Name;
                            dutyEntryTemp.GeoName = dutyEntry.School.Upazila.District.Division.Name + " - " + dutyEntry.School.Upazila.District.Name + " - " + dutyEntry.School.Upazila.Name;
                        }
                    }
                }
            }
            return dutyEntryTemp;
        }

        private DutyEntryModel PrepareDutyEntryModelForSchool2(DutyEntry dutyEntry)
        {
            DutyEntryModel dutyEntryTemp = new DutyEntryModel();
            dutyEntryTemp.Id = dutyEntry.Id;
            dutyEntryTemp.Date = dutyEntry.Date;
            dutyEntryTemp.EmployeeId = dutyEntry.EmployeeId;
            dutyEntryTemp.InTime = dutyEntry.InTime;
            if (dutyEntry.InTime != null) dutyEntryTemp.InTimeString = UtcTimeSpanToLocal((TimeSpan)dutyEntry.InTime);

            dutyEntryTemp.OutTime = dutyEntry.OutTime;
            if (dutyEntry.OutTime != null)
                dutyEntryTemp.OutTimeString = UtcTimeSpanToLocal((TimeSpan)dutyEntry.OutTime);

            if (dutyEntry.StartDate != null)
                dutyEntryTemp.StartDate = dutyEntry.StartDate.Value.AddMinutes(timeZoneOffset).ToString("dd/MM/yyyy");
            if (dutyEntry.EndDate != null)
                dutyEntryTemp.EndDate = dutyEntry.EndDate.Value.AddMinutes(timeZoneOffset).ToString("dd/MM/yyyy");

            dutyEntryTemp.ReasonId = dutyEntry.ReasonId;
            dutyEntryTemp.Location = dutyEntry.Location;
            dutyEntryTemp.Remarks = dutyEntry.Remarks;
            dutyEntryTemp.RemovedBy = dutyEntry.RemovedBy;
            dutyEntryTemp.RemovedOn = dutyEntry.RemovedOn;


            if (dutyEntryTemp.EmployeeId > 0)
            {
                dutyEntryTemp.EmployeeName = dutyEntry.Employee.FullName;
            }
            dutyEntryTemp.ReasonId = dutyEntry.ReasonId;
            if (dutyEntryTemp.ReasonId > 0)
            {
                dutyEntryTemp.DutyReasonName = dutyEntry.DutyReason.Name;
            }
            dutyEntryTemp.SchoolId = dutyEntry.SchoolId;
            if (dutyEntryTemp.SchoolId > 0)
            {
                dutyEntryTemp.SchoolName = dutyEntry.School.Name;
                dutyEntryTemp.UpazilaId = dutyEntry.School.UpazilaId;
                if (dutyEntryTemp.UpazilaId > 0)
                {
                    //dutyEntryTemp.UpazilaName = dutyEntry.School.Upazila.Name;
                    dutyEntryTemp.DistrictId = dutyEntry.School.Upazila.DistrictId;
                    if (dutyEntryTemp.DistrictId > 0)
                    {
                        //dutyEntryTemp.DistrictName = dutyEntry.School.Upazila.District.Name;
                        dutyEntryTemp.DivisionId = dutyEntry.School.Upazila.District.DivisionId;
                        if (dutyEntryTemp.DivisionId > 0)
                        {
                            //dutyEntryTemp.DivisionName = dutyEntry.School.Upazila.District.Division.Name;
                            dutyEntryTemp.GeoName = dutyEntry.School.Upazila.District.Division.Name + " - " + dutyEntry.School.Upazila.District.Name + " - " + dutyEntry.School.Upazila.Name;
                        }
                    }
                }
            }
            return dutyEntryTemp;
        }

        #endregion

        #region Action

        // GET: /DutyEntry/
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
                        return View("DutyEntry");
                    }
                    else
                        return View("DutyEntry2");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            return View("~/Views/Shared/NoPermission.cshtml");
        }

        #endregion


        #region Methods
        [HttpPost]
        public JsonResult CreateDutyEntry(DutyEntry dutyEntry)
        {
            const string url = "/DutyEntry/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey);
            if (permission == null)
                permission = roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            var isSuccess = false;
            var message = string.Empty;
            var isNew = dutyEntry.Id == Guid.Empty ? true : false;

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(dutyEntry))
                    {
                        dutyEntry.Date = DateTime.Today;
                        dutyEntry.Id = Guid.NewGuid();
                        if (this.dutyEntryService.CreateDutyEntry(dutyEntry))
                        {
                            CreateAttendanceSummary(dutyEntry);
                            isSuccess = true;
                            message = Resources.ResourceDutyEntry.MsgDutyEntrySaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceDutyEntry.MsgDutyEntrySaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceDutyEntry.MsgDuplicateDutyEntry;
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

                    var dutyEntryObj = dutyEntryService.GetDutyEntry(dutyEntry.Id);
                    if (dutyEntryObj != null)
                    {
                        dutyEntryObj.EmployeeId = dutyEntry.EmployeeId;
                        dutyEntryObj.ReasonId = dutyEntry.ReasonId;
                        dutyEntryObj.StartDate = dutyEntry.StartDate;
                        dutyEntryObj.EndDate = dutyEntry.EndDate;
                        dutyEntryObj.InTime = dutyEntry.InTime;
                        dutyEntryObj.OutTime = dutyEntry.OutTime;
                        dutyEntryObj.Remarks = dutyEntry.Remarks;
                        dutyEntryObj.Location = dutyEntry.Location;
                        dutyEntryObj.ReasonId = dutyEntry.ReasonId;

                        if (this.dutyEntryService.UpdateDutyEntry(dutyEntryObj))
                        {
                            isSuccess = true;
                            message = Resources.ResourceDutyEntry.MsgDutyEntryUpdateSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceDutyEntry.MsgDutyEntryUpdateFailed;
                        }
                    }
                    else
                    {
                        message = Resources.ResourceDutyEntry.MsgDutyEntryUpdateFailed;
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
        public JsonResult DeleteDutyEntry(DutyEntry dutyEntry)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/DutyEntry/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                             Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                isSuccess = this.dutyEntryService.DeleteDutyEntry(dutyEntry.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceDutyEntry.MsgDutyEntryDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceDutyEntry.MsgDutyEntryDeleteFailed;
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


        public JsonResult GetDutyEntryList()
        {
            var dutyEntryListObj = this.dutyEntryService.GetAllDutyEntry();
            List<DutyEntryModel> dutyEntryVMList = new List<DutyEntryModel>();

            foreach (var dutyEntry in dutyEntryListObj)
            {
                dutyEntryVMList.Add(PrepareDutyEntryModel(dutyEntry));
            }
            return Json(dutyEntryVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDutyEntryList2()
        {
            var schoolIds = UserSession.GetUserSchoolAccess();

            if (schoolIds == null || schoolIds.Count == 0)
            {
                schoolIds.Add(1); // for testing only
                Json(new List<DutyEntryModel>(), JsonRequestBehavior.AllowGet);
            }

            //            var dutyEntryListObj = this.dutyEntryService.GetAllDutyEntry().Where(x => schoolIds.Contains(x.SchoolId.Value));
            List<DutyEntryModel> dutyEntryVMList = new List<DutyEntryModel>();

            //            foreach (var dutyEntry in dutyEntryListObj)
            //            {
            //                dutyEntryVMList.Add(PrepareDutyEntryModel(dutyEntry));
            //            }
            return Json(dutyEntryVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActiveDutyEntryList()
        {
            var dutyEntryListObj = this.dutyEntryService.GetAllDutyEntry()/*.Where(x => x.IsActive == true)*/;
            List<DutyEntryModel> dutyEntryVMList = new List<DutyEntryModel>();

            foreach (var dutyEntry in dutyEntryListObj)
            {
                dutyEntryVMList.Add(PrepareDutyEntryModel(dutyEntry));
            }
            return Json(dutyEntryVMList, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult GetDutyEntry(Guid id)
        {
            var dutyEntry = this.dutyEntryService.GetDutyEntry(id);
            var dutyEntryModel = PrepareDutyEntryModelForSchool(dutyEntry);
            return Json(dutyEntryModel, JsonRequestBehavior.AllowGet);
        }

public JsonResult GetDutyEntry2(Guid id)
        {
            var dutyEntry = this.dutyEntryService.GetDutyEntry(id);
            var dutyEntryModel = PrepareDutyEntryModelForSchool(dutyEntry);
            return Json(dutyEntryModel, JsonRequestBehavior.AllowGet);
        }




        public JsonResult GetDutyEntryListBySchoolAndMonth(int schoolId, DateTime filterMonth, int page = 1, int itemsPerPage = 1, string searchText = "")
        {

            var month = filterMonth.Month;
            var year = filterMonth.Year;

            List<DutyEntryListModel> dutyEntryList = new List<DutyEntryListModel>();
          
            var schoolObj = schoolService.GetSchool(schoolId);
            if (schoolObj == null)
                return Json(dutyEntryList, JsonRequestBehavior.AllowGet);

            var allDutyEntryBySchoolAndMonth = dutyEntryService.GetAllDutyEntry().Where(x => x.StartDate != null && (x.SchoolId == schoolId && x.StartDate.Value.Year == year && x.StartDate.Value.Month == month));

            if (!string.IsNullOrEmpty(searchText))
                allDutyEntryBySchoolAndMonth = allDutyEntryBySchoolAndMonth
                    .Where(x => x.Employee.FullName.ToUpper().Contains(searchText.ToUpper()));

            int RecordCount = allDutyEntryBySchoolAndMonth.Count();
            allDutyEntryBySchoolAndMonth = allDutyEntryBySchoolAndMonth.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            foreach (var dutyEntry in allDutyEntryBySchoolAndMonth)
            {
                DutyEntryListModel dem = new DutyEntryListModel();

                dem.EmployeeId = dutyEntry.EmployeeId;
                if (dutyEntry.StartDate != null)
                    dem.Name = dutyEntry.Employee.FullName + " - " + dutyEntry.StartDate.Value.ToString("dd MMM");
                dem.Id = dutyEntry.Id;

                dutyEntryList.Add(dem);
            }

            return Json(new { RecordCount = RecordCount, EmployeeList = dutyEntryList }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetDutyEntryListBySchoolAndMonth2(List<int> schoolIds, DateTime filterMonth, int page = 1, int itemsPerPage = 1, string searchText = "")
        {

            var month = filterMonth.Month;
            var year = filterMonth.Year;

            List<DutyEntryListModel> dutyEntryList = new List<DutyEntryListModel>();

            var allDutyEntryBySchoolAndMonth = dutyEntryService.GetAllDutyEntry().Where(x => x.StartDate != null && (x.StartDate.Value.Year == year && x.StartDate.Value.Month == month));

            if (schoolIds == null || !schoolIds.Any())
            {
                schoolIds = UserSession.GetUserSchoolAccess();
            }

            allDutyEntryBySchoolAndMonth = allDutyEntryBySchoolAndMonth.Where(x => x.SchoolId != null && schoolIds.Contains((int) x.SchoolId));

            if (!string.IsNullOrEmpty(searchText))
                allDutyEntryBySchoolAndMonth = allDutyEntryBySchoolAndMonth
                    .Where(x => x.Employee.FullName.ToUpper().Contains(searchText.ToUpper()));

            int RecordCount = allDutyEntryBySchoolAndMonth.Count();
            allDutyEntryBySchoolAndMonth = allDutyEntryBySchoolAndMonth.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            foreach (var dutyEntry in allDutyEntryBySchoolAndMonth)
            {
                DutyEntryListModel dem = new DutyEntryListModel();

                dem.EmployeeId = dutyEntry.EmployeeId;
                if (dutyEntry.StartDate != null)
                    dem.Name = dutyEntry.Employee.FullName + " - " + dutyEntry.StartDate.Value.ToString("dd MMM");
                dem.Id = dutyEntry.Id;

                dutyEntryList.Add(dem);
            }

            return Json(new { RecordCount = RecordCount, EmployeeList = dutyEntryList }, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }

}