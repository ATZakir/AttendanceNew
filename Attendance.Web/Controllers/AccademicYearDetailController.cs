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
    public class AccademicYearDetailController : Controller
    {
        public readonly IDistrictService districtService;
        public readonly IAccademicYearDetailService accademicYearDetailService;
        public readonly IUserPermissionService userPermissionService;
        public readonly ISchoolService schoolService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        public readonly IEmployeeAttendanceSummaryService employeeAttendanceSummary;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        protected long timeZoneOffset = UserSession.GetTimeZoneOffset();

        string cacheKey = "permission:accademicYearDetail" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        // GET: /AccademicYearDetail/
        public ActionResult Index()
        {
            const string url = "/AccademicYearDetail/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set(cacheKey, permission, 240);
                    return View("AccademicYearDetail");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }

            return View("~/Views/Shared/NoPermission.cshtml");
        }

        public AccademicYearDetailController(IAccademicYearDetailService accademicYearDetailService, IDistrictService districtService, IUserPermissionService userPermissionService, ISchoolService schoolService, IRoleSubModuleItemService roleSubModuleItemService, IEmployeeAttendanceSummaryService employeeAttendanceSummary)
        {
            this.accademicYearDetailService = accademicYearDetailService;
            this.districtService = districtService;
            this.userPermissionService = userPermissionService;
            this.schoolService = schoolService;
            this.roleSubModuleItemService = roleSubModuleItemService;
            this.employeeAttendanceSummary = employeeAttendanceSummary;
        }

        [HttpPost]
        public JsonResult CreateAccademicYearDetail(AccademicYearDetail accademicYearDetail)
        {
            const string url = "/AccademicYearDetail/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey);
            if (permission == null)
                permission = roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            var isSuccess = false;
            var message = string.Empty;
            var accademicYearDetailObj = accademicYearDetailService.GetAccademicYearDetail(accademicYearDetail.AccademicYear, accademicYearDetail.DayDate.Date);
            var isNew = accademicYearDetailObj == null;

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(accademicYearDetail))
                    {
                        accademicYearDetail.DayDate = accademicYearDetail.DayDate.Date;
                        if (this.accademicYearDetailService.CreateAccademicYearDetail(accademicYearDetail))
                        {
                         isSuccess = true;
                          message = Resources.ResourceAccademicYearDetail.MsgAccademicYearDetailSaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceAccademicYearDetail.MsgAccademicYearDetailSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceAccademicYearDetail.MsgDuplicateAccademicYearDetail;
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
                    accademicYearDetailObj.AccademicYear = accademicYearDetail.AccademicYear;
                    accademicYearDetailObj.DayDate = accademicYearDetail.DayDate.Date;
                    accademicYearDetailObj.DayType = accademicYearDetail.DayType;
                    accademicYearDetailObj.DayDescription = accademicYearDetail.DayDescription;
                    if (this.accademicYearDetailService.UpdateAccademicYearDetail(accademicYearDetailObj))
                    {
                        isSuccess = true;
                        message = Resources.ResourceAccademicYearDetail.MsgAccademicYearDetailUpdateSuccessful;

                    }
                    else
                    {
                        message = Resources.ResourceAccademicYearDetail.MsgAccademicYearDetailUpdateFailed;
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
        private bool CheckIsExist(AccademicYearDetail accademicYearDetail)
        {
            return this.accademicYearDetailService.CheckIsExist(accademicYearDetail);
        }

        [HttpPost]
        public JsonResult DeleteAccademicYearDetail(AccademicYearDetail accademicYearDetail)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/AccademicYearDetail/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                isSuccess = this.accademicYearDetailService.DeleteAccademicYearDetail(accademicYearDetail.AccademicYear, accademicYearDetail.DayDate.Date);
                if (isSuccess)
                {
                    message = Resources.ResourceAccademicYearDetail.MsgAccademicYearDetailDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceAccademicYearDetail.MsgAccademicYearDetailDeleteFailed;
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

        public JsonResult GetAccademicYearDetailList()
        {
            var accademicYearDetailListObj = this.accademicYearDetailService.GetAllAccademicYearDetail();
            List<AccademicYearDetailModel> accademicYearDetailVMList = new List<AccademicYearDetailModel>();

            foreach (var accademicYearDetail in accademicYearDetailListObj)
            {
                accademicYearDetailVMList.Add(PrepareAccademicYearDetailModel(accademicYearDetail));
            }
            return Json(accademicYearDetailVMList, JsonRequestBehavior.AllowGet);
        }

        private AccademicYearDetailModel PrepareAccademicYearDetailModel(AccademicYearDetail accademicYearDetail)
        {
            AccademicYearDetailModel accademicYearDetailTemp = new AccademicYearDetailModel();
            accademicYearDetailTemp.AccademicYear = accademicYearDetail.AccademicYear;
            accademicYearDetailTemp.DayType = accademicYearDetail.DayType;
            accademicYearDetailTemp.DayTypeName = accademicYearDetail.DayType1.DayType1;
            accademicYearDetailTemp.DayDate = accademicYearDetail.DayDate.AddMinutes(timeZoneOffset).ToString("dd MMM yyyy");
            accademicYearDetailTemp.DayDescription = accademicYearDetail.DayDescription;

            return accademicYearDetailTemp;
        }

        public JsonResult GetAccademicYearDetail(int id)
        {
            var accademicYearDetail = this.accademicYearDetailService.GetAccademicYearDetail(id);
            return Json(accademicYearDetail);
        }
    }

}