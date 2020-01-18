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
    public class BoardorUniversityController : Controller
    {
        public readonly IBoardOrUniversityService boardorUniversityService;
        public readonly ISubModuleItemService subModuleItemService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        public BoardorUniversityController(IBoardOrUniversityService boardorUniversityService, ISubModuleItemService subModuleItemService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.boardorUniversityService = boardorUniversityService;
            this.subModuleItemService = subModuleItemService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        string cacheKey = "permission:boardorUniversity" /*+ Helpers.UserSession.GetUserFromSession().RoleId*/;
        RoleSubModuleItem permission = null;

        
        // GET: /BoardorUniversity/
        public ActionResult Index()
        {
            const string url = "/BoardorUniversity/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId) ;

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set(cacheKey, permission, 240);
                    return View("BoardorUniversity");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            
            return View("~/Views/Shared/NoPermission.cshtml");
        }


        public ActionResult BoardorUniversity()
        {
            return View();
        }



        [HttpPost]
        public JsonResult CreateBoardorUniversity(BoardOrUniversity boardorUniversity)
        {
            var isSuccess = false;
            var message = string.Empty;
            var isNew = boardorUniversity.Id == 0 ? true : false;
            const string url = "/BoardorUniversity/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ??
                         roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(boardorUniversity))
                    {
                        if (this.boardorUniversityService.CreateBoardOrUniversity(boardorUniversity))
                        {
                            isSuccess = true;
                            message = Resources.ResourceBoardorUniversity.MsgBoardorUniversitySaveSuccessful;
                        }
                        else
                        {
                            message = Resources.ResourceBoardorUniversity.MsgBoardorUniversitySaveFailed;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceBoardorUniversity.MsgDuplicateBoardorUniversity;
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
                    if (this.boardorUniversityService.UpdateBoardOrUniversity(boardorUniversity))
                    {
                        isSuccess = true;
                        message = Resources.ResourceBoardorUniversity.MsgBoardorUniversityUpdateSuccessful;
                    }
                    else
                    {
                        message = Resources.ResourceBoardorUniversity.MsgBoardorUniversityUpdateFailed;
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

        private bool CheckIsExist(Model.BoardOrUniversity boardorUniversity)
        {
            return this.boardorUniversityService.CheckIsExist(boardorUniversity);
        }

        [HttpPost]
        public JsonResult DeleteBoardorUniversity(BoardOrUniversity boardorUniversity)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/BoardorUniversity/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);
            
            if (permission.DeleteOperation == true)
            {
                isSuccess = this.boardorUniversityService.DeleteBoardOrUniversity(boardorUniversity.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceBoardorUniversity.MsgBoardorUniversityDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceBoardorUniversity.MsgBoardorUniversityDeleteFailed;
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

        public JsonResult GetBoardorUniversityList()
        {
            var boardorUniversityListObj = this.boardorUniversityService.GetAllBoardOrUniversity();
            List<BoardorUniversityModel> boardorUniversityVMList = new List<BoardorUniversityModel>();

            foreach (var boardorUniversity in boardorUniversityListObj)
            {
                boardorUniversityVMList.Add(PrepareBoardorUniversityModel(boardorUniversity));
            }

            return Json(boardorUniversityVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActiveBoardorUniversityList()
        {
            var boardorUniversityListObj = this.boardorUniversityService.GetAllBoardOrUniversity().Where(x=>x.IsActive==true);
            List<BoardorUniversityModel> boardorUniversityVMList = new List<BoardorUniversityModel>();

            foreach (var boardorUniversity in boardorUniversityListObj)
            {
                boardorUniversityVMList.Add(PrepareBoardorUniversityModel(boardorUniversity));
            }

            return Json(boardorUniversityVMList, JsonRequestBehavior.AllowGet);
        }

        private static BoardorUniversityModel PrepareBoardorUniversityModel(BoardOrUniversity boardorUniversity)
        {
            BoardorUniversityModel boardorUniversityTemp = new BoardorUniversityModel();
            boardorUniversityTemp.Id = boardorUniversity.Id;
            boardorUniversityTemp.Name = boardorUniversity.Name;
            if (boardorUniversity.IsActive != null)
                boardorUniversityTemp.IsActive = boardorUniversity.IsActive.Value;
            return boardorUniversityTemp;
        }

        public JsonResult GetBoardorUniversity(int id)
        {
            var boardorUniversity = this.boardorUniversityService.GetBoardOrUniversity(id);
            return Json(boardorUniversity);
        }

    }    
}