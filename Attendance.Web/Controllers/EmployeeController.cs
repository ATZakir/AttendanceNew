using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Attendance.CachingService;
using Attendance.ClientModel;
using Attendance.Model;
using Attendance.Service;
using Attendance.Web.Helpers;

namespace Attendance.Web.Controllers
{
    public class EmployeeController : Controller
    {
        public readonly IEmployeeService employeeService;
        public readonly IEmploymentHistoryService employmentHistoryService;
        public readonly IAdminEmploymentHistoryService adminEmploymentHistoryService;
        public readonly IEmployeeEducationService employeeEducationService;
        public readonly IEmployeeLeaveBalanceService employeeLeaveBalanceService;
        public readonly ISchoolService schoolService;
        public readonly IDutyShiftService dutyShiftService;
        public readonly IDutyShiftMasterService dutyShiftMasterService;
        public readonly ISubModuleItemService subModuleItemService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        public readonly IWorkflowactionSettingService workflowactionSettingService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        protected long timeZoneOffset = UserSession.GetTimeZoneOffset();

        public EmployeeController(IEmployeeService employeeService, ISubModuleItemService subModuleItemService,
            IRoleSubModuleItemService roleSubModuleItemService,
            IWorkflowactionSettingService workflowactionSettingService,
            INotificationSettingService notificationSettingService,
            IEmploymentHistoryService employmentHistoryService,
            IAdminEmploymentHistoryService adminEmploymentHistoryService,
            IEmployeeEducationService employeeEducationService,
            ISchoolService schoolService, IEmployeeLeaveBalanceService employeeLeaveBalanceService)
        {
            this.employeeService = employeeService;
            this.subModuleItemService = subModuleItemService;
            this.workflowactionSettingService = workflowactionSettingService;
            this.employmentHistoryService = employmentHistoryService;
            this.adminEmploymentHistoryService = adminEmploymentHistoryService;
            this.employeeEducationService = employeeEducationService;
            this.schoolService = schoolService;
            this.employeeLeaveBalanceService = employeeLeaveBalanceService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        string dateTimeFormat = WebConfigurationManager.AppSettings["DateTimeFormat"];
        string cacheKey = "permission:employee" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        // GET: /Employee/
        public ActionResult Index()
        {
            const string url = "/Employee/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                             Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set(cacheKey, permission, 240);
                    return View("Employee");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }

            return View("~/Views/Shared/NoPermission.cshtml");
        }

        public ActionResult SchoolEmployee()
        {
            const string url = "/Employee/SchoolEmployee";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                             Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set(cacheKey, permission, 240);
                    return View("SchoolEmployee");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }

            return View("~/Views/Shared/NoPermission.cshtml");
        }

        // GET: /Employee/
        public ActionResult Operation()
        {
            ViewBag.Operation = 1;
            const string url = "/Employee/Operation";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                             Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set(cacheKey, permission, 240);
                    return View("Employee");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }

