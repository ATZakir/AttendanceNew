using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Attendance.CachingService;
using Attendance.ClientModel;
using Attendance.Model;
using Attendance.Service;


namespace Attendance.Web.Controllers
{
    public class AccademicYearController : Controller
    {
        public readonly IAccademicYearService accademicYearService;
        public readonly ISubModuleItemService subModuleItemService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();
        public readonly IAccademicYearDetailService accademicYearDetailService;

        public AccademicYearController(IAccademicYearService accademicYearService, ISubModuleItemService subModuleItemService, IRoleSubModuleItemService roleSubModuleItemService, IAccademicYearDetailService accademicYearDetailService)
        {
            this.accademicYearService = accademicYearService;
            this.subModuleItemService = subModuleItemService;
            this.roleSubModuleItemService = roleSubModuleItemService;
            this.accademicYearDetailService = accademicYearDetailService;
        }

        string cacheKey = "permission:accademicYear" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        
        // GET: /AccademicYear/
        public ActionResult Index()
        {
            const string url = "/AccademicYear/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId) ;

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set(cacheKey, permission, 240);
                    return View("AccademicYear");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            
            return View("~/Views/Shared/NoPermission.cshtml");
        }


        public ActionResult AccademicYear()
        {
            return View();
        }



        [HttpPost]
        public JsonResult CreateAccademicYear(AccademicYear accademicYear)
        {
            var isSuccess = false;
            var message = string.Empty;
            var accademicYearObj = accademicYearService.GetAccademicYear(accademicYear.AccademicYear1);
            var isNew = accademicYearObj == null;
            const string url = "/AccademicYear/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(accademicYear))
                    {
                        if (this.accademicYearService.CreateAccademicYear(accademicYear))
                        {
                            isSuccess = true;
                            message = Resources.ResourceAccademicYear.MsgAccademicYearSaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceAccademicYear.MsgAccademicYearSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceAccademicYear.MsgDuplicateAccademicYear;
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
                    accademicYearObj.AccademicYear1 = accademicYear.AccademicYear1;
                    accademicYearObj.Weekend1 = accademicYear.Weekend1;
                    accademicYearObj.Weekend2 = accademicYear.Weekend2;

                    if (this.accademicYearService.UpdateAccademicYear(accademicYearObj))
                    {
                        isSuccess = true;
                        message = Resources.ResourceAccademicYear.MsgAccademicYearUpdateSuccessful;
                    }
                    else
                    {
                        message = Resources.ResourceAccademicYear.MsgAccademicYearUpdateFailed;
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

        private bool CheckIsExist(Model.AccademicYear accademicYear)
        {
            return this.accademicYearService.CheckIsExist(accademicYear);
        }

        [HttpPost]
        public JsonResult DeleteAccademicYear(AccademicYear accademicYear)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/AccademicYear/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);
            
            if (permission.DeleteOperation == true)
            {
                isSuccess = this.accademicYearService.DeleteAccademicYear(accademicYear.AccademicYear1);
                if (isSuccess)
                {
                    message = Resources.ResourceAccademicYear.MsgAccademicYearDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceAccademicYear.MsgAccademicYearDeleteFailed;
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

        public JsonResult GetAccademicYearList()
        {
            var accademicYearListObj = this.accademicYearService.GetAllAccademicYear();
            List<AccademicYearModel> accademicYearVMList = new List<AccademicYearModel>();

            foreach (var accademicYear in accademicYearListObj)
            {
                accademicYearVMList.Add(PrepareAccademicYearModel(accademicYear));
            }

            return Json(accademicYearVMList, JsonRequestBehavior.AllowGet);
        }

        private static AccademicYearModel PrepareAccademicYearModel(AccademicYear accademicYear)
        {
            AccademicYearModel accademicYearTemp = new AccademicYearModel();
            accademicYearTemp.AccademicYear1 = accademicYear.AccademicYear1;
            accademicYearTemp.Weekend1 = accademicYear.Weekend1;
             accademicYearTemp.Weekend2 = accademicYear.Weekend2;
            return accademicYearTemp;
        }

        public JsonResult GetActiveAccademicYearList()
        {
            var accademicYearListObj = this.accademicYearService.GetAllAccademicYear();
            List<AccademicYearModel> accademicYearVMList = new List<AccademicYearModel>();

            foreach (var accademicYear in accademicYearListObj)
            {
                accademicYearVMList.Add(PrepareAccademicYearModel(accademicYear));
            }

            return Json(accademicYearVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAccademicYear(int id)
        {
            var accademicYear = this.accademicYearService.GetAccademicYear(id);
            return Json(accademicYear);
        }

        public JsonResult GetWeekends(int year)
        {
            var isSuccess = false;
            var message = string.Empty;
            DateTime startDate = new DateTime(year, 1, 1);
            DateTime endDate = startDate.AddYears(1);

            var accademicYear = this.accademicYearService.GetAccademicYear(year);
            int weekend1 = accademicYear.Weekend1;
            var weekend2 = accademicYear.Weekend2;

            TimeSpan diff = endDate - startDate;
            int days = diff.Days;
            for (var i = 0; i <= days; i++)
            {
                var testDate = startDate.AddDays(i);
                if ((int)testDate.DayOfWeek == weekend1 || (int)testDate.DayOfWeek == weekend2)
                {
                    var weekendDate = testDate.ToShortDateString();
                   var weekendDateDetailTemp = new AccademicYearDetail();
                    weekendDateDetailTemp.AccademicYear = year;
                    weekendDateDetailTemp.DayDate = Convert.ToDateTime(weekendDate);
                    weekendDateDetailTemp.DayType = 2;
                    weekendDateDetailTemp.DayDescription = "Weekend";

                    if (this.accademicYearDetailService.CreateAccademicYearDetail(weekendDateDetailTemp))
                    {
                        isSuccess = true;
                        message = Resources.ResourceAccademicYear.MsgAccademicYearUpdateSuccessful;
                    }
                }
            }


            return Json(new
            {
                isSuccess = isSuccess,
                message = message,
            }, JsonRequestBehavior.AllowGet);
        }

    }    
}