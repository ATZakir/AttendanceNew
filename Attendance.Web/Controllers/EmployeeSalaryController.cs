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
    public class EmployeeSalaryController : Controller
    {
        public readonly IDistrictService districtService;
        public readonly IEmployeeSalaryService employeeSalaryService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        protected long timeZoneOffset = UserSession.GetTimeZoneOffset();

        string cacheKey = "permission:employeeSalary" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        // GET: /EmployeeSalary/
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
                        return View("EmployeeSalary");
                    }
                    else
                        return View("EmployeeSalary2");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            return View("~/Views/Shared/NoPermission.cshtml");
        }

        public EmployeeSalaryController(IEmployeeSalaryService employeeSalaryService, IDistrictService districtService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.employeeSalaryService = employeeSalaryService;
            this.districtService = districtService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        [HttpPost]
        public JsonResult CreateEmployeeSalary(EmployeeSalary employeeSalary)
        {
            const string url = "/EmployeeSalary/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey);
            if (permission == null)
                permission = roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            var isSuccess = false;
            var message = string.Empty;
            var isNew = employeeSalary.Id == Guid.Empty ? true : false;

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(employeeSalary))
                    {
                        employeeSalary.Id = Guid.NewGuid();
                        if (this.employeeSalaryService.CreateEmployeeSalary(employeeSalary))
                        {
                            isSuccess = true;
                            message = Resources.ResourceEmployeeSalary.MsgEmployeeSalarySaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceEmployeeSalary.MsgEmployeeSalarySaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceEmployeeSalary.MsgDuplicateEmployeeSalary;
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
                    if (this.employeeSalaryService.UpdateEmployeeSalary(employeeSalary))
                    {
                        isSuccess = true;
                        message = Resources.ResourceEmployeeSalary.MsgEmployeeSalaryUpdateSuccessful;
                    }
                    else
                    {
                        message = Resources.ResourceEmployeeSalary.MsgEmployeeSalaryUpdateFailed;
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
        private bool CheckIsExist(EmployeeSalary employeeSalary)
        {
            return this.employeeSalaryService.CheckIsExist(employeeSalary);
        }

        [HttpPost]
        public JsonResult DeleteEmployeeSalary(EmployeeSalary employeeSalary)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/EmployeeSalary/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                isSuccess = this.employeeSalaryService.DeleteEmployeeSalary(employeeSalary.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceEmployeeSalary.MsgEmployeeSalaryDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceEmployeeSalary.MsgEmployeeSalaryDeleteFailed;
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

        public JsonResult GetEmployeeSalaryList()
        {
            var employeeSalaryListObj = this.employeeSalaryService.GetAllEmployeeSalary();
            List<EmployeeSalaryModel> employeeSalaryVMList = new List<EmployeeSalaryModel>();

            foreach (var employeeSalary in employeeSalaryListObj)
            {
                employeeSalaryVMList.Add(PrepareEmployeeSalaryModel(employeeSalary));
            }
            return Json(employeeSalaryVMList, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetEmployeeSalaryList2()
        {
            var schoolIds = UserSession.GetUserSchoolAccess();
            if (schoolIds == null || schoolIds.Count == 0) Json(new List<EmployeeSalaryModel>(), JsonRequestBehavior.AllowGet);

            var employeeSalaryListObj = this.employeeSalaryService.GetAllEmployeeSalary().Where(x => schoolIds.Contains(x.SchoolId.Value));
            List<EmployeeSalaryModel> employeeSalaryVMList = new List<EmployeeSalaryModel>();

            foreach (var employeeSalary in employeeSalaryListObj)
            {
                employeeSalaryVMList.Add(PrepareEmployeeSalaryModel(employeeSalary));
            }
            return Json(employeeSalaryVMList, JsonRequestBehavior.AllowGet);
        }

        private EmployeeSalaryModel PrepareEmployeeSalaryModel(EmployeeSalary employeeSalary)
        {
            EmployeeSalaryModel employeeSalaryTemp = new EmployeeSalaryModel();
            employeeSalaryTemp.Id = employeeSalary.Id;
            employeeSalaryTemp.BasicSalary = employeeSalary.BasicSalary;
            employeeSalaryTemp.EmployeeId = employeeSalary.EmployeeId;
            if (employeeSalaryTemp.EmployeeId > 0)
            {
                employeeSalaryTemp.EmployeeName = employeeSalary.Employee.FullName;
            }
            employeeSalaryTemp.GPFNo = employeeSalary.GPFNo;
            employeeSalaryTemp.MBNo = employeeSalary.MBNo;
            employeeSalaryTemp.SalaryAccount = employeeSalary.SalaryAccount;
            employeeSalaryTemp.SalaryScale = employeeSalary.SalaryScale;
            employeeSalaryTemp.SchoolId = employeeSalary.SchoolId;
            if (employeeSalaryTemp.SchoolId > 0)
            {
                employeeSalaryTemp.SchoolName = employeeSalary.School.Name;
                employeeSalaryTemp.UpazilaId = employeeSalary.School.UpazilaId;
                if (employeeSalaryTemp.UpazilaId > 0)
                {
                    //employeeSalaryTemp.UpazilaName = employeeSalary.School.Upazila.Name;
                    employeeSalaryTemp.DistrictId = employeeSalary.School.Upazila.DistrictId;
                    if (employeeSalaryTemp.DistrictId > 0)
                    {
                        //employeeSalaryTemp.DistrictName = employeeSalary.School.Upazila.District.Name;
                        employeeSalaryTemp.DivisionId = employeeSalary.School.Upazila.District.DivisionId;
                        if (employeeSalaryTemp.DivisionId > 0)
                        {
                            //employeeSalaryTemp.DivisionName = employeeSalary.School.Upazila.District.Division.Name;
                            employeeSalaryTemp.GeoName = employeeSalary.School.Upazila.District.Division.Name + " - " + employeeSalary.School.Upazila.District.Name + " - " + employeeSalary.School.Upazila.Name;
                        }
                    }
                }
            }

            return employeeSalaryTemp;
        }

        public JsonResult GetActiveEmployeeSalaryList()
        {
            var employeeSalaryListObj = this.employeeSalaryService.GetAllEmployeeSalary()/*.Where(x => x.IsActive == true)*/;
            List<EmployeeSalaryModel> employeeSalaryVMList = new List<EmployeeSalaryModel>();

            foreach (var employeeSalary in employeeSalaryListObj)
            {
                employeeSalaryVMList.Add(PrepareEmployeeSalaryModel(employeeSalary));
            }
            return Json(employeeSalaryVMList, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult GetEmployeeSalary(Guid id)
        {
            var employeeSalary = this.employeeSalaryService.GetEmployeeSalary(id);
            return Json(employeeSalary);
        }
    }

}