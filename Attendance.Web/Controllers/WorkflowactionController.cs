using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Attendance.CachingService;
using Attendance.Model;
using Attendance.Service;
using Attendance.ClientModel;

namespace Attendance.Web.Controllers
{
    public class WorkflowactionController : Controller
    {
        public readonly IWorkflowactionService workflowactionService;
        public readonly ISubModuleItemService subModuleItemService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        public WorkflowactionController(IWorkflowactionService workflowactionService, ISubModuleItemService subModuleItemService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.workflowactionService = workflowactionService;
            this.subModuleItemService = subModuleItemService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        string cacheKey = "permission:workflowaction" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;


        // GET: /Workflowaction/
        public ActionResult Index()
        {
            const string url = "/Workflowaction/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set("permission:workflowaction" + Helpers.UserSession.GetUserFromSession().RoleId, permission, 240);
                    return View("Workflowaction");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }

            return View("~/Views/Shared/NoPermission.cshtml");
        }


        public ActionResult Workflowaction()
        {
            return View();
        }



        [HttpPost]
        public JsonResult CreateWorkflowaction(Workflowaction workflowaction)
        {
            var isSuccess = false;
            var message = string.Empty;
            var isNew = workflowaction.Id == 0 ? true : false;
            const string url = "/Workflowaction/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (isNew)
            {

                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(workflowaction))
                    {

                        if (this.workflowactionService.CreateWorkflowaction(workflowaction))
                        {
                            isSuccess = true;
                            message = "Workflow action saved successfully!";
                        }
                        else
                        {
                            message = "Workflow action could not be saved!";
                        }


                    }
                    else
                    {
                        isSuccess = false;
                        message = "Can't save. Same workflow action name found!";
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
                    if (this.workflowactionService.UpdateWorkflowaction(workflowaction))
                    {
                        isSuccess = true;
                        message = "Workflow action updated successfully!";
                    }
                    else
                    {
                        message = "Workflow action could not be updated!";
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
        private bool CheckIsExist(Model.Workflowaction workflowaction)
        {
            return this.workflowactionService.CheckIsExist(workflowaction);
        }
        [HttpPost]
        public JsonResult DeleteWorkflowaction(Workflowaction workflowaction)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/Workflowaction/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                isSuccess = this.workflowactionService.DeleteWorkflowaction(workflowaction.Id);
                if (isSuccess)
                {
                    message = "Workflow action deleted successfully!";

                }
                else
                {
                    message = "Workflow action can't be deleted!";
                }
            }
            else
            {
                message = "You don't have permission to delete ";
            }


            return Json(new
            {
                isSuccess = isSuccess,
                message = message
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWorkflowactionList()
        {
            var workflowactionListObj = this.workflowactionService.GetAllWorkflowaction();
            List<WorkflowactionModel> workflowactionVMList = new List<WorkflowactionModel>();

            foreach (var workflowaction in workflowactionListObj)
            {
                WorkflowactionModel workflowactionTemp = new WorkflowactionModel();
                workflowactionTemp.Id = workflowaction.Id;
                workflowactionTemp.Name = workflowaction.Name;

                workflowactionVMList.Add(workflowactionTemp);
            }
            return Json(workflowactionVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWorkflowaction(int id)
        {
            var workflowaction = this.workflowactionService.GetWorkflowaction(id);
            return Json(workflowaction);
        }
    }

    
}