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
    public interface IAttendanceRemarkService
    {

        bool CreateAttendanceRemark(AttendanceRemark attendanceRemark);
        bool UpdateAttendanceRemark(AttendanceRemark attendanceRemark);
        bool DeleteAttendanceRemark(int id);
        AttendanceRemark GetAttendanceRemark(int id);
        
        IEnumerable<AttendanceRemark> GetAllAttendanceRemark();
        void SaveRecord();
        bool CheckIsExist(AttendanceRemark attendanceRemark);
    }
    public class AttendanceRemarkService : IAttendanceRemarkService
    {
        public AttendanceRemarkService()
        {

        }
        private readonly IAttendanceRemarkRepository attendanceRemarkRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(AttendanceRemarkService));

        public AttendanceRemarkService(IAttendanceRemarkRepository attendanceRemarkRepository, IUnitOfWork unitOfWork)
        {
            this.attendanceRemarkRepository = attendanceRemarkRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(AttendanceRemark attendanceRemark)
        {
            return attendanceRemarkRepository.Get(chk => chk.Name == attendanceRemark.Name) == null ? false : true;
        }

        public bool CreateAttendanceRemark(AttendanceRemark attendanceRemark)
        {
            bool isSuccess = true;
            try
            {
                attendanceRemarkRepository.Add(attendanceRemark);
                this.SaveRecord();
                ServiceUtil<AttendanceRemark>.WriteActionLog(attendanceRemark.Id, ENUMOperation.CREATE, attendanceRemark);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating AttendanceRemark", ex);
            }
            return isSuccess;
        }

        public bool UpdateAttendanceRemark(AttendanceRemark attendanceRemark)
        {
            bool isSuccess = true;
            try
            {
                attendanceRemarkRepository.Update(attendanceRemark);
                this.SaveRecord();
                ServiceUtil<AttendanceRemark>.WriteActionLog(attendanceRemark.Id, ENUMOperation.UPDATE, attendanceRemark);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating AttendanceRemark", ex);
            }
            return isSuccess;
        }

        public bool DeleteAttendanceRemark(int id)
        {
            bool isSuccess = true;
            var attendanceRemark = attendanceRemarkRepository.GetById(id);
            try
            {
                attendanceRemarkRepository.Delete(attendanceRemark);
                SaveRecord();
                ServiceUtil<AttendanceRemark>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting AttendanceRemark", ex);
            }
            return isSuccess;
        }

        public AttendanceRemark GetAttendanceRemark(int id)
        {
            return attendanceRemarkRepository.GetById(id);
        }
        
        
        public IEnumerable<AttendanceRemark> GetAllAttendanceRemark()
        {
            return attendanceRemarkRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