            return View("~/Views/Shared/NoPermission.cshtml");
        }



        public JsonResult GetEmployeeLeaveBalanceForLeave( int id)
        {
            var empLeaveBalance =
                employeeLeaveBalanceService.GetEmployeeLeaveBalanceByEmpIdAndYear(id, DateTime.Now.Year);

            var empLBMList = new List<EmployeeLeaveBalanceModel>();
            if (empLeaveBalance.Any())
            {
                foreach (var elb in empLeaveBalance)
                {
                    var elbm = new EmployeeLeaveBalanceModel();
                    elbm.Year = elb.Year.Value;
                    elbm.EmployeeId = elb.EmployeeId;
                    elbm.LeaveTypeId = (int) elb.LeaveTypeId;
                    elbm.LeaveTypeName = elb.LeaveType.Name;
                    elbm.Achieved = elb.Achieved;
                    elbm.Balance = elb.Balance;
                    elbm.DafaultAllocatedDays = elb.DafaultAllocatedDays;
                    elbm.CarryForward = elb.CarryForward;
                    empLBMList.Add(elbm);
                }
            }

            return Json(empLBMList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateEmployee(Employee employee, string fileName)
        {
            var isSuccess = false;
            var message = string.Empty;
            var isNew = employee.Id == 0 ? true : false;
            const string url = "/Employee/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                             Helpers.UserSession.GetUserFromSession().RoleId);

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(employee))
                    {
                        if (employee.EmploymentHistories != null)
                        {
                            foreach (var emp in employee.EmploymentHistories)
                            {
                                if (emp.DateFrom != null)
                                    emp.DateFrom = emp.DateFrom.Value.ToUniversalTime();
                                if (emp.DateTo != null)
                                    emp.DateTo = emp.DateTo.Value.ToUniversalTime();
                                //var id = emp.DutyShiftId;
                                //    emp.DutyShiftId = this.dutyShiftService.GetDutyShiftByMasterId((int) id);
                                emp.EmployeeId = employee.Id;
                            }
                        }

                        if (employee.AdminEmploymentHistories != null)
                        {
                            foreach (var adEmp in employee.AdminEmploymentHistories)
                            {
                                if (adEmp.DateFrom != null)
                                    adEmp.DateFrom = adEmp.DateFrom.Value.ToUniversalTime();
                                if (adEmp.DateTo != null)
                                    adEmp.DateTo = adEmp.DateTo.Value.ToUniversalTime();
                                adEmp.EmployeeId = employee.Id;
                            }
                        }


                        foreach (var empEdu in employee.EmployeeEducations)
                        {
                            empEdu.EmployeeId = employee.Id;
                        }

                        if (fileName != null)
                        {
                            fileName = fileName.Replace(" ", "_");
                            employee.PhotoPath = employee.Code + '_' + fileName;
                        }


                        foreach (var employeeLeaveBalance in employee.EmployeeLeaveBalances)
                        {
                            employeeLeaveBalance.Year = DateTime.Now.Year;
                        }

                        if (this.employeeService.CreateEmployee(employee))
                        {
                            isSuccess = true;
                            message = "Employee saved successfully!";
                        }
                        else
                        {
                            message = "Employee could not saved!";
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = "Can't save. Same employee name found!";
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
                    var employeeObj = this.employeeService.GetEmployee(employee.Id);
                    if (employeeObj != null)
                    {
                        //delete
                        if (employeeObj.EmploymentHistories.Any())
                        {
                            foreach (var empHistory in employeeObj.EmploymentHistories.ToList())
                            {
                                this.employmentHistoryService.DeleteEmploymentHistory(empHistory.Id);
                            }
                        }

                        //create
                        if (employee.EmploymentHistories.Any())
                        {
                            foreach (var empHistory in employee.EmploymentHistories.ToList())
                            {
                                empHistory.EmployeeId = employee.Id;
                                if (empHistory.DateFrom != null)
                                    empHistory.DateFrom = empHistory.DateFrom.Value.ToUniversalTime();
                                if (empHistory.DateTo != null)
                                    empHistory.DateTo = empHistory.DateTo.Value.ToUniversalTime();

                                this.employmentHistoryService.CreateEmploymentHistory(empHistory);
                            }
                        }
                    }


                    if (employeeObj != null)
                    {
                        //delete
                        if (employeeObj.AdminEmploymentHistories.Any())
                        {
                            foreach (var empHistory in employeeObj.AdminEmploymentHistories.ToList())
                            {
                                this.adminEmploymentHistoryService.DeleteAdminEmploymentHistory(empHistory.Id);
                            }
                        }

                        //create
                        if (employee.AdminEmploymentHistories.Any())
                        {
                            foreach (var empHistory in employee.AdminEmploymentHistories)
                            {
                                empHistory.EmployeeId = employee.Id;
                                if (empHistory.DateFrom != null)
                                    empHistory.DateFrom = empHistory.DateFrom.Value.ToUniversalTime();
                                if (empHistory.DateTo != null)
                                    empHistory.DateTo = empHistory.DateTo.Value.ToUniversalTime();

                                this.adminEmploymentHistoryService.CreateAdminEmploymentHistory(empHistory);
                            }
                        }
                    }

                    if (employeeObj != null)
                    {
                        //delete
                        if (employeeObj.EmployeeEducations.Any())
                        {
                            foreach (var empHistory in employeeObj.EmployeeEducations.ToList())
                            {
                                this.employeeEducationService.DeleteEmployeeEducation((int)empHistory.Id);
                            }
                        }

                        //create
                        if (employee.EmployeeEducations.Any())
                        {
                            foreach (var empHistory in employee.EmployeeEducations)
                            {
                                empHistory.EmployeeId = employee.Id;
                                this.employeeEducationService.CreateEmployeeEducation(empHistory);
                            }
                        }
                    }

                    if (employeeObj != null)
                    {
                        employeeObj.Code = employee.Code;
                        employeeObj.FullName = employee.FullName;
                        employeeObj.PresentAddress = employee.PresentAddress;
                        employeeObj.PermanentAddress = employee.PermanentAddress;
                        employeeObj.FatherName = employee.FatherName;
                        employeeObj.MotherName = employee.MotherName;
                        employeeObj.NationalId = employee.NationalId;
                        employeeObj.PassportNo = employee.PassportNo;

                        //if (employeeObj.PhotoPath == employee.PhotoPath)
                        //{
                        //    employeeObj.PhotoPath = employee.PhotoPath;
                        //}
                        if (fileName != "")
                        {
                            fileName = fileName.Replace(" ", "_");
                            employeeObj.PhotoPath = employee.Code + '_' + fileName;
                        }


                        employeeObj.Email = employee.Email;
                        employeeObj.OfficePhone = employee.OfficePhone;
                        employeeObj.OfficeMobile = employee.OfficeMobile;
                        employeeObj.ResidentPhone = employee.ResidentPhone;
                        employeeObj.ResidentMobile = employee.ResidentMobile;
                        employeeObj.BloodGroup = employee.BloodGroup;

                        if (employeeObj.EmployeeLeaveBalances.Any())
                        {
                            foreach (var employeeObjEmployeeLeaveBalance in employeeObj.EmployeeLeaveBalances)
                            {
                                var newVal = employee.EmployeeLeaveBalances.FirstOrDefault(x => x.Id == employeeObjEmployeeLeaveBalance.Id);
                                if (newVal != null)
                                {
                                    employeeObjEmployeeLeaveBalance.Achieved = newVal.Achieved;
                                    employeeObjEmployeeLeaveBalance.Balance = newVal.Balance;
                                    employeeObjEmployeeLeaveBalance.CarryForward = newVal.CarryForward;
                                }
                            }
                        }

                        foreach (var employeeLeaveBalance in employee.EmployeeLeaveBalances)
                        {
                            employeeLeaveBalance.Year = DateTime.Now.Year;
                        }

                        if (this.employeeService.UpdateEmployee(employeeObj))
                        {
                            isSuccess = true;
                            message = "Employee updated successfully!";
                        }
                        else
                        {
                            message = "Employee could not updated!";
                        }
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

        private bool CheckIsExist(Model.Employee employee)
        {
            return this.employeeService.CheckIsExist(employee);
        }

        [HttpPost]
        public JsonResult DeleteEmployee(Employee employee)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/Employee/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                             Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                var employeeObj = this.employeeService.GetEmployee(employee.Id);
                if (employeeObj != null)
                {
                    if (employeeObj.EmploymentHistories != null)
                    {
                        foreach (var empHistory in employeeObj.EmploymentHistories.ToList())
                        {
                            this.employmentHistoryService.DeleteEmploymentHistory(empHistory.Id);
                        }
                    }
                }

                isSuccess = this.employeeService.DeleteEmployee(employee.Id);
                if (isSuccess)
                {
                    message = "Employee deleted successfully!";
                }
                else
                {
                    message = "Employee can't be deleted!";
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


        //upload file..............

        //public void UpoladFile(string name, string code, HttpPostedFileBase file)
        //{
        //    var length = Request.ContentLength;
        //    var bytes = new byte[length];
        //    Request.InputStream.Read(bytes, 0, length);

        //    string str = file.FileName;
        //    string ext = str.Substring(0, str.LastIndexOf(".") + 1).TrimEnd('.');

        //    if (System.IO.File.Exists(Server.MapPath("~/Files/EmployeeImage/" + code + '_' + ext + Path.GetExtension(file.FileName))))
        //    {
        //        System.IO.File.Delete(Server.MapPath("~/Files/EmployeeImage/" + code + '_' + ext + Path.GetExtension(file.FileName)));
        //    }
        //    var saveToFileLoc = Server.MapPath("~/Files/EmployeeImage/" + code + '_' + ext + Path.GetExtension(file.FileName));

        //    var fileStream = new FileStream(saveToFileLoc, FileMode.Create, FileAccess.ReadWrite);
        //    fileStream.Write(bytes, 0, length);
        //    fileStream.Close();
        //}


        public void UpoladFile(string name, string code, HttpPostedFileBase file)
        {
            if (file != null)
            {
                var mainFolder = Server.MapPath("~/Files");
                if (!Directory.Exists(mainFolder))
                {
                    Directory.CreateDirectory(mainFolder);
                }


                var folder = Server.MapPath("~/Files/EmployeeImage");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }


                string str = file.FileName;
                string ext = str.Substring(0, str.LastIndexOf(".") + 1).TrimEnd('.');
                if (ext != String.Empty)
                {
                    ext = ext.Replace(" ", "_");
                }

                if (System.IO.File.Exists(
                    Server.MapPath("~/Files/EmployeeImage/" + code + '_' + ext + Path.GetExtension(file.FileName))))
                {
                    System.IO.File.Delete(
                        Server.MapPath("~/Files/EmployeeImage/" + code + '_' + ext + Path.GetExtension(file.FileName)));
                }

                var saveToFileLoc =
                    Server.MapPath("~/Files/EmployeeImage/" + code + '_' + ext + Path.GetExtension(file.FileName));
                try
                {
                    file.SaveAs(saveToFileLoc);
                }
                catch (Exception e)
                {
                    Console.WriteLine("File Save Error: " + e);
                }
            }
        }


        public JsonResult GetAllEmployee()
        {
            var employeeListObj = this.employeeService.GetAllEmployee();
            List<EmployeeBaseModel> employeeVMList = new List<EmployeeBaseModel>();
            foreach (var employee in employeeListObj)
            {
                var employeeTemp = new EmployeeBaseModel();
                employeeTemp.Id = employee.Id;
                employeeTemp.Code = employee.Code;
                employeeTemp.FullName = employee.FullName;
                try
                {
                    var employeeHistory = employee.EmploymentHistories.FirstOrDefault(x => x.DateTo == null);
                    if (employeeHistory != null)
                    {
                        employeeTemp.Designation = employeeHistory.Designation.Name;
                        employeeTemp.Department = employeeHistory.Department.Name;
                    }
                }
                catch (Exception e)
                {
                }

                employeeVMList.Add(employeeTemp);
            }

            return Json(employeeVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeList()
        {
            var employeeListObj = this.employeeService.GetAllEmployee();
            List<EmployeeModel> employeeVMList = new List<EmployeeModel>();

            foreach (var employee in employeeListObj)
            {
                EmployeeModel employeeTemp = new EmployeeModel();
                employeeTemp.Id = employee.Id;
                employeeTemp.Code = employee.Code;
                employeeTemp.FullName = employee.FullName;
                employeeTemp.PresentAddress = employee.PresentAddress;
                employeeTemp.PermanentAddress = employee.PermanentAddress;
                employeeTemp.FatherName = employee.FatherName;
                employeeTemp.MotherName = employee.MotherName;
                employeeTemp.Email = employee.Email;
                employeeTemp.NationalId = employee.NationalId;
                employeeTemp.OfficeMobile = employee.OfficeMobile;
                employeeTemp.OfficePhone = employee.OfficePhone;
                employeeTemp.PassportNo = employee.PassportNo;
                employeeTemp.PhotoPath = employee.PhotoPath;
                employeeTemp.ResidentMobile = employee.ResidentMobile;
                employeeTemp.ResidentPhone = employee.ResidentPhone;
                employeeTemp.BloodGroup = employee.BloodGroup;
                string LatestDepartmentName = "";
                string LatestDesignationName = "";
                bool IsSchoolAdmin = false;
                if (employee.EmploymentHistories.Any())
                {
                     IsSchoolAdmin = true;
                    List<EmploymentHistoryModel> employmentHistoryVMList = new List<EmploymentHistoryModel>();
                    foreach (var empHistory in employee.EmploymentHistories.OrderBy(a => a.DateFrom))
                    {
                        EmploymentHistoryModel empHistoryTemp = new EmploymentHistoryModel();
                        empHistoryTemp.Id = empHistory.Id;
                        empHistoryTemp.DesignationId = empHistory.DesignationId;
                        if (empHistory.Designation != null)
                        {
                            empHistoryTemp.DesignationName = empHistory.Designation.Name;
                        }

                        LatestDesignationName = empHistoryTemp.DesignationName;
                        empHistoryTemp.DepartmentId = empHistory.DepartmentId;
                        if (empHistory.Department != null)
                        {
                            empHistoryTemp.DepartmentName = empHistory.Department.Name;
                        }

                        empHistoryTemp.SchoolId = empHistory.SchoolId;
                        if (empHistory.School != null)
                        {
                            empHistoryTemp.SchoolName = empHistory.School.Name;
                        }

                        empHistoryTemp.DutyShiftId = empHistory.DutyShiftId;
                        if (empHistory.DutyShift != null)
                        {
                            empHistoryTemp.DutyShiftName = empHistory.DutyShift.DutyShiftMaster.Name;
                        }

                        LatestDepartmentName = empHistoryTemp.DepartmentName;
                        if (empHistory.DateFrom != null)
                            empHistoryTemp.DateFrom = empHistory.DateFrom.Value.AddMinutes(timeZoneOffset)
                                .ToString("dd MMM yyyy");
                        ;
                        if (empHistory.DateTo != null)
                            empHistoryTemp.DateTo = empHistory.DateTo.Value.AddMinutes(timeZoneOffset)
                                .ToString("dd MMM yyyy");
                        ;
                        empHistoryTemp.EmployeeId = empHistory.EmployeeId;

                        employmentHistoryVMList.Add(empHistoryTemp);
                    }

                    employeeTemp.EmploymentHistories = employmentHistoryVMList;
                }
                //employeeTemp.LatestDepartmentName = LatestDepartmentName;
                //employeeTemp.LatestDesignationName =LatestDesignationName;
                employeeTemp.IsSchoolAdmin = IsSchoolAdmin;

                if (employeeTemp.Id != 1)
                    employeeVMList.Add(employeeTemp);
                else if (UserSession.IsAdmin())
                    employeeVMList.Add(employeeTemp);
            }

            return Json(employeeVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeListBySchool(int id)
        {
            List<EmployeeModel> employeeVMList = new List<EmployeeModel>();
            //var schoolId = this.employmentHistoryService.GetEmploymentHistoryBySchool(id);
            var schoolObj = this.schoolService.GetSchool(id);
            if (schoolObj == null) return Json(employeeVMList, JsonRequestBehavior.AllowGet);
            var employeeListObj = schoolObj.EmploymentHistories.Where(x => x.DateTo == null).Select(x => x.Employee);

            foreach (var employee in employeeListObj)
            {
                EmployeeModel employeeTemp = new EmployeeModel();
                employeeTemp.Id = employee.Id;
                employeeTemp.Code = employee.Code;
                employeeTemp.FullName = employee.FullName;
                employeeTemp.PresentAddress = employee.PresentAddress;
                employeeTemp.PermanentAddress = employee.PermanentAddress;
                employeeTemp.FatherName = employee.FatherName;
                employeeTemp.MotherName = employee.MotherName;
                employeeTemp.Email = employee.Email;
                employeeTemp.NationalId = employee.NationalId;
                employeeTemp.OfficeMobile = employee.OfficeMobile;
                employeeTemp.OfficePhone = employee.OfficePhone;
                employeeTemp.PassportNo = employee.PassportNo;
                employeeTemp.PhotoPath = employee.PhotoPath;
                employeeTemp.ResidentMobile = employee.ResidentMobile;
                employeeTemp.ResidentPhone = employee.ResidentPhone;
                employeeTemp.BloodGroup = employee.BloodGroup;
                string LatestDepartmentName = "";
                string LatestDesignationName = "";
                if (employee.EmploymentHistories != null)
                {
                    List<EmploymentHistoryModel> employmentHistoryVMList = new List<EmploymentHistoryModel>();
                    foreach (var empHistory in employee.EmploymentHistories.OrderBy(a => a.DateFrom))
                    {
                        EmploymentHistoryModel empHistoryTemp = new EmploymentHistoryModel();
                        empHistoryTemp.Id = empHistory.Id;
                        empHistoryTemp.DesignationId = empHistory.DesignationId;
                        if (empHistory.Designation != null)
                        {
                            empHistoryTemp.DesignationName = empHistory.Designation.Name;
                        }

                        LatestDesignationName = empHistoryTemp.DesignationName;
                        empHistoryTemp.DepartmentId = empHistory.DepartmentId;
                        if (empHistory.Department != null)
                        {
                            empHistoryTemp.DepartmentName = empHistory.Department.Name;
                        }

                        empHistoryTemp.SchoolId = empHistory.SchoolId;
                        if (empHistory.School != null)
                        {
                            empHistoryTemp.SchoolName = empHistory.School.Name;
                        }

                        empHistoryTemp.DutyShiftId = empHistory.DutyShiftId;
                        if (empHistory.DutyShift != null)
                        {
                            empHistoryTemp.DutyShiftName = empHistory.DutyShift.DutyShiftMaster.Name;
                        }

                        LatestDepartmentName = empHistoryTemp.DepartmentName;
                        if (empHistory.DateFrom != null)
                            empHistoryTemp.DateFrom = empHistory.DateFrom.Value.AddMinutes(timeZoneOffset)
                                .ToString("dd MMM yyyy");
                        ;
                        if (empHistory.DateTo != null)
                            empHistoryTemp.DateTo = empHistory.DateTo.Value.AddMinutes(timeZoneOffset)
                                .ToString("dd MMM yyyy");
                        ;
                        empHistoryTemp.EmployeeId = empHistory.EmployeeId;

                        employmentHistoryVMList.Add(empHistoryTemp);
                    }

                    employeeTemp.EmploymentHistories = employmentHistoryVMList;
                }

                if (employee.EmployeeEducations != null)
                {
                    List<EmployeeEducationModel> employmentHistoryVMList = new List<EmployeeEducationModel>();
                    foreach (var Educations in employee.EmployeeEducations.OrderBy(a => a.Employee))
                    {
                        EmployeeEducationModel EducationsTemp = new EmployeeEducationModel();
                        EducationsTemp.Id = Educations.Id;
                        EducationsTemp.LevelId = Educations.LevelId;
                        if (Educations.EducationLevel != null)
                        {
                            EducationsTemp.EducationLevel = Educations.EducationLevel.Name;
                        }

                        EducationsTemp.BoardOrUniversityId = Educations.BoardOrUniversityId;
                        if (Educations.BoardOrUniversity != null)
                        {
                            EducationsTemp.BoardorUniversityName = Educations.BoardOrUniversity.Name;
                        }

                        EducationsTemp.InstituteId = Educations.InstituteId;
                        if (Educations.Institute != null)
                        {
                            EducationsTemp.InstituteName = Educations.Institute.Name;
                        }

                        if (Educations.DegreeTitle != null)
                            EducationsTemp.DegreeTitle = Educations.DegreeTitle;
                        if (Educations.DegreeMajor != null)
                            EducationsTemp.DegreeMajor = Educations.DegreeMajor;
                        if (Educations.Result != null)
                            EducationsTemp.Result = Educations.Result;

                        if (Educations.Scale != null)
                            EducationsTemp.Scale = Educations.Scale;

                        EducationsTemp.EmployeeId = Educations.EmployeeId;

                        employmentHistoryVMList.Add(EducationsTemp);
                    }

                    employeeTemp.EmployeeEducations = employmentHistoryVMList;
                }
                //employeeTemp.LatestDepartmentName = LatestDepartmentName;
                //employeeTemp.LatestDesignationName =LatestDesignationName;

                if (employee.EmployeeLeaveBalances.Any())
                {
                    var empLeaveBalance = employee.EmployeeLeaveBalances.Where(x => x.Year == DateTime.Now.Year && x.LeaveTypeId != null);
                    if (empLeaveBalance.Any())
                    {
                        foreach (var employeeLeaveBalance in empLeaveBalance)
                        {
                            var emplb = new EmployeeLeaveBalanceModel();
                            emplb.Id = employeeLeaveBalance.Id;
                            emplb.EmployeeId = employeeLeaveBalance.EmployeeId;
                            if (employeeLeaveBalance.LeaveTypeId != null)
                            {
                                emplb.LeaveTypeId = employeeLeaveBalance.LeaveTypeId.Value;
                            }

                            if (employeeLeaveBalance.LeaveType != null)
                            {
                                emplb.LeaveTypeName = employeeLeaveBalance.LeaveType.Name;
                            }
                            emplb.DafaultAllocatedDays = employeeLeaveBalance.DafaultAllocatedDays;
                            emplb.Achieved = employeeLeaveBalance.Achieved;
                            emplb.CarryForward = employeeLeaveBalance.CarryForward;
                            emplb.Balance = employeeLeaveBalance.Balance;
                            employeeTemp.EmployeeLeaveBalances.Add(emplb);
                        }
                    }
                }

                if (employeeTemp.Id != 1)
                    employeeVMList.Add(employeeTemp);
                else if (UserSession.IsAdmin())
                    employeeVMList.Add(employeeTemp);
            }

            return Json(employeeVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeListBySchool2(int schoolId, int page = 1, int itemsPerPage = 1, string searchText = "")
        {
            List<EmployeeModel> employeeVMList = new List<EmployeeModel>();
            var schoolObj = this.schoolService.GetSchool(schoolId);
            if (schoolObj == null)
                return Json(employeeVMList, JsonRequestBehavior.AllowGet);

            var employeeListObj = schoolObj.EmploymentHistories.Where(x => x.DateTo == null).Select(x => x.Employee);

            if (searchText != null && searchText.Length > 0)
                employeeListObj = employeeListObj.Where(x => x.FullName.ToUpper().Contains(searchText.ToUpper()) || x.Code.ToUpper().Contains(searchText.ToUpper())).ToList();

            int RecordCount = employeeListObj.Count();
            employeeListObj = employeeListObj.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            foreach (var employee in employeeListObj)
            {
                EmployeeModel employeeTemp = new EmployeeModel();
                employeeTemp.Id = employee.Id;
                employeeTemp.Code = employee.Code;
                employeeTemp.FullName = employee.FullName;
                employeeTemp.PresentAddress = employee.PresentAddress;
                employeeTemp.PermanentAddress = employee.PermanentAddress;
                employeeTemp.FatherName = employee.FatherName;
                employeeTemp.MotherName = employee.MotherName;
                employeeTemp.Email = employee.Email;
                employeeTemp.NationalId = employee.NationalId;
                employeeTemp.OfficeMobile = employee.OfficeMobile;
                employeeTemp.OfficePhone = employee.OfficePhone;
                employeeTemp.PassportNo = employee.PassportNo;
                employeeTemp.PhotoPath = employee.PhotoPath;
                employeeTemp.ResidentMobile = employee.ResidentMobile;
                employeeTemp.ResidentPhone = employee.ResidentPhone;
                employeeTemp.BloodGroup = employee.BloodGroup;
                string LatestDepartmentName = "";
                string LatestDesignationName = "";
                if (employee.EmploymentHistories != null)
                {
                    List<EmploymentHistoryModel> employmentHistoryVMList = new List<EmploymentHistoryModel>();
                    foreach (var empHistory in employee.EmploymentHistories.OrderBy(a => a.DateFrom))
                    {
                        EmploymentHistoryModel empHistoryTemp = new EmploymentHistoryModel();
                        empHistoryTemp.Id = empHistory.Id;
                        empHistoryTemp.DesignationId = empHistory.DesignationId;
                        if (empHistory.Designation != null)
                        {
                            empHistoryTemp.DesignationName = empHistory.Designation.Name;
                        }

                        LatestDesignationName = empHistoryTemp.DesignationName;
                        empHistoryTemp.DepartmentId = empHistory.DepartmentId;
                        if (empHistory.Department != null)
                        {
                            empHistoryTemp.DepartmentName = empHistory.Department.Name;
                        }

                        empHistoryTemp.SchoolId = empHistory.SchoolId;
                        if (empHistory.School != null)
                        {
                            empHistoryTemp.SchoolName = empHistory.School.Name;
                        }

                        empHistoryTemp.DutyShiftId = empHistory.DutyShiftId;
                        if (empHistory.DutyShift != null)
                        {
                            empHistoryTemp.DutyShiftName = empHistory.DutyShift.DutyShiftMaster.Name;
                        }

                        LatestDepartmentName = empHistoryTemp.DepartmentName;
                        if (empHistory.DateFrom != null)
                            empHistoryTemp.DateFrom = empHistory.DateFrom.Value.AddMinutes(timeZoneOffset)
                                .ToString("dd MMM yyyy");
                        ;
                        if (empHistory.DateTo != null)
                            empHistoryTemp.DateTo = empHistory.DateTo.Value.AddMinutes(timeZoneOffset)
                                .ToString("dd MMM yyyy");
                        ;
                        empHistoryTemp.EmployeeId = empHistory.EmployeeId;

                        employmentHistoryVMList.Add(empHistoryTemp);
                    }

                    employeeTemp.EmploymentHistories = employmentHistoryVMList;
                }

                if (employee.EmployeeEducations != null)
                {
                    List<EmployeeEducationModel> employmentHistoryVMList = new List<EmployeeEducationModel>();
                    foreach (var Educations in employee.EmployeeEducations.OrderBy(a => a.Employee))
                    {
                        EmployeeEducationModel EducationsTemp = new EmployeeEducationModel();
                        EducationsTemp.Id = Educations.Id;
                        EducationsTemp.LevelId = Educations.LevelId;
                        if (Educations.EducationLevel != null)
                        {
                            EducationsTemp.EducationLevel = Educations.EducationLevel.Name;
                        }

                        EducationsTemp.BoardOrUniversityId = Educations.BoardOrUniversityId;
                        if (Educations.BoardOrUniversity != null)
                        {
                            EducationsTemp.BoardorUniversityName = Educations.BoardOrUniversity.Name;
                        }

                        EducationsTemp.InstituteId = Educations.InstituteId;
                        if (Educations.Institute != null)
                        {
                            EducationsTemp.InstituteName = Educations.Institute.Name;
                        }

                        if (Educations.DegreeTitle != null)
                            EducationsTemp.DegreeTitle = Educations.DegreeTitle;
                        if (Educations.DegreeMajor != null)
                            EducationsTemp.DegreeMajor = Educations.DegreeMajor;
                        if (Educations.Result != null)
                            EducationsTemp.Result = Educations.Result;

                        if (Educations.Scale != null)
                            EducationsTemp.Scale = Educations.Scale;

                        EducationsTemp.EmployeeId = Educations.EmployeeId;

                        employmentHistoryVMList.Add(EducationsTemp);
                    }

                    employeeTemp.EmployeeEducations = employmentHistoryVMList;
                }
                //employeeTemp.LatestDepartmentName = LatestDepartmentName;
                //employeeTemp.LatestDesignationName =LatestDesignationName;

                if (employee.EmployeeLeaveBalances.Any())
                {
                    var empLeaveBalance = employee.EmployeeLeaveBalances.Where(x => x.Year == DateTime.Now.Year && x.LeaveTypeId != null);
                    if (empLeaveBalance.Any())
                    {
                        foreach (var employeeLeaveBalance in empLeaveBalance)
                        {
                            var emplb = new EmployeeLeaveBalanceModel();
                            emplb.Id = employeeLeaveBalance.Id;
                            emplb.EmployeeId = employeeLeaveBalance.EmployeeId;
                            if (employeeLeaveBalance.LeaveTypeId != null)
                            {
                                emplb.LeaveTypeId = employeeLeaveBalance.LeaveTypeId.Value;
                            }

                            if (employeeLeaveBalance.LeaveType != null)
                            {
                                emplb.LeaveTypeName = employeeLeaveBalance.LeaveType.Name;
                            }
                            emplb.DafaultAllocatedDays = employeeLeaveBalance.DafaultAllocatedDays;
                            emplb.Achieved = employeeLeaveBalance.Achieved;
                            emplb.CarryForward = employeeLeaveBalance.CarryForward;
                            emplb.Balance = employeeLeaveBalance.Balance;
                            employeeTemp.EmployeeLeaveBalances.Add(emplb);
                        }
                    }
                }

                if (employeeTemp.Id != 1)
                    employeeVMList.Add(employeeTemp);
                else if (UserSession.IsAdmin())
                    employeeVMList.Add(employeeTemp);
            }

            return Json(new { RecordCount = RecordCount, EmployeeList = employeeVMList }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeSimpleListBySchool(int id)
        {
            List<EmployeeBaseModel> employeeVMList = new List<EmployeeBaseModel>();
            var schoolObj = this.schoolService.GetSchool(id);
            if (schoolObj == null) return Json(employeeVMList, JsonRequestBehavior.AllowGet);

            var employeeListObj = schoolObj.EmploymentHistories.Where(x => x.DateTo == null).Select(x => x.Employee);

            foreach (var employee in employeeListObj)
            {
                var employeeTemp = new EmployeeBaseModel();
                employeeTemp.Id = employee.Id;
                employeeTemp.Code = employee.Code;
                employeeTemp.FullName = employee.FullName;
                try
                {
                    var employeeHistory = employee.EmploymentHistories.FirstOrDefault(x => x.DateTo == null);
                    if (employeeHistory != null)
                    {
                        employeeTemp.Designation = employeeHistory.Designation.Name;
                        employeeTemp.Department = employeeHistory.Department.Name;
                    }
                }
                catch (Exception e)
                {
                }

                employeeVMList.Add(employeeTemp);
            }

            return Json(employeeVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeList2(int divisionId, int districtId, int upazilaId, int page = 1, int itemsPerPage = 1, string searchText = "")
        {
            var employeeListObj = this.employeeService.GetAllEmployee().Where(x => x.AdminEmploymentHistories.Count(y => y.DateTo == null) > 0);

            if (divisionId != 0)
                employeeListObj = employeeListObj.Where(x => x.AdminEmploymentHistories.Count(y => y.DivisionId == divisionId && y.DateTo == null) > 0);

            if (districtId != 0)
                employeeListObj = employeeListObj.Where(x => x.AdminEmploymentHistories.Count(y => y.DistrictlId == districtId && y.DateTo == null) > 0);

            if (upazilaId != 0)
                employeeListObj = employeeListObj.Where(x => x.AdminEmploymentHistories.Count(y => y.UpazillalId == upazilaId && y.DateTo == null) > 0);

            if (searchText != null && searchText.Length > 0)
                employeeListObj = employeeListObj.Where(x => x.FullName.ToUpper().Contains(searchText.ToUpper()) || x.Code.ToUpper().Contains(searchText.ToUpper()));

            int RecordCount = employeeListObj.Count();
            employeeListObj = employeeListObj.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            List<EmployeeModel> employeeVMList = new List<EmployeeModel>();

            foreach (var employee in employeeListObj)
            {
                if (employee.Id != 1)
                    employeeVMList.Add(PrepareEmployeeModel(employee));
                else if (UserSession.IsAdmin())
                    employeeVMList.Add(PrepareEmployeeModel(employee));
            }

            return Json(new { RecordCount = RecordCount, EmployeeList = employeeVMList }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeListOperation()
        {
            var employeeListObj = this.employeeService.GetAllEmployee();
            List<EmployeeModel> employeeVMList = new List<EmployeeModel>();

            foreach (var employee in employeeListObj)
            {
                EmployeeModel employeeTemp = new EmployeeModel();
                employeeTemp.Id = employee.Id;
                employeeTemp.Code = employee.Code;
                employeeTemp.FullName = employee.FullName;
                employeeTemp.PresentAddress = employee.PresentAddress;
                employeeTemp.PermanentAddress = employee.PermanentAddress;
                employeeTemp.FatherName = employee.FatherName;
                employeeTemp.MotherName = employee.MotherName;
                employeeTemp.Email = employee.Email;
                employeeTemp.NationalId = employee.NationalId;
                employeeTemp.OfficeMobile = employee.OfficeMobile;
                employeeTemp.OfficePhone = employee.OfficePhone;
                employeeTemp.PassportNo = employee.PassportNo;
                employeeTemp.PhotoPath = employee.PhotoPath;
                employeeTemp.ResidentMobile = employee.ResidentMobile;
                employeeTemp.ResidentPhone = employee.ResidentPhone;
                employeeTemp.BloodGroup = employee.BloodGroup;


                if (employee.EmploymentHistories != null)
                {
                    List<EmploymentHistoryModel> employmentHistoryVMList = new List<EmploymentHistoryModel>();
                    foreach (var empHistory in employee.EmploymentHistories)
                    {
                        EmploymentHistoryModel empHistoryTemp = new EmploymentHistoryModel();
                        empHistoryTemp.Id = empHistory.Id;
                        empHistoryTemp.DesignationId = empHistory.DesignationId;
                        empHistoryTemp.DepartmentId = empHistory.DepartmentId;
                        if (empHistory.DateFrom != null)
                            empHistoryTemp.DateFrom = empHistory.DateFrom.Value.AddMinutes(timeZoneOffset)
                                .ToString("dd MMM yyyy");
                        ;
                        if (empHistory.DateTo != null)
                            empHistoryTemp.DateTo = empHistory.DateTo.Value.AddMinutes(timeZoneOffset)
                                .ToString("dd MMM yyyy");
                        ;
                        empHistoryTemp.EmployeeId = empHistory.EmployeeId;

                        employmentHistoryVMList.Add(empHistoryTemp);
                    }

                    employeeTemp.EmploymentHistories = employmentHistoryVMList;
                }

                if (employeeTemp.Id != 1)
                    employeeVMList.Add(employeeTemp);
                else if (UserSession.IsAdmin())
                    employeeVMList.Add(employeeTemp);
            }

            return Json(employeeVMList, JsonRequestBehavior.AllowGet);
        }

        private EmployeeModel PrepareEmployeeModel(Employee employee)
        {
            EmployeeModel employeeTemp = new EmployeeModel();
            employeeTemp.Id = employee.Id;
            employeeTemp.Code = employee.Code;
            employeeTemp.FullName = employee.FullName;
            employeeTemp.PresentAddress = employee.PresentAddress;
            employeeTemp.PermanentAddress = employee.PermanentAddress;
            employeeTemp.FatherName = employee.FatherName;
            employeeTemp.MotherName = employee.MotherName;
            employeeTemp.Email = employee.Email;
            employeeTemp.NationalId = employee.NationalId;
            employeeTemp.OfficeMobile = employee.OfficeMobile;
            employeeTemp.OfficePhone = employee.OfficePhone;
            employeeTemp.PassportNo = employee.PassportNo;
            employeeTemp.PhotoPath = employee.PhotoPath;
            employeeTemp.ResidentMobile = employee.ResidentMobile;
            employeeTemp.ResidentPhone = employee.ResidentPhone;
            employeeTemp.BloodGroup = employee.BloodGroup;

            if (employee.AdminEmploymentHistories != null)
            {
                List<AdminEmploymentHistoryModel>
                    AdminEmploymentHistoryVMList = new List<AdminEmploymentHistoryModel>();
                foreach (var adEmpHistory in employee.AdminEmploymentHistories.OrderBy(a => a.DateFrom))
                {
                    AdminEmploymentHistoryModel admEmpHistoryTemp = new AdminEmploymentHistoryModel();
                    admEmpHistoryTemp.Id = adEmpHistory.Id;

                    admEmpHistoryTemp.DepartmentId = adEmpHistory.DepartmentId;
                    if (adEmpHistory.Department != null)
                    {
                        admEmpHistoryTemp.DepartmentName = adEmpHistory.Department.Name;
                    }

                    admEmpHistoryTemp.DesignationId = adEmpHistory.DesignationId;
                    if (adEmpHistory.Designation != null)
                    {
                        admEmpHistoryTemp.DesignationName = adEmpHistory.Designation.Name;
                    }

                    // New Change 
                    admEmpHistoryTemp.DivisionId = adEmpHistory.DivisionId;
                    if (adEmpHistory.Division != null)
                    {
                        admEmpHistoryTemp.DivisionName = adEmpHistory.Division.Name;
                    }

                    admEmpHistoryTemp.DistrictId = adEmpHistory.DistrictlId;
                    if (adEmpHistory.District != null)
                    {
                        admEmpHistoryTemp.DistrictName = adEmpHistory.District.Name;
                    }

                    admEmpHistoryTemp.UpazilaId = adEmpHistory.UpazillalId;
                    if (adEmpHistory.Upazila != null)
                    {
                        admEmpHistoryTemp.UpazilaName = adEmpHistory.Upazila.Name;
                    }

                    if (adEmpHistory.DateFrom != null)
                        admEmpHistoryTemp.DateFrom = adEmpHistory.DateFrom.Value.AddMinutes(timeZoneOffset)
                            .ToString("dd MMM yyyy");
                    ;
                    if (adEmpHistory.DateTo != null)
                        admEmpHistoryTemp.DateTo =
                            adEmpHistory.DateTo.Value.AddMinutes(timeZoneOffset).ToString("dd MMM yyyy");
                    ;
                    admEmpHistoryTemp.EmployeeId = adEmpHistory.EmployeeId;

                    AdminEmploymentHistoryVMList.Add(admEmpHistoryTemp);
                }

                employeeTemp.AdminEmploymentHistories = AdminEmploymentHistoryVMList;
            }

            if (employee.EmployeeEducations != null)
            {
                List<EmployeeEducationModel> employmentHistoryVMList = new List<EmployeeEducationModel>();
                foreach (var Educations in employee.EmployeeEducations.OrderBy(a => a.Employee))
                {
                    EmployeeEducationModel EducationsTemp = new EmployeeEducationModel();
                    EducationsTemp.Id = Educations.Id;
                    EducationsTemp.LevelId = Educations.LevelId;
                    if (Educations.EducationLevel != null)
                    {
                        EducationsTemp.EducationLevel = Educations.EducationLevel.Name;
                    }

                    EducationsTemp.BoardOrUniversityId = Educations.BoardOrUniversityId;
                    if (Educations.BoardOrUniversity != null)
                    {
                        EducationsTemp.BoardorUniversityName = Educations.BoardOrUniversity.Name;
                    }

                    EducationsTemp.InstituteId = Educations.InstituteId;
                    if (Educations.Institute != null)
                    {
                        EducationsTemp.InstituteName = Educations.Institute.Name;
                    }

                    if (Educations.DegreeTitle != null)
                        EducationsTemp.DegreeTitle = Educations.DegreeTitle;
                    if (Educations.DegreeMajor != null)
                        EducationsTemp.DegreeMajor = Educations.DegreeMajor;
                    if (Educations.Result != null)
                        EducationsTemp.Result = Educations.Result;

                    if (Educations.Scale != null)
                        EducationsTemp.Scale = Educations.Scale;

                    EducationsTemp.EmployeeId = Educations.EmployeeId;

                    employmentHistoryVMList.Add(EducationsTemp);
                }

                employeeTemp.EmployeeEducations = employmentHistoryVMList;
            }

            return employeeTemp;
        }

        public JsonResult GetEmployeeDep(int id)
        {
            var depName = "";
            var msg = true;
            var employee = this.employeeService.GetEmployee(id);
            if (employee.EmploymentHistories != null)
            {
                var empdep = employee.EmploymentHistories.OrderByDescending(empl => empl.DateFrom).FirstOrDefault();
                Employee emp = new Employee();
                if (empdep != null)
                {
                    depName = empdep.Department != null ? empdep.Department.Name : "";
                    if (depName == "")
                    {
                        msg = false;
                    }
                }
                else
                {
                    msg = false;
                }
            }

            return Json(new { depName = depName, msg = msg }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployee(int id)
        {
            var employee = this.employeeService.GetEmployee(id);
            var employeeTemp = new EmployeeBaseModel();
            employeeTemp.Id = employee.Id;
            employeeTemp.Code = employee.Code;
            employeeTemp.FullName = employee.FullName;
            employeeTemp.Email = employee.Email;
            employeeTemp.PhotoPath = employee.PhotoPath;
            try
            {
                var employeeHistory = employee.EmploymentHistories.FirstOrDefault(x => x.DateTo == null);
                if (employeeHistory != null)
                {
                    employeeTemp.Designation = employeeHistory.Designation.Name;
                    employeeTemp.Department = employeeHistory.Department.Name;
                    employeeTemp.SchoolId = employeeHistory.SchoolId;
                    employeeTemp.DivisionId = employeeHistory.School.Upazila.District.DivisionId;
                    employeeTemp.DistrictId = employeeHistory.School.Upazila.DistrictId;
                    employeeTemp.UpazilaId = employeeHistory.School.UpazilaId;
                }
            }
            catch (Exception e)
            {
            }

            return Json(employeeTemp, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAllAvaiableUserForASubModuleItemFilterByDeptDesignation(int workflowactionId,
            int subModuleItemId, int? departmentId, int? designationId)
        {
            var allEmployeeList =
                this.employeeService.GetAllEmployeeByDepartmentAndDesignation(departmentId, designationId);

            var allUserSubModuleItemPreferenceList =
                this.workflowactionSettingService.GetWorkflowactionSettingBySubModuleItemId(subModuleItemId,
                    workflowactionId);

            List<EmployeeModel> employeeList = new List<EmployeeModel>();

            foreach (var user in allEmployeeList)
            {
                var itemExist = false;
                foreach (var medicinePref in allUserSubModuleItemPreferenceList)
                {
                    if (user.Id == medicinePref.EmployeeId)
                    {
                        itemExist = true;
                        break;
                    }
                }

                if (itemExist != true)
                {
                    EmployeeModel employeeViewModelTemp = new EmployeeModel();
                    employeeViewModelTemp.Id = user.Id;
                    //employeeViewModelTemp.RoleId = user.RoleId;
                    //if (user.EmployeeId != null)
                    employeeViewModelTemp.FullName = user.FullName;

                    if (!employeeList.Contains(employeeViewModelTemp))
                    {
                        employeeList.Add(employeeViewModelTemp);
                    }
                }
            }

            //..........................For Selected Users.........................................//

            List<EmployeeModel> employeePreferanceVMList = new List<EmployeeModel>();

            foreach (var userPreferance in allUserSubModuleItemPreferenceList)
            {
                EmployeeModel empPreferanceTemp = new EmployeeModel();
                if (userPreferance.EmployeeId != null)
                    empPreferanceTemp.Id = (int)userPreferance.EmployeeId;

                if (userPreferance.EmployeeId != null)
                {
                    empPreferanceTemp.FullName = userPreferance.Employee.FullName;
                    //userPreferanceTemp.RoleId = userPreferance.Employee.RoleId;
                }

                employeePreferanceVMList.Add(empPreferanceTemp);
            }

            return Json(new
            {
                availableEmployees = employeeList,
                selectedEmployees = employeePreferanceVMList
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllAvaiableUserForASubModuleItemFilterByDeptDesignationForNotification(int subModuleItemId,
            int? departmentId, int? designationId)
        {
            var allEmployeeList =
                this.employeeService.GetAllEmployeeByDepartmentAndDesignation(departmentId, designationId);

            List<EmployeeModel> employeeList = new List<EmployeeModel>();

            foreach (var emp in allEmployeeList)
            {
                var itemExist = false;
                if (itemExist != true)
                {
                    EmployeeModel employeeViewModelTemp = new EmployeeModel();
                    employeeViewModelTemp.Id = emp.Id;
                    //employeeViewModelTemp.RoleId = user.RoleId;
                    employeeViewModelTemp.FullName = emp.FullName;

                    if (!employeeList.Contains(employeeViewModelTemp))
                    {
                        employeeList.Add(employeeViewModelTemp);
                    }
                }
            }

            //..........................For Selected Users.........................................//

            List<EmployeeModel> employeePreferanceVMList = new List<EmployeeModel>();

            return Json(new
            {
                availableEmployees = employeeList,
                selectedEmployees = employeePreferanceVMList
            }, JsonRequestBehavior.AllowGet);
        }
    }
}