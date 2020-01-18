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
    public class NoticeController : BaseImageController
    {
        public readonly IDistrictService districtService;
        public readonly INoticeService noticeService;
        public readonly IRoleSubModuleItemService roleSubModuleItemService;
        private static readonly ICacheProvider cacheProvider = new DefaultCacheProvider();

        protected long timeZoneOffset = UserSession.GetTimeZoneOffset();

        string cacheKey = "permission:notice" + Helpers.UserSession.GetUserFromSession().RoleId;
        RoleSubModuleItem permission = null;

        // GET: /Notice/
        public ActionResult Index()
        {
            var url = Request.RawUrl;

            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey);
            if (permission == null)
                permission = roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission != null)
            {
                if (permission.ReadOperation == true)
                {
                    cacheProvider.Set(cacheKey, permission, 240);
                    return View("Notice");
                }
                else
                {
                    return View("~/Views/Shared/NoPermission.cshtml");
                }
            }
            return View("~/Views/Shared/NoPermission.cshtml");
        }

        
        public ActionResult Detail(int id)
        {
            ViewBag.NoticeId = id;
            return View();
        }

        public NoticeController(INoticeService noticeService, IDistrictService districtService, IRoleSubModuleItemService roleSubModuleItemService)
        {
            this.noticeService = noticeService;
            this.districtService = districtService;
            this.roleSubModuleItemService = roleSubModuleItemService;
        }

        [HttpPost]
        public JsonResult CreateNotice(Notice notice)
        {
            const string url = "/Notice/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey);
            if (permission == null)
                permission = roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url, Helpers.UserSession.GetUserFromSession().RoleId);

            var isSuccess = false;
            var message = string.Empty;
            

            var noticeObj = noticeService.GetNotice(notice.Id);
            var isNew = noticeObj == null;
            if (isNew)
            {
                if (permission.CreateOperation == true)
                {
                    if (!CheckIsExist(notice))
                    {
                        if (Request.Files["file"] != null)
                        {
                            HttpPostedFileBase file = Request.Files["file"];

                            int MaxContentLengthForImage = 1024 * 1024 * 2; //Size = 2 MB  
                            int MaxContentLengthForPDF = 1024 * 1024 * 2; //Size = 2 MB  

                            IList<string> allowedFileExtensions = new List<string> {".png", ".pdf" };
                            var ext = file.FileName.Substring(file.FileName.LastIndexOf('.'));
                            var extension = ext.ToLower();
                            if (!allowedFileExtensions.Contains(extension))
                            {
                                isSuccess = false;
                                message = Resources.ResourceNotice.MsgFileType;
                            }
                            else if (extension == ".pdf" && file.ContentLength > MaxContentLengthForPDF)
                            {
                                isSuccess = false;
                                message = Resources.ResourceNotice.MsgFileSize;
                            }
                            else if (extension != ".pdf" && file.ContentLength > MaxContentLengthForImage)
                            {
                                isSuccess = false;
                                message = Resources.ResourceNotice.MsgFileSize;
                            }
                            else
                            {
                                string filenamepart = notice.Title.Replace(" ", "");
                                string imageFileName = filenamepart + '_' + file.FileName.Replace
                                                           (" ", "_").ToLowerInvariant();

                                if (base.UploadFile(imageFileName, "Notice", file))
                                    notice.AttachFile = imageFileName;

                                if (this.noticeService.CreateNotice(notice))
                                {
                                    isSuccess = true;
                                    message = Resources.ResourceNotice.MsgNoticeSaveSuccessful;
                                }
                                else
                                {
                                    message = Resources.ResourceNotice.MsgNoticeSaveFailed;
                                }
                            }
                            
                        }
                        
                    }
                    else
                    {
                        isSuccess = false;
                        message = Resources.ResourceNotice.MsgDuplicateNotice;
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
                    var isUpdate = noticeService.GetNotice(notice.Id);
                    if (Request.Files["file"] != null)
                    {
                        HttpPostedFileBase file = Request.Files["file"];
                        string filenamepart = notice.Title.Replace(" ", "");
                        string imageFileName = filenamepart + '_' + file.FileName.Replace
                            (" ", "_").ToLowerInvariant();

                        if (base.UploadFile(imageFileName, "Notice", file))
                            notice.AttachFile = imageFileName;
                    }
                    isUpdate.Id = notice.Id;
                    isUpdate.AttachFile = notice.AttachFile;
                    isUpdate.Description = notice.Description;
                    isUpdate.Title = notice.Title;
                    isUpdate.Validity = notice.Validity;
                    if (this.noticeService.UpdateNotice(isUpdate))
                    {
                        isSuccess = true;
                        message = Resources.ResourceNotice.MsgNoticeUpdateSuccessful;
                    }
                    else
                    {
                        message = Resources.ResourceNotice.MsgNoticeUpdateFailed;
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
        private bool CheckIsExist(Notice notice)
        {
            return this.noticeService.CheckIsExist(notice);
        }

        [HttpPost]
        public JsonResult DeleteNotice(Notice notice)
        {
            var isSuccess = true;
            var message = string.Empty;
            const string url = "/Notice/Index";
            permission = (RoleSubModuleItem)cacheProvider.Get(cacheKey) ?? roleSubModuleItemService.GetRoleSubModuleItemBySubModuleIdandRole(url,
                                Helpers.UserSession.GetUserFromSession().RoleId);

            if (permission.DeleteOperation == true)
            {
                isSuccess = this.noticeService.DeleteNotice(notice.Id);
                if (isSuccess)
                {
                    message = Resources.ResourceNotice.MsgNoticeDeleteSuccessful;
                }
                else
                {
                    message = Resources.ResourceNotice.MsgNoticeDeleteFailed;
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

        public JsonResult GetNoticeList()
        {
            var noticeListObj = this.noticeService.GetAllNotice();
            List<NoticeModel> noticeVMList = new List<NoticeModel>();

            foreach (var notice in noticeListObj)
            {
                noticeVMList.Add(PrepareNoticeModel(notice));
            }
            return Json(noticeVMList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNoticeListForAdmin()
        {
            var noticeListObj = this.noticeService.GetAllNotice().Where(x=>x.Validity != null && x.Validity.Value.Date >= DateTime.Now.Date);
            List<NoticeModel> noticeVMList = new List<NoticeModel>();

            foreach (var notice in noticeListObj)
            {
                noticeVMList.Add(PrepareNoticeModel(notice));
            }
            return Json(noticeVMList, JsonRequestBehavior.AllowGet);
        }



        private NoticeModel PrepareNoticeModel(Notice notice)
        {
            NoticeModel noticeTemp = new NoticeModel();
            noticeTemp.Id = notice.Id;
            noticeTemp.AttachFile = notice.AttachFile;
            noticeTemp.Description = notice.Description;
            noticeTemp.Title = notice.Title;
            if (notice.Validity != null)
                noticeTemp.Validity = notice.Validity.Value.AddMinutes(timeZoneOffset).ToString("dd MMM yyyy");
            

            return noticeTemp;
        }

        public JsonResult GetActiveNoticeList()
        {
            var noticeListObj = this.noticeService.GetAllNotice()/*.Where(x => x.IsActive == true)*/;
            List<NoticeModel> noticeVMList = new List<NoticeModel>();

            foreach (var notice in noticeListObj)
            {
                noticeVMList.Add(PrepareNoticeModel(notice));
            }
            return Json(noticeVMList, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult GetNotice(int id)
        {
            var notice = this.noticeService.GetNotice(id);
            var noticeModel = new NoticeDetailModel();
            if (notice != null)
            {
                noticeModel.Id = notice.Id;
                noticeModel.Title = notice.Title;
                noticeModel.Description = notice.Description;
                if (notice.Validity != null) 
                    noticeModel.Validity = notice.Validity.Value.ToString("D");
                noticeModel.AttachFile = notice.AttachFile;
                if (noticeModel.AttachFile != null)
                {
                    var extension = noticeModel.AttachFile.Substring(noticeModel.AttachFile.LastIndexOf('.') + 1);
                    noticeModel.FileType = extension == "pdf" ? "pdf" : "image";
                }
                
            }

            return Json(noticeModel, JsonRequestBehavior.AllowGet);
        }

        public long GetLastId()
        {
            long lastId = this.noticeService.GetLastId();
            return lastId;
        }
    }

}