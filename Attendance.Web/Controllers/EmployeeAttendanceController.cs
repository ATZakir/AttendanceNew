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
    public class EmployeeAttendanceController : Controller
    {
        public readonly IDistrictService districtService;
        public readonly ISchoolService schoolService;
        public readonly IEmployeeService employeeService;
        public readonly IEmploymentHistoryService employmentHistoryService;
        public readonly IEmployeeAttendanceService employeeAttendanceService;
        public readonly IEmployeeAttendanceSummaryService employeeAttendanceSummaryService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        protected long timeZoneOffset = UserSession.GetTimeZoneOffset();

        string cacheKey = "permission:employeeAttendance" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        public EmployeeAttendanceController(IEmployeeService employeeService, IEmploymentHistoryService employmentHistoryService, IEmployeeAttendanceService employeeAttendanceService, ISchoolService schoolService, IDistrictService districtService, IRoleSubModuleItemService roleSubModuleItemService, IEmployeeAttendanceSummaryService employeeAttendanceSummaryService)
        {
            this.employeeService = employeeService;
            this.employmentHistoryService = employmentHistoryService;
            this.employeeAttendanceService = employeeAttendanceService;
            this.schoolService = schoolService;
            this.districtService = districtService;
            this.roleSubModuleItemService = roleSubModuleItemService;
            this.employeeAttendanceSummaryService = employeeAttendanceSummaryService;
        }

        // GET: /EmployeeAttendance/
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
                        return View("EmployeeAttendance");
                    }
                    else
                        return View("EmployeeAttendance2");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            return View("~/Views/Shared/NoPermission.cshtml");
        }

        [HttpPost]
        public JsonResult CreateEmployeeAttendance(EmployeeAttendance employeeAttendance)
        {
            const string url = "/EmployeeAttendance/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey);
            if (permission == null)
                permission = roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            var isSuccess = false;
            var message = string.Empty;
            var isNew = employeeAttendance.Id == Guid.Empty ? true : false;

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(employeeAttendance))
                    {
                        employeeAttendance.Id = Guid.NewGuid();
                        employeeAttendance.Date = employeeAttendance.Date.Date;

                        if (this.employeeAttendanceService.CreateEmployeeAttendance(employeeAttendance))
                        {   
                            if (CreateAttendanceSummary(employeeAttendance))
                            {
                                isSuccess = true;
                                message = Resources.ResourceEmployeeAttendance.MsgEmployeeAttendanceSaveSuccessful;
                            }
                        }
                        else
                        {
                            message = Resources.ResourceEmployeeAttendance.MsgEmployeeAttendanceSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceEmployeeAttendance.MsgDuplicateEmployeeAttendance;
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
                    if (this.employeeAttendanceService.UpdateEmployeeAttendance(employeeAttendance))
                    {
                        if (CreateAttendanceSummary(employeeAttendance))
                        {
                            isSuccess = true;
                            message = Resources.ResourceEmployeeAttendance.MsgEmployeeAttendanceUpdateSuccessful;
                        }
                        
                    }
                    else
                    {
                        message = Resources.ResourceEmployeeAttendance.MsgEmployeeAttendanceUpdateFailed;
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

        private bool CreateAttendanceSummary(EmployeeAttendance employeeAttendance)
        {
            // for IN
            if (employeeAttendance.Type == 1)
            {
                int empId = employeeAttendance.EmployeeId;
                string flag = "H";
                if (employeeAttendance.RemarkId == 1)
                {
                    flag = "P";
                }
                else if (employeeAttendance.RemarkId == 2)
                {
                    flag = "A";
                }
                else if (employeeAttendance.RemarkId == 3)
                {
                    flag = "LP";
                }
                else if (employeeAttendance.RemarkId == 4)
                {
                    flag = "LA";
                }
                else if (employeeAttendance.RemarkId == 5)
                {
                    flag = "D";
                }
                else if (employeeAttendance.RemarkId == 6)
                {
                    flag = "HD";
                }
                else if (employeeAttendance.RemarkId == 7)
                {
                    flag = "EO";
                }
                else if (employeeAttendance.RemarkId == 8)
                {
                    flag = "ELP";
                }
                return employeeAttendanceSummaryService.InsertEmployeeAttendanceSummary(empId, employeeAttendance.Date, employeeAttendance.Date,  flag);
            }

            return true;

        }

        private bool CheckIsExist(EmployeeAttendance employeeAttendance)
        {
            return this.employeeAttendanceService.CheckIsExist(employeeAttendance);
        }

        [HttpPost]
        public JsonResult DeleteEmployeeAttendance(EmployeeAttendance employeeAttendance)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/EmployeeAttendance/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                isSuccess = this.employeeAttendanceService.DeleteEmployeeAttendance(employeeAttendance.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceEmployeeAttendance.MsgEmployeeAttendanceDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceEmployeeAttendance.MsgEmployeeAttendanceDeleteFailed;
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

        public JsonResult GetEmployeeAttendanceList()
        {
            var employeeAttendanceListObj = this.employeeAttendanceService.GetAllEmployeeAttendance();
            List<EmployeeAttendanceModel> employeeAttendanceVMList = new List<EmployeeAttendanceModel>();

            foreach (var employeeAttendance in employeeAttendanceListObj)
            {
                employeeAttendanceVMList.Add(PrepareEmployeeAttendanceModel(employeeAttendance));
            }
            return Json(employeeAttendanceVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeAttendanceList2()
        {
            var schoolIds = UserSession.GetUserSchoolAccess();
            if (schoolIds == null || schoolIds.Count == 0) Json(new List<EmployeeAttendanceModel>(), JsonRequestBehavior.AllowGet);

            var employeeAttendanceListObj = this.employeeAttendanceService.GetAllEmployeeAttendance().Where(x => schoolIds.Contains(x.SchoolId.Value));
            List<EmployeeAttendanceModel> employeeAttendanceVMList = new List<EmployeeAttendanceModel>();

            foreach (var employeeAttendance in employeeAttendanceListObj)
            {
                employeeAttendanceVMList.Add(PrepareEmployeeAttendanceModel(employeeAttendance));
            }
            return Json(employeeAttendanceVMList, JsonRequestBehavior.AllowGet);
        }

        private EmployeeAttendanceModel PrepareEmployeeAttendanceModel(EmployeeAttendance employeeAttendance)
        {
            EmployeeAttendanceModel employeeAttendanceTemp = new EmployeeAttendanceModel();
            employeeAttendanceTemp.Id = employeeAttendance.Id;
            employeeAttendanceTemp.Date = employeeAttendance.Date.AddMinutes(timeZoneOffset).ToString("dd MMM yyyy");
            employeeAttendanceTemp.EmployeeId = employeeAttendance.EmployeeId;
            if (employeeAttendanceTemp.EmployeeId > 0)
            {
                employeeAttendanceTemp.EmployeeName = employeeAttendance.Employee.FullName;
            }
            employeeAttendanceTemp.Note = employeeAttendance.Note;
            employeeAttendanceTemp.RemarkId = employeeAttendance.RemarkId;
            if (employeeAttendanceTemp.RemarkId > 0)
            {
                employeeAttendanceTemp.AttendanceRemarkName = employeeAttendance.AttendanceRemark.Name;
            }
            employeeAttendanceTemp.Time = employeeAttendance.Time;
            employeeAttendanceTemp.Type = employeeAttendance.Type;
            employeeAttendanceTemp.SchoolId = employeeAttendance.SchoolId;
            if (employeeAttendanceTemp.SchoolId > 0)
            {
                employeeAttendanceTemp.SchoolName = employeeAttendance.School.Name;
                employeeAttendanceTemp.UpazilaId = employeeAttendance.School.UpazilaId;
                if (employeeAttendanceTemp.UpazilaId > 0)
                {
                    //employeeAttendanceTemp.UpazilaName = employeeAttendance.School.Upazila.Name;
                    employeeAttendanceTemp.DistrictId = employeeAttendance.School.Upazila.DistrictId;
                    if (employeeAttendanceTemp.DistrictId > 0)
                    {
                        //employeeAttendanceTemp.DistrictName = employeeAttendance.School.Upazila.District.Name;
                        employeeAttendanceTemp.DivisionId = employeeAttendance.School.Upazila.District.DivisionId;
                        if (employeeAttendanceTemp.DivisionId > 0)
                        {
                            //employeeAttendanceTemp.DivisionName = employeeAttendance.School.Upazila.District.Division.Name;
                            employeeAttendanceTemp.GeoName = employeeAttendance.School.Upazila.District.Division.Name + " - " + employeeAttendance.School.Upazila.District.Name + " - " + employeeAttendance.School.Upazila.Name;
                        }
                    }
                }
            }

            return employeeAttendanceTemp;
        }

        public JsonResult GetActiveEmployeeAttendanceList()
        {
            var employeeAttendanceListObj = this.employeeAttendanceService.GetAllEmployeeAttendance()/*.Where(x => x.IsActive == true)*/;
            List<EmployeeAttendanceModel> employeeAttendanceVMList = new List<EmployeeAttendanceModel>();

            foreach (var employeeAttendance in employeeAttendanceListObj)
            {
                employeeAttendanceVMList.Add(PrepareEmployeeAttendanceModel(employeeAttendance));
            }
            return Json(employeeAttendanceVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeAttendance(Guid id)
        {
            var employeeAttendance = this.employeeAttendanceService.GetEmployeeAttendance(id);
            return Json(employeeAttendance);
        }

        private EmployeeAttendanceSummaryModel PrepareAttendanceSummeryModel(EmployeeAttendanceSummary attendanceSummary)
        {
            EmployeeAttendanceSummaryModel attendanceSummaryModel = new EmployeeAttendanceSummaryModel();

            attendanceSummaryModel.Year = attendanceSummary.Year;
            attendanceSummaryModel.Month = attendanceSummary.Month;
            attendanceSummaryModel.EmployeeId = attendanceSummary.EmployeeId;
            attendanceSummaryModel.EmployeeName = attendanceSummary.Employee != null ? attendanceSummary.Employee.FullName : "";

            attendanceSummaryModel.SchoolId = attendanceSummary.SchoolId;
            attendanceSummaryModel.SchoolName = attendanceSummary.School != null ? attendanceSummary.School.Name : "";

            int tp = 0;
            int ta = 0;
            int tl = 0;

            attendanceSummaryModel.D1=SumupAttendanceSummeryField(attendanceSummary.D1, attendanceSummary.Year, attendanceSummary.Month, 1, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D2=SumupAttendanceSummeryField(attendanceSummary.D2, attendanceSummary.Year, attendanceSummary.Month, 2, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D3=SumupAttendanceSummeryField(attendanceSummary.D3, attendanceSummary.Year, attendanceSummary.Month, 3, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D4=SumupAttendanceSummeryField(attendanceSummary.D4, attendanceSummary.Year, attendanceSummary.Month, 4, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D5=SumupAttendanceSummeryField(attendanceSummary.D5, attendanceSummary.Year, attendanceSummary.Month, 5, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D6=SumupAttendanceSummeryField(attendanceSummary.D6, attendanceSummary.Year, attendanceSummary.Month, 6, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D7=SumupAttendanceSummeryField(attendanceSummary.D7, attendanceSummary.Year, attendanceSummary.Month, 7, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D8=SumupAttendanceSummeryField(attendanceSummary.D8, attendanceSummary.Year, attendanceSummary.Month, 8, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D9=SumupAttendanceSummeryField(attendanceSummary.D9, attendanceSummary.Year, attendanceSummary.Month, 9, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D10=SumupAttendanceSummeryField(attendanceSummary.D10, attendanceSummary.Year, attendanceSummary.Month, 10, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D11=SumupAttendanceSummeryField(attendanceSummary.D11, attendanceSummary.Year, attendanceSummary.Month, 11, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D12=SumupAttendanceSummeryField(attendanceSummary.D12, attendanceSummary.Year, attendanceSummary.Month, 12, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D13=SumupAttendanceSummeryField(attendanceSummary.D13, attendanceSummary.Year, attendanceSummary.Month, 13, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D14=SumupAttendanceSummeryField(attendanceSummary.D14, attendanceSummary.Year, attendanceSummary.Month, 14, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D15=SumupAttendanceSummeryField(attendanceSummary.D15, attendanceSummary.Year, attendanceSummary.Month, 15, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D16=SumupAttendanceSummeryField(attendanceSummary.D16, attendanceSummary.Year, attendanceSummary.Month, 16, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D17=SumupAttendanceSummeryField(attendanceSummary.D17, attendanceSummary.Year, attendanceSummary.Month, 17, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D18=SumupAttendanceSummeryField(attendanceSummary.D18, attendanceSummary.Year, attendanceSummary.Month, 18, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D19=SumupAttendanceSummeryField(attendanceSummary.D19, attendanceSummary.Year, attendanceSummary.Month, 19, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D20=SumupAttendanceSummeryField(attendanceSummary.D20, attendanceSummary.Year, attendanceSummary.Month, 20, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D21=SumupAttendanceSummeryField(attendanceSummary.D21, attendanceSummary.Year, attendanceSummary.Month, 21, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D22=SumupAttendanceSummeryField(attendanceSummary.D22, attendanceSummary.Year, attendanceSummary.Month, 22, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D23=SumupAttendanceSummeryField(attendanceSummary.D23, attendanceSummary.Year, attendanceSummary.Month, 23, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D24=SumupAttendanceSummeryField(attendanceSummary.D24, attendanceSummary.Year, attendanceSummary.Month, 24, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D25=SumupAttendanceSummeryField(attendanceSummary.D25, attendanceSummary.Year, attendanceSummary.Month, 25, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D26=SumupAttendanceSummeryField(attendanceSummary.D26, attendanceSummary.Year, attendanceSummary.Month, 26, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D27=SumupAttendanceSummeryField(attendanceSummary.D27, attendanceSummary.Year, attendanceSummary.Month, 27, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D28=SumupAttendanceSummeryField(attendanceSummary.D28, attendanceSummary.Year, attendanceSummary.Month, 28, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D29=SumupAttendanceSummeryField(attendanceSummary.D29, attendanceSummary.Year, attendanceSummary.Month, 29, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D30=SumupAttendanceSummeryField(attendanceSummary.D30, attendanceSummary.Year, attendanceSummary.Month, 30, ref tp, ref ta, ref tl);
            attendanceSummaryModel.D31=SumupAttendanceSummeryField(attendanceSummary.D31, attendanceSummary.Year, attendanceSummary.Month, 31, ref tp, ref ta, ref tl);

            attendanceSummaryModel.TP = tp;
            attendanceSummaryModel.TA = ta;
            attendanceSummaryModel.TL = tl;
            attendanceSummaryModel.PP = Math.Round(ta != 0 ? (float)100*tp/(tp+ta) : 100.00, 2);

            return attendanceSummaryModel;
        }

        private string SumupAttendanceSummeryField(string ObjDn, int year, int month, int day, ref int tp, ref int ta, ref int tl)
        {
            try {
                DateTime toDate = DateTime.Today.Date;
                DateTime dataDate = new DateTime(year, month, day);
                if (toDate >= dataDate && ObjDn == null)
                    ObjDn = "A";
            }
            catch
            {
                ObjDn = null;
            }

            if (ObjDn == "P" || ObjDn == "LP" || ObjDn == "EO" || ObjDn == "ELP")
                tp += 1;
            else if (ObjDn == "A")
                ta += 1;
            else if (ObjDn == "LA")
                tl += 1;

            return ObjDn;
        }

        public JsonResult AttendanceLog(int schoolId, DateTime? datetime=null)
        {            
            List<EmployeeAttendanceLogModel> employeeAttendanceLogVMList = this.employmentHistoryService.GetAllEmploymentHistory()
                .Where(x => x.DateTo == null && x.SchoolId == schoolId)
                .Select(y => new EmployeeAttendanceLogModel
                {
                    EmployeeName = y.Employee.FullName,
                    InTime = y.Employee.EmployeeAttendances.Where(d => d.Date == DateTime.UtcNow.Date).Min(s => s.Time).ToString(),
                    OutTime = y.Employee.EmployeeAttendances.Where(d => d.Date == DateTime.UtcNow.Date).Max(f => f.Time).ToString()
                }).ToList();

            return Json(new { AttendenceLogList = employeeAttendanceLogVMList }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AttendanceSummary(int schoolId, int year, int month, int day)
        {
            var attendanceListObj = this.employeeAttendanceService.GetAllEmployeeAttendance().Where(x => x.SchoolId == schoolId && x.Date.Date == DateTime.UtcNow.Date);

            var attendanceSummaryListObj = this.employeeAttendanceSummaryService.GetAllEmployeeAttendanceSummary().Where(x => x.SchoolId == schoolId && x.Year == year && x.Month == month);
            SchoolAttendanceSummaryModel schoolAttendanceSummary = new SchoolAttendanceSummaryModel();
            List<EmployeeAttendanceSummaryModel> attendanceSummaryVMList = new List<EmployeeAttendanceSummaryModel>();

            int TotalEmployee = attendanceSummaryListObj.Count();
            int PresentEmployee = 0;
            int AbsentEmployee = 0;
            int LeaveEmployee = 0;
            int OnDutyEmployee = 0;
            int InTrainingEmployee = 0;

            foreach (var attendanceSummaryObj in attendanceSummaryListObj)
            {
                PresentEmployee += CountDateAttendance(attendanceSummaryObj, day, "P");
                PresentEmployee += CountDateAttendance(attendanceSummaryObj, day, "LP");
                AbsentEmployee += CountDateAttendance(attendanceSummaryObj, day, "A");
                AbsentEmployee += CountDateAttendance(attendanceSummaryObj, day, null);
                LeaveEmployee += CountDateAttendance(attendanceSummaryObj, day, "LA");
                OnDutyEmployee += CountDateAttendance(attendanceSummaryObj, day, "D");
                InTrainingEmployee += CountDateAttendance(attendanceSummaryObj, day, "T");

                attendanceSummaryVMList.Add(PrepareAttendanceSummeryModel(attendanceSummaryObj));
            }

            return Json(new SchoolAttendanceSummaryModel { TotalEmployee = TotalEmployee, PresentEmployee = PresentEmployee, AbsentEmployee = AbsentEmployee, LeaveEmployee = LeaveEmployee, OnDutyEmployee = OnDutyEmployee, InTrainingEmployee = InTrainingEmployee, AttendenceList = attendanceSummaryVMList }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SchoolAttendanceSummary(int divisionId, int districtId, int upazilaId)
        {
            DateTime toDay = DateTime.Today;

            var schoolListObj = this.schoolService.GetAllSchool().Where(x => x.Id > 0);

            if (upazilaId != 0)
                schoolListObj = schoolListObj.Where(x => x.UpazilaId == upazilaId);
            else if (districtId != 0)
                schoolListObj = schoolListObj.Where(x => x.Upazila.DistrictId == districtId);
            else if (divisionId != 0)
                schoolListObj = schoolListObj.Where(x => x.Upazila.District.DivisionId == divisionId);

            List<SchoolAttendanceSummaryModel> schoolVMList = new List<SchoolAttendanceSummaryModel>();

            int TotalEmployee = 0;
            int MaleEmployee = 0;
            int FemaleEmployee = 0;
            int PresentEmployee = 0;
            int LatePresentEmployee = 0;
            int AbsentEmployee = 0;
            int EarlyOutEmployee = 0;
            int EarlyOutLatePresentEmployee = 0;
            int LeaveEmployee = 0;
            int OnDutyEmployee = 0;
            int InTrainingEmployee = 0;

            foreach (var school in schoolListObj)
            {
                var attendanceSummaryListObj = this.employeeAttendanceSummaryService.GetAllEmployeeAttendanceSummary().Where(x => x.SchoolId == school.Id && x.Year == toDay.Year && x.Month == toDay.Month);
                SchoolAttendanceSummaryModel schoolAttendanceSummary = new SchoolAttendanceSummaryModel();

                schoolAttendanceSummary.SchoolId = school.Id;
                schoolAttendanceSummary.Code = school.Code;
                schoolAttendanceSummary.Name = school.Name;
                schoolAttendanceSummary.TotalEmployee = school.EmploymentHistories.Where(x => x.DateTo == null).Count();
                schoolAttendanceSummary.MaleEmployee = school.EmploymentHistories.Where(x => x.DateTo == null && x.Employee.Gender == "Male").Count();
                schoolAttendanceSummary.FemaleEmployee = school.EmploymentHistories.Where(x => x.DateTo == null && x.Employee.Gender == "Female").Count();
                schoolAttendanceSummary.PresentEmployee = 0;
                schoolAttendanceSummary.LatePresentEmployee = 0;
                schoolAttendanceSummary.AbsentEmployee = 0;
                schoolAttendanceSummary.EarlyOutEmployee = 0;
                schoolAttendanceSummary.EarlyOutLatePresentEmployee = 0;
                schoolAttendanceSummary.LeaveEmployee = 0;
                schoolAttendanceSummary.OnDutyEmployee = 0;
                schoolAttendanceSummary.InTrainingEmployee = 0;

                foreach (var attendanceSummaryObj in attendanceSummaryListObj)
                {
                    schoolAttendanceSummary.PresentEmployee += CountDateAttendance(attendanceSummaryObj, toDay.Day, "P");
                    schoolAttendanceSummary.LatePresentEmployee += CountDateAttendance(attendanceSummaryObj, toDay.Day, "LP");
                    schoolAttendanceSummary.AbsentEmployee += CountDateAttendance(attendanceSummaryObj, toDay.Day, "A");
                    schoolAttendanceSummary.AbsentEmployee += CountDateAttendance(attendanceSummaryObj, toDay.Day, null);
                    schoolAttendanceSummary.EarlyOutEmployee += CountDateAttendance(attendanceSummaryObj, toDay.Day, "EO");
                    schoolAttendanceSummary.EarlyOutLatePresentEmployee += CountDateAttendance(attendanceSummaryObj, toDay.Day, "ELP");
                    schoolAttendanceSummary.LeaveEmployee += CountDateAttendance(attendanceSummaryObj, toDay.Day, "LA");
                    schoolAttendanceSummary.OnDutyEmployee += CountDateAttendance(attendanceSummaryObj, toDay.Day, "D");
                    schoolAttendanceSummary.OnDutyEmployee += CountDateAttendance(attendanceSummaryObj, toDay.Day, "HD");
                    schoolAttendanceSummary.InTrainingEmployee += CountDateAttendance(attendanceSummaryObj, toDay.Day, "T");
                }

                schoolAttendanceSummary.PresentPercent = schoolAttendanceSummary.TotalEmployee != 0 ? 100 * (schoolAttendanceSummary.PresentEmployee + schoolAttendanceSummary.LatePresentEmployee + schoolAttendanceSummary.EarlyOutEmployee + schoolAttendanceSummary.EarlyOutLatePresentEmployee ) / schoolAttendanceSummary.TotalEmployee : 0.0;
                
                TotalEmployee += schoolAttendanceSummary.TotalEmployee;
                MaleEmployee += schoolAttendanceSummary.MaleEmployee;
                FemaleEmployee += schoolAttendanceSummary.FemaleEmployee;
                PresentEmployee += schoolAttendanceSummary.PresentEmployee;
                LatePresentEmployee += schoolAttendanceSummary.LatePresentEmployee;
                AbsentEmployee += schoolAttendanceSummary.AbsentEmployee;
                EarlyOutEmployee += schoolAttendanceSummary.EarlyOutEmployee;
                EarlyOutLatePresentEmployee += schoolAttendanceSummary.EarlyOutLatePresentEmployee;
                LeaveEmployee += schoolAttendanceSummary.LeaveEmployee;
                OnDutyEmployee += schoolAttendanceSummary.OnDutyEmployee;
                InTrainingEmployee += schoolAttendanceSummary.InTrainingEmployee;
                schoolVMList.Add(schoolAttendanceSummary);
            }

            double PresentPercent = TotalEmployee != 0 ? 100 * (PresentEmployee + LatePresentEmployee + EarlyOutEmployee + EarlyOutLatePresentEmployee) / TotalEmployee : 0.0;

            return Json(new { SchoolList = schoolVMList, TotalEmployee, MaleEmployee, FemaleEmployee, PresentEmployee, LatePresentEmployee, AbsentEmployee, EarlyOutEmployee, EarlyOutLatePresentEmployee, LeaveEmployee, OnDutyEmployee, InTrainingEmployee, PresentPercent }, JsonRequestBehavior.AllowGet);
        }

        private int CountDateAttendance(EmployeeAttendanceSummary attendanceSummary, int today, string flag)
        {
            switch (today)
            {
                case 1:
                    return attendanceSummary.D1 == flag ? 1 : 0;
                    
                case 2:
                    return attendanceSummary.D2 == flag ? 1 : 0;
                    
                case 3:
                    return attendanceSummary.D3 == flag ? 1 : 0;
                    
                case 4:
                    return attendanceSummary.D4 == flag ? 1 : 0;
                    
                case 5:
                    return attendanceSummary.D5 == flag ? 1 : 0;
                    
                case 6:
                    return attendanceSummary.D6 == flag ? 1 : 0;
                    
                case 7:
                    return attendanceSummary.D7 == flag ? 1 : 0;
                    
                case 8:
                    return attendanceSummary.D8 == flag ? 1 : 0;
                    
                case 9:
                    return attendanceSummary.D9 == flag ? 1 : 0;
                    
                case 10:
                    return attendanceSummary.D10 == flag ? 1 : 0;
                    
                case 11:
                    return attendanceSummary.D11 == flag ? 1 : 0;
                    
                case 12:
                    return attendanceSummary.D12 == flag ? 1 : 0;
                    
                case 13:
                    return attendanceSummary.D13 == flag ? 1 : 0;
                    
                case 14:
                    return attendanceSummary.D14 == flag ? 1 : 0;
                    
                case 15:
                    return attendanceSummary.D15 == flag ? 1 : 0;
                    
                case 16:
                    return attendanceSummary.D16 == flag ? 1 : 0;
                    
                case 17:
                    return attendanceSummary.D17 == flag ? 1 : 0;
                    
                case 18:
                    return attendanceSummary.D18 == flag ? 1 : 0;
                    
                case 19:
                    return attendanceSummary.D19 == flag ? 1 : 0;
                    
                case 20:
                    return attendanceSummary.D20 == flag ? 1 : 0;
                    
                case 21:
                    return attendanceSummary.D21 == flag ? 1 : 0;
                    
                case 22:
                    return attendanceSummary.D22 == flag ? 1 : 0;
                    
                case 23:
                    return attendanceSummary.D23 == flag ? 1 : 0;
                    
                case 24:
                    return attendanceSummary.D24 == flag ? 1 : 0;
                    
                case 25:
                    return attendanceSummary.D25 == flag ? 1 : 0;
                    
                case 26:
                    return attendanceSummary.D26 == flag ? 1 : 0;
                    
                case 27:
                    return attendanceSummary.D27 == flag ? 1 : 0;
                    
                case 28:
                    return attendanceSummary.D28 == flag ? 1 : 0;
                    
                case 29:
                    return attendanceSummary.D29 == flag ? 1 : 0;
                    
                case 30:
                    return attendanceSummary.D30 == flag ? 1 : 0;
                    
                case 31:
                    return attendanceSummary.D31 == flag ? 1 : 0;
                    
                default:
                    return 0;
            }
        }
    }
}