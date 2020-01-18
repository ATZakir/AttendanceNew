using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Attendance.CachingService;
using Attendance.Model;
using Attendance.Service;
using Attendance.ClientModel;
using Attendance.Web.Helpers;

namespace Attendance.Web.Controllers
{
    public class ActionLogController : Controller
    {
        public readonly IActionLogService actionLogService;
        public readonly ISubModuleItemService subModuleItemService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        public ActionLogController(IActionLogService actionLogService, ISubModuleItemService subModuleItemService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.actionLogService = actionLogService;
            this.subModuleItemService = subModuleItemService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        string cacheKey = "permission:actionLog" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;
        protected long timeZoneOffset = UserSession.GetTimeZoneOffset();

        // GET: /ActionLog/
        public ActionResult Index()
        {
            const string url = "/ActionLog/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId) ;

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set("permission:actionLog" + Helpers.UserSession.GetUserFromSession().RoleId, permission,240);
                    return View("ActionLog");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            
            return View("~/Views/Shared/NoPermission.cshtml");
        }


        public ActionResult ActionLog()
        {
            return View();
        }



        [HttpPost]
        public JsonResult CreateActionLog(ActionLog actionLog)
        {
            var isSuccess = false;
            var message = string.Empty;
            var isNew = actionLog.Id == Guid.Empty ? true : false;
            const string url = "/ActionLog/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(actionLog))
                    {
                        if (this.actionLogService.CreateActionLog(actionLog))
                        {
                            isSuccess = true;
                            message = "ActionLog Saved Successfully!";
                        }
                        else
                        {
                            message = "ActionLog could not Saved!";
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = "Can't save. Same actionLog name found!";
                    }
                }
                else
                {
                    message = "You don't have permission to create";
                }

                
            }
            else
            {
                if (permission.UpdateOperation == true)
                {
                    if (this.actionLogService.UpdateActionLog(actionLog))
                    {
                        isSuccess = true;
                        message = "ActionLog updated successfully!";
                    }
                    else
                    {
                        message = "ActionLog could not updated!";
                    }
                }
                else
                {
                    message = "You don't have permission to update"; 
                }

            }

            return Json(new
            {
                isSuccess = isSuccess,
                message = message,
            }, JsonRequestBehavior.AllowGet);
        }
        private bool CheckIsExist(Model.ActionLog actionLog)
        {
            return false;
            //return this.actionLogService.CheckIsExist(actionLog);
        }
        [HttpPost]
        public JsonResult DeleteActionLog(ActionLog actionLog)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/ActionLog/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);
            
            if (permission.DeleteOperation == true)
            {
                isSuccess = this.actionLogService.DeleteActionLog(actionLog.Id);
                if (isSuccess)
                {
                    message = "ActionLog deleted successfully!";

                }
                else
                {
                    message = "ActionLog can't be deleted!";
                }
            }
            else
            {
                message = "You don't have permission to delete";
            }


            return Json(new
            {
                isSuccess = isSuccess,
                message = message
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActionLogList()
        {
            var actionLogListObj = this.actionLogService.GetAllActionLog();
            List<ActionLogModel> actionLogVMList = new List<ActionLogModel>();

            foreach (var actionLog in actionLogListObj)
            {
                ActionLogModel actionLogTemp = new ActionLogModel();
                //actionLogTemp.Id = actionLog.Id;
                //actionLogTemp.Name = actionLog.Name;
                //actionLogTemp.Code = actionLog.Code;

                actionLogVMList.Add(actionLogTemp);
            }
            return Json(actionLogVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActionLog(Guid id)
        {
            var actionLog = this.actionLogService.GetActionLog(id);
            return Json(actionLog);
        }
    }

}