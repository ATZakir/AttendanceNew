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
    public class DutyShiftController : Controller
    {
        public readonly IDistrictService districtService;
        public readonly IDutyShiftService dutyShiftService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();
        protected long timeZoneOffset = UserSession.GetTimeZoneOffset();

        string cacheKey = "permission:dutyShift" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        // GET: /DutyShift/
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
                        return View("DutyShift");
                    }
                    else
                        return View("DutyShift2");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            return View("~/Views/Shared/NoPermission.cshtml");
        }

        public DutyShiftController(IDutyShiftService dutyShiftService, IDistrictService districtService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.dutyShiftService = dutyShiftService;
            this.districtService = districtService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        [HttpPost]
        public JsonResult CreateDutyShift(DutyShiftModel dutyShiftModel)
        {
            const string url = "/DutyShift/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey);
            if (permission == null)
                permission = roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            var isSuccess = false;
            var message = string.Empty;
            var isNew = dutyShiftModel.Id == 0 ? true : false;

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    var dutyShift = new DutyShift();
                    dutyShift.Id = dutyShiftModel.Id;
                    dutyShift.DutyShiftMasterId = dutyShiftModel.DutyShiftMasterId;
                    dutyShift.SchoolId = dutyShiftModel.SchoolId;
                    dutyShift.InTime = DateToTimeSpan(dutyShiftModel.InTime);
                    dutyShift.MaxInTime = DateToTimeSpan(dutyShiftModel.MaxInTime);
                    dutyShift.OutTime = DateToTimeSpan(dutyShiftModel.OutTime);

                    if (!CheckIsExist(dutyShift))
                    {
                        if (this.dutyShiftService.CreateDutyShift(dutyShift))
                        {
                            isSuccess = true;
                            message = Resources.ResourceDutyShift.MsgDutyShiftSaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceDutyShift.MsgDutyShiftSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceDutyShift.MsgDuplicateDutyShift;
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
                    var dutyShift = new DutyShift();
                    dutyShift.Id = dutyShiftModel.Id;
                    dutyShift.DutyShiftMasterId = dutyShiftModel.DutyShiftMasterId;
                    dutyShift.SchoolId = dutyShiftModel.SchoolId;
                    dutyShift.InTime = DateToTimeSpan(dutyShiftModel.InTime);
                    dutyShift.MaxInTime = DateToTimeSpan(dutyShiftModel.MaxInTime);
                    dutyShift.OutTime = DateToTimeSpan(dutyShiftModel.OutTime);

                    if (this.dutyShiftService.UpdateDutyShift(dutyShift))
                    {
                        isSuccess = true;
                        message = Resources.ResourceDutyShift.MsgDutyShiftUpdateSuccessful;
                    }
                    else
                    {
                        message = Resources.ResourceDutyShift.MsgDutyShiftUpdateFailed;
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

        private static TimeSpan DateToTimeSpan(string timeString)
        {
            DateTime inTimeDate = DateTime.ParseExact(timeString, "yyyy-MM-dd HH:mm:ss", null);
            inTimeDate = inTimeDate.ToUniversalTime();
            int hour = ((DateTime) (inTimeDate)).Hour;
            int min = ((DateTime) (inTimeDate)).Minute;
            int sec = 0;
            TimeSpan ts = new TimeSpan(hour, min, sec);
            return ts;
        }
        private string UtcTimeSpanToLocal(TimeSpan timeString)
        {
            //DateTime dtUtc = DateTime.UtcNow.Date.Add(timeString);
            //DateTime dt = dtUtc.ToLocalTime();
            //TimeSpan ts = dt.TimeOfDay;
            var toDayDate = DateTime.UtcNow;
            DateTime s = toDayDate.Date + timeString;
            string date = s.AddMinutes(timeZoneOffset).ToString("yyyy-MM-dd hh:mm tt");
            return date;
        }

        private bool CheckIsExist(DutyShift dutyShift)
        {
            return this.dutyShiftService.CheckIsExist(dutyShift);
        }

        [HttpPost]
        public JsonResult DeleteDutyShift(DutyShift dutyShift)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/DutyShift/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                isSuccess = this.dutyShiftService.DeleteDutyShift(dutyShift.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceDutyShift.MsgDutyShiftDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceDutyShift.MsgDutyShiftDeleteFailed;
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

        public JsonResult GetDutyShiftList()
        {
            var dutyShiftListObj = this.dutyShiftService.GetAllDutyShift();
            List<DutyShiftModel> dutyShiftVMList = new List<DutyShiftModel>();

            foreach (var dutyShift in dutyShiftListObj)
            {
                dutyShiftVMList.Add(PrepareDutyShiftModel(dutyShift));
            }
            return Json(dutyShiftVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDutyShiftList2()
        {
            var schoolIds = UserSession.GetUserSchoolAccess();
            if (schoolIds == null || schoolIds.Count == 0) Json(new List<DutyShiftModel>(), JsonRequestBehavior.AllowGet);

            var dutyShiftListObj = this.dutyShiftService.GetAllDutyShift().Where(x => schoolIds.Contains(x.SchoolId.Value));
            List<DutyShiftModel> dutyShiftVMList = new List<DutyShiftModel>();

            foreach (var dutyShift in dutyShiftListObj)
            {
                dutyShiftVMList.Add(PrepareDutyShiftModel(dutyShift));
            }
            return Json(dutyShiftVMList, JsonRequestBehavior.AllowGet);
        }

        private DutyShiftModel PrepareDutyShiftModel(DutyShift dutyShift)
        {

            DutyShiftModel dutyShiftTemp = new DutyShiftModel();
            dutyShiftTemp.Id = dutyShift.Id;
            if (dutyShift.InTime != null)
                dutyShiftTemp.InTime = UtcTimeSpanToLocal((TimeSpan) dutyShift.InTime);
            if (dutyShift.MaxInTime != null)
                dutyShiftTemp.MaxInTime = UtcTimeSpanToLocal((TimeSpan)dutyShift.MaxInTime);
            if (dutyShift.OutTime != null)
                dutyShiftTemp.OutTime = UtcTimeSpanToLocal((TimeSpan)dutyShift.OutTime);
            dutyShiftTemp.SchoolId = dutyShift.SchoolId;
            if (dutyShiftTemp.SchoolId > 0)
            {
                dutyShiftTemp.SchoolName = dutyShift.School.Name;
                dutyShiftTemp.UpazilaId = dutyShift.School.UpazilaId;
                if (dutyShiftTemp.UpazilaId > 0)
                {
                    //dutyShiftTemp.UpazilaName = dutyShift.School.Upazila.Name;
                    dutyShiftTemp.DistrictId = dutyShift.School.Upazila.DistrictId;
                    if (dutyShiftTemp.DistrictId > 0)
                    {
                        //dutyShiftTemp.DistrictName = dutyShift.School.Upazila.District.Name;
                        dutyShiftTemp.DivisionId = dutyShift.School.Upazila.District.DivisionId;
                        if (dutyShiftTemp.DivisionId > 0)
                        {
                            //dutyShiftTemp.DivisionName = dutyShift.School.Upazila.District.Division.Name;
                            dutyShiftTemp.GeoName = dutyShift.School.Upazila.District.Division.Name + " - " + dutyShift.School.Upazila.District.Name + " - " + dutyShift.School.Upazila.Name;
                        }
                    }
                }
            }
            dutyShiftTemp.DutyShiftMasterId = dutyShift.DutyShiftMasterId;
            if (dutyShiftTemp.DutyShiftMasterId > 0)
            {
                dutyShiftTemp.DutyShiftMasterName = dutyShift.DutyShiftMaster.Name;
            }
            
            return dutyShiftTemp;
        }


        public JsonResult GetActiveDutyShiftList()
        {
            var dutyShiftListObj = this.dutyShiftService.GetAllDutyShift()/*.Where(x => x.IsActive == true)*/;
            List<DutyShiftModel> dutyShiftVMList = new List<DutyShiftModel>();

            foreach (var dutyShift in dutyShiftListObj)
            {
                dutyShiftVMList.Add(PrepareDutyShiftModel(dutyShift));
            }
            return Json(dutyShiftVMList, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult GetDutyShift(int id)
        {
            var dutyShift = this.dutyShiftService.GetDutyShift(id);
            return Json(dutyShift);
        }
    }

}