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
    public class SchoolController : Controller
    {
        public readonly IDistrictService districtService;
        public readonly ISchoolService schoolService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        protected long timeZoneOffset = UserSession.GetTimeZoneOffset();

        string cacheKey = "permission:school" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        // GET: /School/
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
                        return View("School");
                    }
                    else
                        return View("School2");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            return View("~/Views/Shared/NoPermission.cshtml");
        }

        public SchoolController(ISchoolService schoolService, IDistrictService districtService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.schoolService = schoolService;
            this.districtService = districtService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        [HttpPost]
        public JsonResult CreateSchool(School school)
        {
            const string url = "/School/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey);
            if (permission == null)
                permission = roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            var isSuccess = false;
            var message = string.Empty;
            var isNew = school.Id == 0 ? true : false;

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(school))
                    {
                        if (this.schoolService.CreateSchool(school))
                        {
                            isSuccess = true;
                            message = Resources.ResourceSchool.MsgSchoolSaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceSchool.MsgSchoolSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceSchool.MsgDuplicateSchool;
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
                    if (this.schoolService.UpdateSchool(school))
                    {
                        isSuccess = true;
                        message = Resources.ResourceSchool.MsgSchoolUpdateSuccessful;
                    }
                    else
                    {
                        message = Resources.ResourceSchool.MsgSchoolUpdateFailed;
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
        private bool CheckIsExist(School school)
        {
            return this.schoolService.CheckIsExist(school);
        }

        [HttpPost]
        public JsonResult DeleteSchool(School school)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/School/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                isSuccess = this.schoolService.DeleteSchool(school.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceSchool.MsgSchoolDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceSchool.MsgSchoolDeleteFailed;
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

        public JsonResult GetSchoolList()
        {
            var schoolListObj = this.schoolService.GetAllSchool();
            List<SchoolModel> schoolVMList = new List<SchoolModel>();

            foreach (var school in schoolListObj)
            {
                schoolVMList.Add(PrepareSchoolModel(school));
            }
            return Json(schoolVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSchoolList2()
        {
            var schoolIds = UserSession.GetUserSchoolAccess();
            if (schoolIds == null || schoolIds.Count == 0) Json(new List<SchoolModel>(), JsonRequestBehavior.AllowGet);

            var schoolListObj = this.schoolService.GetAllSchool().Where(x => schoolIds.Contains(x.Id) && x.Id>0);
            List<SchoolModel> schoolVMList = new List<SchoolModel>();

            foreach (var school in schoolListObj)
            {
                schoolVMList.Add(PrepareSchoolModel(school));
            }
            return Json(schoolVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllSchoolListByUpazila(int id)
        {
            var schoolListObj = this.schoolService.GetAllSchool().Where(x => x.UpazilaId == id || 
            x.Id == 0);
            List<SchoolModel> schoolVMList = new List<SchoolModel>();

            foreach (var school in schoolListObj)
            {
                schoolVMList.Add(PrepareSchoolModel(school));
            }
            return Json(schoolVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllSchoolListByUpazila2(int id)
        {
            var schoolIds = UserSession.GetUserSchoolAccess();
            if (schoolIds == null || schoolIds.Count == 0) Json(new List<SchoolModel>(), JsonRequestBehavior.AllowGet);

            var schoolListObj = this.schoolService.GetAllSchool().Where(x => x.UpazilaId == id && schoolIds.Contains(x.Id));
            List<SchoolModel> schoolVMList = new List<SchoolModel>();

            foreach (var school in schoolListObj)
            {
                schoolVMList.Add(PrepareSchoolModel(school));
            }
            return Json(schoolVMList, JsonRequestBehavior.AllowGet);
        }

        private SchoolModel PrepareSchoolModel(School school)
        {
            SchoolModel schoolTemp = new SchoolModel();
            schoolTemp.Id = school.Id;
            schoolTemp.Name = school.Name;
            schoolTemp.Address = school.Address;
            schoolTemp.Code = school.Code;
            schoolTemp.EIN = school.EIN;
            schoolTemp.Email = school.Email;
            schoolTemp.Mobile = school.Mobile;
            schoolTemp.Phone = school.Phone;
            schoolTemp.UpazilaId = school.UpazilaId;
            if (schoolTemp.UpazilaId > 0)
            {
                schoolTemp.UpazilaName = school.Upazila.Name;
                schoolTemp.DistrictId = school.Upazila.DistrictId;
                if (schoolTemp.DistrictId > 0)
                {
                    schoolTemp.DistrictName = school.Upazila.District.Name;
                    schoolTemp.DivisionId = school.Upazila.District.DivisionId;
                    if (schoolTemp.DivisionId > 0)
                    {
                        schoolTemp.DivisionName = school.Upazila.District.Division.Name;
                    }
                }                
            }
            if (school.IsActive != null)
                schoolTemp.IsActive = school.IsActive.Value;
            return schoolTemp;
        }

        public JsonResult GetActiveSchoolList()
        {
            var schoolListObj = this.schoolService.GetAllSchool()/*.Where(x => x.IsActive == true)*/;
            List<SchoolModel> schoolVMList = new List<SchoolModel>();

            foreach (var school in schoolListObj)
            {
                schoolVMList.Add(PrepareSchoolModel(school));
            }
            return Json(schoolVMList, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult GetSchool(int id)
        {
            var school = this.schoolService.GetSchool(id);
            return Json(PrepareSchoolModel(school), JsonRequestBehavior.AllowGet);
        }
    }

}