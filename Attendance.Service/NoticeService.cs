using Attendance.Data.Repository;
using Attendance.Data.Infrastructure;
using Attendance.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attendance.LoggerService;
using Attendance.Core.Common;

namespace Attendance.Service
{
    public interface INoticeService
    {

        bool CreateNotice(Notice notice);
        bool UpdateNotice(Notice notice);
        bool DeleteNotice(long id);
        Notice GetNotice(long id);
        long GetLastId();

        IEnumerable<Notice> GetAllNotice();
        void SaveRecord();
        bool CheckIsExist(Notice notice);
    }
    public class NoticeService : INoticeService
    {
        public NoticeService()
        {

        }
        private readonly INoticeRepository noticeRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(NoticeService));

        public NoticeService(INoticeRepository noticeRepository, IUnitOfWork unitOfWork)
        {
            this.noticeRepository = noticeRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(Notice notice)
        {
            return noticeRepository.Get(chk => chk.Title == notice.Title) == null ? false : true;
        }

        public bool CreateNotice(Notice notice)
        {
            bool isSuccess = true;
            try
            {
                noticeRepository.Add(notice);
                this.SaveRecord();
                ServiceUtil<Notice>.WriteActionLog(notice.Id, ENUMOperation.CREATE, notice);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating Notice", ex);
            }
            return isSuccess;
        }

        public bool UpdateNotice(Notice notice)
        {
            bool isSuccess = true;
            try
            {
                noticeRepository.Update(notice);
                this.SaveRecord();
                ServiceUtil<Notice>.WriteActionLog(notice.Id, ENUMOperation.UPDATE, notice);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating Notice", ex);
            }
            return isSuccess;
        }

        public bool DeleteNotice(long id)
        {
            bool isSuccess = true;
            var notice = noticeRepository.GetById(id);
            try
            {
                noticeRepository.Delete(notice);
                SaveRecord();
                ServiceUtil<Notice>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting Notice", ex);
            }
            return isSuccess;
        }

        public Notice GetNotice(long id)
        {
            return noticeRepository.GetById(id);
        }

        public long GetLastId()
        {
            var lastId = noticeRepository.GetAll().Select(x=>x.Id).Max();
            //var lastObj = noticeRepository.GetAll().OrderByDescending(x => x.Id).FirstOrDefault();
            //if (lastObj != null)
            //{
            //    return lastObj.Id;
            //}
            //return 0;

            return lastId;
        }
        
        
        public IEnumerable<Notice> GetAllNotice()
        {
            return noticeRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
