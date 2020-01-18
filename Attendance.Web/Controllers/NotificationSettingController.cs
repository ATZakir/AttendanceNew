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
    public class NotificationSettingController : Controller
    {
        public readonly INotificationSettingService notificationSettingService;
        public readonly ISubModuleItemService subModuleItemService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        public NotificationSettingController(INotificationSettingService notificationSettingService, ISubModuleItemService subModuleItemService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.notificationSettingService = notificationSettingService;
            this.subModuleItemService = subModuleItemService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        string cacheKey = "permission:notificationSetting" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;


        // GET: /NotificationSetting/
        public ActionResult Index()
        {
            const string url = "/NotificationSetting/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set(cacheKey, permission, 240);
                    return View("NotificationSetting");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }

            return View("~/Views/Shared/NoPermission.cshtml");
        }

        [HttpPost]
        public JsonResult CreateNotificationSettingList(List<NotificationSetting> notificationSettingList, int subModuleItemId, int workflowactionId)
        {
            var isSuccess = false;
            var message = string.Empty;
            const string url = "/NotificationSetting/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.CreateOperation == true)
            {
                if (this.notificationSettingService.DeleteAllNotificationSettingBySubModuleId(subModuleItemId, workflowactionId))
                {
                    if (notificationSettingList != null)
                    {
                        foreach (var notificationSetting in notificationSettingList)
                        {
                            notificationSetting.Id = Guid.NewGuid();
                            if (this.notificationSettingService.CreateNotificationSetting(notificationSetting))
                            {
                                isSuccess = true;
                            }
                        }
                    }
                    message = "Notification setting created successfuly";
                }
                else
                {
                    message = "Notification setting could not be created!";
                }
            }
            else
            {//TODO
                message = "You don't have permission to create";
            }

            return Json(new
            {
                isSuccess = isSuccess,
                message = message,
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult DeleteNotificationSetting(NotificationSetting notificationSetting)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/NotificationSetting/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                isSuccess = this.notificationSettingService.DeleteNotificationSetting(notificationSetting.Id);
                if (isSuccess)
                {
                    message = "Notification setting deleted successfully!";

                }
                else
                {
                    message = "Notification setting can't be deleted!";
                }
            }
            else
            {//TODO
                message = "You don't have permission to delete"; ;
            }


            return Json(new
            {
                isSuccess = isSuccess,
                message = message
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNotificationSettingList()
        {
            var notificationSettingListObj = this.notificationSettingService.GetAllNotificationSetting();
            List<NotificationSettingModel> notificationSettingVmList = new List<NotificationSettingModel>();

            foreach (var notificationSetting in notificationSettingListObj)
            {
                NotificationSettingModel notificationSettingTemp = new NotificationSettingModel();
                notificationSettingTemp.Id = notificationSetting.Id;

                notificationSettingTemp.SubModuleItemId = notificationSetting.SubModuleItemId;
                if (notificationSetting.SubModuleItemId != null)
                    notificationSettingTemp.SubModuleItemName = notificationSetting.SubModuleItem.Name;

                notificationSettingTemp.NotifiedUserId = notificationSetting.NotifiedEmployeeId;
                if (notificationSetting.NotifiedEmployeeId != null)
                    notificationSettingTemp.UserName = notificationSetting.Employee.FullName;

                notificationSettingTemp.WorkflowactionId = notificationSetting.WorkflowactionId;
                if (notificationSetting.WorkflowactionId != null)
                    notificationSettingTemp.WorkflowactionName = notificationSetting.Workflowaction.Name;

                notificationSettingVmList.Add(notificationSettingTemp);
            }
            return Json(notificationSettingVmList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNotificationSetting(Guid id)
        {
            var notificationSetting = this.notificationSettingService.GetNotificationSetting(id);
            return Json(notificationSetting);
        }
    }

 
}