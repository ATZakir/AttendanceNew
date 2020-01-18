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
    public class LeaveTypeController : Controller
    {
        public readonly ILeaveTypeService leaveTypeService;
        public readonly ISubModuleItemService subModuleItemService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        public LeaveTypeController(ILeaveTypeService leaveTypeService, ISubModuleItemService subModuleItemService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.leaveTypeService = leaveTypeService;
            this.subModuleItemService = subModuleItemService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        string cacheKey = "permission:leaveType" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        
        // GET: /LeaveType/
        public ActionResult Index()
        {
            const string url = "/LeaveType/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId) ;

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set(cacheKey, permission, 240);
                    return View("LeaveType");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            
            return View("~/Views/Shared/NoPermission.cshtml");
        }


        public ActionResult LeaveType()
        {
            return View();
        }



        [HttpPost]
        public JsonResult CreateLeaveType(LeaveType leaveType)
        {
            var isSuccess = false;
            var message = string.Empty;
            var isNew = leaveType.Id == 0 ? true : false;
            const string url = "/LeaveType/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(leaveType))
                    {
                        if (this.leaveTypeService.CreateLeaveType(leaveType))
                        {
                            isSuccess = true;
                            message = Resources.ResourceLeaveType.MsgLeaveTypeSaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceLeaveType.MsgLeaveTypeSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceLeaveType.MsgDuplicateLeaveType;
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
                    if (this.leaveTypeService.UpdateLeaveType(leaveType))
                    {
                        isSuccess = true;
                        message = Resources.ResourceLeaveType.MsgLeaveTypeUpdateSuccessful;
                    }
                    else
                    {
                        message = Resources.ResourceLeaveType.MsgLeaveTypeUpdateFailed;
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

        private bool CheckIsExist(Model.LeaveType leaveType)
        {
            return this.leaveTypeService.CheckIsExist(leaveType);
        }

        [HttpPost]
        public JsonResult DeleteLeaveType(LeaveType leaveType)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/LeaveType/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);
            
            if (permission.DeleteOperation == true)
            {
                isSuccess = this.leaveTypeService.DeleteLeaveType(leaveType.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceLeaveType.MsgLeaveTypeDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceLeaveType.MsgLeaveTypeDeleteFailed;
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

        public JsonResult GetLeaveTypeList()
        {
            var leaveTypeListObj = this.leaveTypeService.GetAllLeaveType();
            List<LeaveTypeModel> leaveTypeVMList = new List<LeaveTypeModel>();

            foreach (var leaveType in leaveTypeListObj)
            {
                leaveTypeVMList.Add(PrepareLeaveTypeModel(leaveType));
            }

            return Json(leaveTypeVMList, JsonRequestBehavior.AllowGet);
        }

        private static LeaveTypeModel PrepareLeaveTypeModel(LeaveType leaveType)
        {
            LeaveTypeModel leaveTypeTemp = new LeaveTypeModel();
            leaveTypeTemp.Id = leaveType.Id;
            leaveTypeTemp.Name = leaveType.Name;
            leaveTypeTemp.CarryForward = leaveType.CarryForward;
            leaveTypeTemp.PayDays = leaveType.PayDays;
            leaveTypeTemp.PaymentMode = leaveType.PaymentMode;
            leaveTypeTemp.Remarks = leaveType.Remarks;
            leaveTypeTemp.ShortName = leaveType.ShortName;
            if (leaveType.IsActive != null)
                leaveTypeTemp.IsActive = leaveType.IsActive.Value;

            return leaveTypeTemp;
        }

        public JsonResult GetActiveLeaveTypeList()
        {
            var leaveTypeListObj = this.leaveTypeService.GetAllLeaveType().Where(x => x.IsActive == true);
            List<LeaveTypeModel> leaveTypeVMList = new List<LeaveTypeModel>();

            foreach (var leaveType in leaveTypeListObj)
            {
                leaveTypeVMList.Add(PrepareLeaveTypeModel(leaveType));
            }

            return Json(leaveTypeVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLeaveType(int id)
        {
            var leaveType = this.leaveTypeService.GetLeaveType(id);
            return Json(leaveType);
        }

    }    
}