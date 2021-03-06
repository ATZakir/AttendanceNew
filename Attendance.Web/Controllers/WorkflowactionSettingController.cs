using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Attendance.CachingService;
using Attendance.Model;
using Attendance.Service;
using Attendance.Web.Helpers;
using Attendance.ClientModel;

namespace Attendance.Web.Controllers
{
    public class WorkflowactionSettingController : Controller
    {
        public readonly IWorkflowactionSettingService workflowactionSettingService;
        public readonly ISubModuleItemService subModuleItemService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        public WorkflowactionSettingController(IWorkflowactionSettingService workflowactionSettingService, ISubModuleItemService subModuleItemService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.workflowactionSettingService = workflowactionSettingService;
            this.subModuleItemService = subModuleItemService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        string cacheKey = "permission:workflowactionSetting" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;


        // GET: /WorkflowactionSetting/
        public ActionResult Index()
        {
            const string url = "/WorkflowactionSetting/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set("permission:workflowactionSetting" + Helpers.UserSession.GetUserFromSession().RoleId, permission, 240);
                    return View("WorkflowactionSetting");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }

            return View("~/Views/Shared/NoPermission.cshtml");
        }

        [HttpPost]
        public JsonResult CreateWorkflowactionSettingList(List<WorkflowactionSetting> workflowactionSettingList, int subModuleItemId, int workflowactionId)
        {
            var isSuccess = false;
            var message = string.Empty;
            const string url = "/WorkflowactionSetting/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.CreateOperation == true)
            {
                if (this.workflowactionSettingService.DeleteAllWorkflowactionSettingBySubModuleId(subModuleItemId, workflowactionId))
                {
                    if (workflowactionSettingList != null)
                    {
                        foreach (var workflowactionSetting in workflowactionSettingList)
                        {
                            workflowactionSetting.Id = Guid.NewGuid();
                            if (this.workflowactionSettingService.CreateWorkflowactionSetting(workflowactionSetting))
                            {
                                isSuccess = true;
                            }
                        }
                    }
                    message = "Workflow setting created successfully";
                }
                else
                {
                    message = "Workflow setting could not be created";
                }
            }
            else
            {
                message = "You don't have permission to create";
            }

            return Json(new
            {
                isSuccess = isSuccess,
                message = message,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteWorkflowactionSetting(WorkflowactionSetting workflowactionSetting)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/WorkflowactionSetting/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                isSuccess = this.workflowactionSettingService.DeleteWorkflowactionSetting(workflowactionSetting.Id);
                if (isSuccess)
                {
                    message = "Workflow action setting deleted successfully!";

                }
                else
                {
                    message = "Workflow action setting can't be deleted!";
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

        public JsonResult GetWorkflowactionSettingList()
        {
            var workflowactionSettingListObj = this.workflowactionSettingService.GetAllWorkflowactionSetting();
            List<WorkflowactionSettingModel> workflowactionSettingVMList = new List<WorkflowactionSettingModel>();

            foreach (var workflowactionSetting in workflowactionSettingListObj)
            {
                WorkflowactionSettingModel workflowactionSettingTemp = new WorkflowactionSettingModel();
                workflowactionSettingTemp.Id = workflowactionSetting.Id;
                workflowactionSettingTemp.SubMouduleItemId = workflowactionSetting.SubMouduleItemId;
                workflowactionSettingTemp.UserId = workflowactionSetting.EmployeeId;
                if (workflowactionSetting.EmployeeId > 0)
                        workflowactionSettingTemp.UserName = workflowactionSetting.Employee.FullName;

                workflowactionSettingVMList.Add(workflowactionSettingTemp);
            }
            return Json(workflowactionSettingVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWorkflowactionSetting(Guid id)
        {
            var workflowactionSetting = this.workflowactionSettingService.GetWorkflowactionSetting(id);
            return Json(workflowactionSetting);
        }

        [HttpPost]
        public JsonResult CheckLoggedUserInWorkflowactionSettingForUrl(string url, int workflowactionId)
        {
            bool isLoggedUserApproved = false;
            var userObj = UserSession.GetUserFromSession();
            if (userObj != null)
            {
                 if(userObj.Id != 0)
                 {
                     isLoggedUserApproved = this.workflowactionSettingService.CheckUserInWorkflowactionSettingForUrl(url,userObj.Id, workflowactionId);
                 }
            }
            return Json(isLoggedUserApproved);
        }
    }
}