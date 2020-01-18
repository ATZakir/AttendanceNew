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
    public class InstituteController : Controller
    {
        public readonly IDistrictService districtService;
        public readonly ISchoolService schoolService;
        public readonly IInstituteService instituteService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        protected long timeZoneOffset = UserSession.GetTimeZoneOffset();

        string cacheKey = "permission:institute" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        // GET: /Institute/
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
                    return View("Institute");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            return View("~/Views/Shared/NoPermission.cshtml");
        }

        public InstituteController(IInstituteService instituteService, IDistrictService districtService, IRoleSubModuleItemService roleSubModuleItemService, ISchoolService schoolService)
        {
            this.instituteService = instituteService;
            this.districtService = districtService;
            this.roleSubModuleItemService = roleSubModuleItemService;
            this.schoolService = schoolService;
        }

        [HttpPost]
        public JsonResult CreateInstitute(Institute institute)
        {
            const string url = "/Institute/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey);
            if (permission == null)
                permission = roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            var isSuccess = false;
            var message = string.Empty;
            var isNew = institute.Id == 0 ? true : false;

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(institute))
                    {
                        if (this.instituteService.CreateInstitute(institute))
                        {
                            isSuccess = true;
                            message = Resources.ResourceInstitute.MsgInstituteSaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceInstitute.MsgInstituteSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceInstitute.MsgDuplicateInstitute;
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
                    if (this.instituteService.UpdateInstitute(institute))
                    {
                        isSuccess = true;
                        message = Resources.ResourceInstitute.MsgInstituteUpdateSuccessful;
                    }
                    else
                    {
                        message = Resources.ResourceInstitute.MsgInstituteUpdateFailed;
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

        private bool CheckIsExist(Institute institute)
        {
            return this.instituteService.CheckIsExist(institute);
        }

        [HttpPost]
        public JsonResult DeleteInstitute(Institute institute)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/Institute/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                isSuccess = this.instituteService.DeleteInstitute(institute.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceInstitute.MsgInstituteDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceInstitute.MsgInstituteDeleteFailed;
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

        public JsonResult GetInstituteList()
        {
            var instituteListObj = this.instituteService.GetAllInstitute();
            List<InstituteModel> instituteVMList = new List<InstituteModel>();

            foreach (var institute in instituteListObj)
            {
                instituteVMList.Add(PrepareInstituteModel(institute));
            }
            return Json(instituteVMList, JsonRequestBehavior.AllowGet);
        }
  

        private InstituteModel PrepareInstituteModel(Institute institute)
        {
            InstituteModel instituteTemp = new InstituteModel();
            instituteTemp.Id = institute.Id;
            instituteTemp.Name = institute.Name;
            instituteTemp.DistrictName = districtService.GetDistrict(Convert.ToInt32(institute.DistrictId)).Name;
            instituteTemp.DistrictId = institute.DistrictId;
            return instituteTemp;
        }

        public JsonResult GetSchoolList()
        {
            var schoolListObj = this.schoolService.GetAllSchool();
            List<SchoolModel> schoolVMList = new List<SchoolModel>();

            foreach (var school in schoolListObj)
            {
                SchoolModel schoolTemp = new SchoolModel();
                schoolTemp.Id = school.Id;
                schoolTemp.Name = school.Name;
                schoolTemp.UpazilaName = school.Address;
                schoolVMList.Add(schoolTemp);
            }
            return Json(schoolVMList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInstitute(int id)
        {
            var institute = this.instituteService.GetInstitute(id);
            return Json(institute);
        }
    }

}