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
    public class TrainingController : Controller
    {
        public readonly IDistrictService districtService;
        public readonly ITrainingService trainingService;
        public readonly IUserPermissionService userPermissionService;
        public readonly ISchoolService schoolService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        public readonly IEmployeeAttendanceSummaryService employeeAttendanceSummary;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        protected long timeZoneOffset = UserSession.GetTimeZoneOffset();

        string cacheKey = "permission:training" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        // GET: /Training/
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
                        return View("Training");
                    }
                    else
                        return View("Training2");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            return View("~/Views/Shared/NoPermission.cshtml");
        }

        public TrainingController(ITrainingService trainingService, IDistrictService districtService, IUserPermissionService userPermissionService, ISchoolService schoolService, IRoleSubModuleItemService roleSubModuleItemService, IEmployeeAttendanceSummaryService employeeAttendanceSummary)
        {
            this.trainingService = trainingService;
            this.districtService = districtService;
            this.userPermissionService = userPermissionService;
            this.schoolService = schoolService;
            this.roleSubModuleItemService = roleSubModuleItemService;
            this.employeeAttendanceSummary = employeeAttendanceSummary;
        }

        private bool CreateAttendanceSummary(Training training)
        {
            int empId = training.EmployeeId;
            string flag = "T";

            return employeeAttendanceSummary.InsertEmployeeAttendanceSummary(empId, training.StartDate, training.EndDate, flag);
        }

        [HttpPost]
        public JsonResult CreateTraining(Training training)
        {
            const string url = "/Training/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey);
            if (permission == null)
                permission = roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            var isSuccess = false;
            var message = string.Empty;
            //var isNew = training.Id == 0 ? true : false;
            var trainingObj = trainingService.GetTraining(training.Id);
            var isNew = trainingObj == null;

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(training))
                    {
                        training.RemovedBy = UserSession.GetUserFromSession().Id;
                        training.RemovedOn = DateTime.Now.ToUniversalTime();

                        if (this.trainingService.CreateTraining(training))
                        {
                            if (CreateAttendanceSummary(training))
                            {
                                isSuccess = true;
                                message = Resources.ResourceTraining.MsgTrainingSaveSuccessful;
                            }
                            
                        }
                        else
                        {
                            message = Resources.ResourceTraining.MsgTrainingSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceTraining.MsgDuplicateTraining;
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
                    trainingObj.Id = training.Id;
                    trainingObj.EmployeeId = training.EmployeeId;
                    trainingObj.TypeId = training.TypeId;
                    trainingObj.Description = training.Description;
                    trainingObj.StartDate = training.StartDate;
                    trainingObj.EndDate = training.EndDate;
                    trainingObj.RemovedBy = training.RemovedBy;
                    trainingObj.RemovedOn = training.RemovedOn;
                    trainingObj.Status = training.Status;

                    if (this.trainingService.UpdateTraining(trainingObj))
                    {
                        if (CreateAttendanceSummary(training))
                        {
                            isSuccess = true;
                            message = Resources.ResourceTraining.MsgTrainingUpdateSuccessful;
                        }
                       
                    }
                    else
                    {
                        message = Resources.ResourceTraining.MsgTrainingUpdateFailed;
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
        private bool CheckIsExist(Training training)
        {
            return this.trainingService.CheckIsExist(training);
        }

        [HttpPost]
        public JsonResult DeleteTraining(Training training)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/Training/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                isSuccess = this.trainingService.DeleteTraining(training.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceTraining.MsgTrainingDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceTraining.MsgTrainingDeleteFailed;
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

        public JsonResult GetTrainingList()
        {
            var trainingListObj = this.trainingService.GetAllTraining();
            List<TrainingModel> trainingVMList = new List<TrainingModel>();

            foreach (var training in trainingListObj)
            {
                trainingVMList.Add(PrepareTrainingModel(training));
            }
            return Json(trainingVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTrainingList2()
        {
            var schoolIds = UserSession.GetUserSchoolAccess();
            if(schoolIds==null || schoolIds.Count==0) Json(new List<TrainingModel>(), JsonRequestBehavior.AllowGet);

            var trainingListObj = this.trainingService.GetAllTraining().Where(x => schoolIds.Contains(x.SchoolId.Value));
            List<TrainingModel> trainingVMList = new List<TrainingModel>();

            foreach (var training in trainingListObj)
            {
                trainingVMList.Add(PrepareTrainingModel(training));
            }
            return Json(trainingVMList, JsonRequestBehavior.AllowGet);
        }

        private TrainingModel PrepareTrainingModel(Training training)
        {
            TrainingModel trainingTemp = new TrainingModel();
            trainingTemp.Id = training.Id;
            trainingTemp.Description = training.Description;
            trainingTemp.EndDate = training.EndDate.AddMinutes(timeZoneOffset).ToString("dd MMM yyyy");
            trainingTemp.RemovedBy = training.RemovedBy;
            if (training.RemovedOn != null)
                trainingTemp.RemovedOn = training.RemovedOn.Value.AddMinutes(timeZoneOffset).ToString("dd MMM yyyy");
            trainingTemp.StartDate = training.StartDate.AddMinutes(timeZoneOffset).ToString("dd MMM yyyy");
            trainingTemp.Status = training.Status;
            trainingTemp.TypeId = training.TypeId;
            if (trainingTemp.TypeId > 0)
            {
                trainingTemp.TypeName = training.TrainingType.Name;
            }
            trainingTemp.EmployeeId = training.EmployeeId;
            if (trainingTemp.EmployeeId > 0)
            {
                trainingTemp.EmployeeName = training.Employee.FullName;
            }
            trainingTemp.SchoolId = training.SchoolId;
            if (trainingTemp.SchoolId > 0)
            {
                trainingTemp.SchoolName = training.School.Name;
                trainingTemp.UpazilaId = training.School.UpazilaId;
                if (trainingTemp.UpazilaId > 0)
                {
                    //trainingTemp.UpazilaName = training.School.Upazila.Name;
                    trainingTemp.DistrictId = training.School.Upazila.DistrictId;
                    if (trainingTemp.DistrictId > 0)
                    {
                        //trainingTemp.DistrictName = training.School.Upazila.District.Name;
                        trainingTemp.DivisionId = training.School.Upazila.District.DivisionId;
                        if (trainingTemp.DivisionId > 0)
                        {
                            //trainingTemp.DivisionName = training.School.Upazila.District.Division.Name;
                            trainingTemp.GeoName = training.School.Upazila.District.Division.Name + " - " + training.School.Upazila.District.Name + " - " + training.School.Upazila.Name;
                        }
                    }
                }
            }
            return trainingTemp;
        }

        public JsonResult GetActiveTrainingList()
        {
            var trainingListObj = this.trainingService.GetAllTraining()/*.Where(x => x.IsActive == true)*/;
            List<TrainingModel> trainingVMList = new List<TrainingModel>();

            foreach (var training in trainingListObj)
            {
                trainingVMList.Add(PrepareTrainingModel(training));
            }
            return Json(trainingVMList, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult GetTraining(int id)
        {
            var training = this.trainingService.GetTraining(id);
            return Json(training);
        }

        public long GetLastId()
        {
            long lastId = this.trainingService.GetLastId();
            return lastId;
        }
    }

}