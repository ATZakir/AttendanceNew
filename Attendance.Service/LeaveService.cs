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
    public interface ILeaveService
    {

        bool CreateLeave(Leave leave);
        bool UpdateLeave(Leave leave);
        bool DeleteLeave(string id);
        Leave GetLeave(string id);
        
        IEnumerable<Leave> GetAllLeave();
        void SaveRecord();
        bool CheckIsExist(Leave leave);
    }
    public class LeaveService : ILeaveService
    {
        public LeaveService()
        {

        }
        private readonly ILeaveRepository leaveRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(LeaveService));

        public LeaveService(ILeaveRepository leaveRepository, IUnitOfWork unitOfWork)
        {
            this.leaveRepository = leaveRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(Leave leave)
        {
            return leaveRepository.Get(chk => chk.EmployeeId == leave.EmployeeId && chk.LeaveTypeId==leave.LeaveTypeId && chk.StartDate==leave.StartDate && chk.EndDate==leave.EndDate) == null ? false : true;
        }

        public bool CreateLeave(Leave leave)
        {
            bool isSuccess = true;
            try
            {
                leaveRepository.Add(leave);
                this.SaveRecord();
                ServiceUtil<Leave>.WriteActionLog(leave.Id, ENUMOperation.CREATE, leave);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating Leave", ex);
            }
            return isSuccess;
        }

        public bool UpdateLeave(Leave leave)
        {
            bool isSuccess = true;
            try
            {
                leaveRepository.Update(leave);
                this.SaveRecord();
                ServiceUtil<Leave>.WriteActionLog(leave.Id, ENUMOperation.UPDATE, leave);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating Leave", ex);
            }
            return isSuccess;
        }

        public bool DeleteLeave(string id)
        {
            bool isSuccess = true;
            var leave = leaveRepository.GetById(id);
            try
            {
                leaveRepository.Delete(leave);
                SaveRecord();
                ServiceUtil<Leave>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting Leave", ex);
            }
            return isSuccess;
        }

        public Leave GetLeave(string id)
        {
            return leaveRepository.GetById(id);
        }
        
        
        public IEnumerable<Leave> GetAllLeave()
        {
            return leaveRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
