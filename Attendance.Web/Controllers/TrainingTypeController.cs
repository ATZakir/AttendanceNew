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
    public class TrainingTypeController : Controller
    {
        public readonly ITrainingTypeService trainingTypeService;
        public readonly ISubModuleItemService subModuleItemService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        public TrainingTypeController(ITrainingTypeService trainingTypeService, ISubModuleItemService subModuleItemService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.trainingTypeService = trainingTypeService;
            this.subModuleItemService = subModuleItemService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        string cacheKey = "permission:trainingType" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        
        // GET: /TrainingType/
        public ActionResult Index()
        {
            const string url = "/TrainingType/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId) ;

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set(cacheKey, permission, 240);
                    return View("TrainingType");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            
            return View("~/Views/Shared/NoPermission.cshtml");
        }


        public ActionResult TrainingType()
        {
            return View();
        }



        [HttpPost]
        public JsonResult CreateTrainingType(TrainingType trainingType)
        {
            var isSuccess = false;
            var message = string.Empty;
            var isNew = trainingType.Id == 0 ? true : false;
            const string url = "/TrainingType/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(trainingType))
                    {
                        if (this.trainingTypeService.CreateTrainingType(trainingType))
                        {
                            isSuccess = true;
                            message = Resources.ResourceTrainingType.MsgTrainingTypeSaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceTrainingType.MsgTrainingTypeSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceTrainingType.MsgDuplicateTrainingType;
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
                    if (this.trainingTypeService.UpdateTrainingType(trainingType))
                    {
                        isSuccess = true;
                        message = Resources.ResourceTrainingType.MsgTrainingTypeUpdateSuccessful;
                    }
                    else
                    {
                        message = Resources.ResourceTrainingType.MsgTrainingTypeUpdateFailed;
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

        private bool CheckIsExist(Model.TrainingType trainingType)
        {
            return this.trainingTypeService.CheckIsExist(trainingType);
        }

        [HttpPost]
        public JsonResult DeleteTrainingType(TrainingType trainingType)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/TrainingType/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);
            
            if (permission.DeleteOperation == true)
            {
                isSuccess = this.trainingTypeService.DeleteTrainingType(trainingType.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceTrainingType.MsgTrainingTypeDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceTrainingType.MsgTrainingTypeDeleteFailed;
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

        public JsonResult GetTrainingTypeList()
        {
            var trainingTypeListObj = this.trainingTypeService.GetAllTrainingType();
            List<TrainingTypeModel> trainingTypeVMList = new List<TrainingTypeModel>();

            foreach (var trainingType in trainingTypeListObj)
            {
                trainingTypeVMList.Add(PrepareTrainingTypeModel(trainingType));
            }

            return Json(trainingTypeVMList, JsonRequestBehavior.AllowGet);
        }

        private static TrainingTypeModel PrepareTrainingTypeModel(TrainingType trainingType)
        {
            TrainingTypeModel trainingTypeTemp = new TrainingTypeModel();
            trainingTypeTemp.Id = trainingType.Id;
            trainingTypeTemp.Name = trainingType.Name;
            if (trainingType.IsActive != null)
                trainingTypeTemp.IsActive = trainingType.IsActive.Value;
            return trainingTypeTemp;
        }

        public JsonResult GetActiveTrainingTypeList()
        {
            var trainingTypeListObj = this.trainingTypeService.GetAllTrainingType().Where(x => x.IsActive == true);
            List<TrainingTypeModel> trainingTypeVMList = new List<TrainingTypeModel>();

            foreach (var trainingType in trainingTypeListObj)
            {
                trainingTypeVMList.Add(PrepareTrainingTypeModel(trainingType));
            }

            return Json(trainingTypeVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTrainingType(int id)
        {
            var trainingType = this.trainingTypeService.GetTrainingType(id);
            return Json(trainingType);
        }

    }    
}