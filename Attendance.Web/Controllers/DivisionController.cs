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
    public class DivisionController : Controller
    {
        public readonly IDivisionService divisionService;
        public readonly ISubModuleItemService subModuleItemService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        public DivisionController(IDivisionService divisionService, ISubModuleItemService subModuleItemService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.divisionService = divisionService;
            this.subModuleItemService = subModuleItemService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        string cacheKey = "permission:division" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        
        // GET: /Division/
        public ActionResult Index()
        {
            const string url = "/Division/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId) ;

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set(cacheKey, permission, 240);
                    return View("Division");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            
            return View("~/Views/Shared/NoPermission.cshtml");
        }


        public ActionResult Division()
        {
            return View();
        }



        [HttpPost]
        public JsonResult CreateDivision(Division division)
        {
            var isSuccess = false;
            var message = string.Empty;
            var isNew = division.Id == 0 ? true : false;
            const string url = "/Division/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(division))
                    {
                        if (this.divisionService.CreateDivision(division))
                        {
                            isSuccess = true;
                            message = Resources.ResourceDivision.MsgDivisionSaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceDivision.MsgDivisionSaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceDivision.MsgDuplicateDivision;
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
                    if (this.divisionService.UpdateDivision(division))
                    {
                        isSuccess = true;
                        message = Resources.ResourceDivision.MsgDivisionUpdateSuccessful;
                    }
                    else
                    {
                        message = Resources.ResourceDivision.MsgDivisionUpdateFailed;
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

        private bool CheckIsExist(Model.Division division)
        {
            return this.divisionService.CheckIsExist(division);
        }

        [HttpPost]
        public JsonResult DeleteDivision(Division division)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/Division/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);
            
            if (permission.DeleteOperation == true)
            {
                isSuccess = this.divisionService.DeleteDivision(division.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceDivision.MsgDivisionDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceDivision.MsgDivisionDeleteFailed;
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

        public JsonResult GetDivisionList()
        {
            var divisionListObj = this.divisionService.GetAllDivision();
            List<DivisionModel> divisionVMList = new List<DivisionModel>();

            foreach (var division in divisionListObj)
            {
                divisionVMList.Add(PrepareDivisionModel(division));
            }

            return Json(divisionVMList, JsonRequestBehavior.AllowGet);
        }

        private static DivisionModel PrepareDivisionModel(Division division)
        {
            DivisionModel divisionTemp = new DivisionModel();
            divisionTemp.Id = division.Id;
            divisionTemp.Name = division.Name;
            if (division.IsActive != null)
                divisionTemp.IsActive = division.IsActive.Value;
            return divisionTemp;
        }

        public JsonResult GetActiveDivisionList()
        {
            var divisionListObj = this.divisionService.GetAllDivision().Where(x => x.IsActive == true);
            List<DivisionModel> divisionVMList = new List<DivisionModel>();

            foreach (var division in divisionListObj)
            {
                divisionVMList.Add(PrepareDivisionModel(division));
            }

            return Json(divisionVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDivision(int id)
        {
            var division = this.divisionService.GetDivision(id);
            return Json(division);
        }

    }    
}