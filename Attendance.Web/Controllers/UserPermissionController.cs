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
    public class UserPermissionController : Controller
    {
        public readonly IDivisionService divisionService;
        public readonly IDistrictService districtService;
        public readonly IUpazilaService upazilaService;
        public readonly ISchoolService schoolService;
        public readonly IUserPermissionService userPermissionService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        protected long timeZoneOffset = UserSession.GetTimeZoneOffset();

        string cacheKey = "permission:userPermission" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        // GET: /UserPermission/
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
                    return View("UserPermission");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            return View("~/Views/Shared/NoPermission.cshtml");
        }

        public UserPermissionController(IUserPermissionService userPermissionService, IDivisionService divisionService, IDistrictService districtService, IUpazilaService upazilaService, ISchoolService schoolService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.userPermissionService = userPermissionService;
            this.divisionService = divisionService;
            this.districtService = districtService;
            this.upazilaService = upazilaService;
            this.schoolService = schoolService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        [HttpPost]
        public JsonResult CreateUserPermission(UserPermission userPermission)
        {
            const string url = "/UserPermission/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey);
            if (permission == null)
                permission = roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            var isSuccess = false;
            var message = string.Empty;
            var isNew = userPermission.Id == Guid.Empty ? true : false;

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(userPermission))
                    {
                        if (this.userPermissionService.CreateUserPermission(userPermission))
                        {
                            isSuccess = true;
                            message = Resources.ResourceUserPermission.MsgUserPermissionSaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceUserPermission.MsgUserPermissionSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceUserPermission.MsgDuplicateUserPermission;
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
                    if (this.userPermissionService.UpdateUserPermission(userPermission))
                    {
                        isSuccess = true;
                        message = Resources.ResourceUserPermission.MsgUserPermissionUpdateSuccessful;
                    }
                    else
                    {
                        message = Resources.ResourceUserPermission.MsgUserPermissionUpdateFailed;
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
        private bool CheckIsExist(UserPermission userPermission)
        {
            return this.userPermissionService.CheckIsExist(userPermission);
        }

        [HttpPost]
        public JsonResult DeleteUserPermission(UserPermission userPermission)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/UserPermission/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                isSuccess = this.userPermissionService.DeleteUserPermission(userPermission.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceUserPermission.MsgUserPermissionDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceUserPermission.MsgUserPermissionDeleteFailed;
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

        public JsonResult GetUserPermissionList()
        {
            var userPermissionListObj = this.userPermissionService.GetAllUserPermission();
            List<UserPermissionModel> userPermissionVMList = new List<UserPermissionModel>();

            foreach (var userPermission in userPermissionListObj)
            {
                userPermissionVMList.Add(PrepareUserPermissionModel(userPermission));
            }
            return Json(userPermissionVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllUserPermissionListByUserId(int id)
        {
            var userPermissionListObj = this.userPermissionService.GetUserPermissionByUserId(id);
            List<UserPermissionModel> userPermissionVMList = new List<UserPermissionModel>();

            foreach (var userPermission in userPermissionListObj)
            {
                userPermissionVMList.Add(PrepareUserPermissionModel(userPermission));
            }
            return Json(userPermissionVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllDivisionByUserPermission()
        {
            List<DivisionModel> divisionVMList = new List<DivisionModel>();

            int userId = UserSession.GetUserIdFromSession();
            var userPermissionObj = this.userPermissionService.GetUserPermissionByUserId(userId);
            var divisionAll = userPermissionObj.Where(x => x.DivisionId == 0);
            IEnumerable<Division> divisionListObj=null;
            if (divisionAll.Count()>0)
            {
                divisionListObj = divisionService.GetAllDivision();                
            }
            else
            {
                var divisionIds = userPermissionObj.Select(x => x.DivisionId);
                divisionListObj = divisionService.GetAllDivision().Where(x => divisionIds.Contains(x.Id));
            }

            foreach (var division in divisionListObj.Where(x => x.IsActive == true && x.Id > 0))
            {
                DivisionModel divisionTemp = new DivisionModel();
                divisionTemp.Id = division.Id;
                divisionTemp.Name = division.Name;
                divisionVMList.Add(divisionTemp);
            }

            return Json(divisionVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllDistrictByUserPermission(int id)
        {
            List<DistrictModel> districtVMList = new List<DistrictModel>();

            int userId = UserSession.GetUserIdFromSession();
            var userPermissionObj = this.userPermissionService.GetUserPermissionByUserId(userId).Where(x=>x.DivisionId==id || x.DivisionId==0);
            var districtAll = userPermissionObj.Where(x => x.DistrictId == 0);
            IEnumerable<District> districtListObj = null;
            if (districtAll.Count() > 0)
            {
                districtListObj = districtService.GetAllDistrict().Where(x => x.DivisionId == id);                
            }
            else
            {
                var districtIds = userPermissionObj.Select(x => x.DistrictId);
                districtListObj = districtService.GetAllDistrict().Where(x => districtIds.Contains(x.Id));
            }

            foreach (var district in districtListObj.Where(x => x.IsActive == true && x.Id > 0))
            {
                DistrictModel districtTemp = new DistrictModel();
                districtTemp.Id = district.Id;
                districtTemp.Name = district.Name;

                districtVMList.Add(districtTemp);
            }

            return Json(districtVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllUpazilaByUserPermission(int id)
        {
            List<UpazilaModel> upazilaVMList = new List<UpazilaModel>();

            int userId = UserSession.GetUserIdFromSession();
            var userPermissionObj = this.userPermissionService.GetUserPermissionByUserId(userId).Where(x => x.DistrictId == id || x.DistrictId==0);
            var upazilaAll = userPermissionObj.Where(x => x.UpazilaId == 0);
            IEnumerable<Upazila> upazilaListObj = null;
            if (upazilaAll.Count() > 0)
            {
                upazilaListObj = upazilaService.GetAllUpazila().Where(x => x.DistrictId == id);
            }
            else
            {
                var upazilaIds = userPermissionObj.Select(x => x.UpazilaId);
                upazilaListObj = upazilaService.GetAllUpazila().Where(x => upazilaIds.Contains(x.Id));
            }

            foreach (var upazila in upazilaListObj.Where(x => x.IsActive == true && x.Id > 0))
            {
                UpazilaModel upazilaTemp = new UpazilaModel();
                upazilaTemp.Id = upazila.Id;
                upazilaTemp.Name = upazila.Name;

                upazilaVMList.Add(upazilaTemp);
            }

            return Json(upazilaVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllSchoolByUserPermission(int id)
        {
            List<SchoolModel> schoolVMList = new List<SchoolModel>();

            int userId = UserSession.GetUserIdFromSession();
            var userPermissionObj = this.userPermissionService.GetUserPermissionByUserId(userId).Where(x => x.UpazilaId == id || x.UpazilaId==0);
            var schoolAll = userPermissionObj.Where(x => x.SchoolId == 0);
            IEnumerable<School> schoolListObj = null;
            if (schoolAll.Count() > 0)
            {
                schoolListObj = schoolService.GetAllSchool().Where(x => x.UpazilaId == id);
            }
            else
            {
                var schoolIds = userPermissionObj.Select(x => x.SchoolId);
                schoolListObj = schoolService.GetAllSchool().Where(x => schoolIds.Contains(x.Id));
            }

            foreach (var school in schoolListObj.Where(x => x.IsActive == true && x.Id > 0))
            {
                SchoolModel schoolTemp = new SchoolModel();
                schoolTemp.Id = school.Id;
                schoolTemp.Name = school.Name;

                schoolVMList.Add(schoolTemp);
            }

            return Json(schoolVMList, JsonRequestBehavior.AllowGet);
        }

        private UserPermissionModel PrepareUserPermissionModel(UserPermission userPermission)
        {
            UserPermissionModel userPermissionTemp = new UserPermissionModel();
            userPermissionTemp.Id = userPermission.Id;
            userPermissionTemp.UpazilaId = userPermission.UpazilaId;
            if (userPermissionTemp.UpazilaId > 0)
            {
                userPermissionTemp.UpazilaName = upazilaService.GetUpazila(userPermissionTemp.UpazilaId).Name;                              
            }
            userPermissionTemp.DistrictId = userPermission.DistrictId;
            if (userPermissionTemp.DistrictId > 0)
            {
                userPermissionTemp.DistrictName = districtService.GetDistrict(userPermissionTemp.DistrictId).Name;                
            }
            userPermissionTemp.DivisionId = userPermission.DivisionId;
            if (userPermissionTemp.DivisionId > 0)
            {
                userPermissionTemp.DivisionName = divisionService.GetDivision(userPermissionTemp.DivisionId).Name;
            }
            userPermissionTemp.UserId = userPermission.UserId;
            userPermissionTemp.SchoolId = userPermission.SchoolId;
            if (userPermissionTemp.SchoolId > 0)
            {
                userPermissionTemp.SchoolName = schoolService.GetSchool(userPermissionTemp.SchoolId).Name;
            }
            return userPermissionTemp;
        }

        
        
        public JsonResult GetUserPermission(Guid id)
        {
            var userPermission = this.userPermissionService.GetUserPermission(id);
            return Json(userPermission);
        }
    }

}