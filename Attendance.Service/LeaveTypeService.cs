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
    public interface ILeaveTypeService
    {

        bool CreateLeaveType(LeaveType leaveType);
        bool UpdateLeaveType(LeaveType leaveType);
        bool DeleteLeaveType(int id);
        LeaveType GetLeaveType(int id);
        
        IEnumerable<LeaveType> GetAllLeaveType();
        void SaveRecord();
        bool CheckIsExist(LeaveType leaveType);
    }
    public class LeaveTypeService : ILeaveTypeService
    {
        public LeaveTypeService()
        {

        }
        private readonly ILeaveTypeRepository leaveTypeRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(LeaveTypeService));

        public LeaveTypeService(ILeaveTypeRepository leaveTypeRepository, IUnitOfWork unitOfWork)
        {
            this.leaveTypeRepository = leaveTypeRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(LeaveType leaveType)
        {
            return leaveTypeRepository.Get(chk => chk.Name == leaveType.Name) == null ? false : true;
        }

        public bool CreateLeaveType(LeaveType leaveType)
        {
            bool isSuccess = true;
            try
            {
                leaveTypeRepository.Add(leaveType);
                this.SaveRecord();
                ServiceUtil<LeaveType>.WriteActionLog(leaveType.Id, ENUMOperation.CREATE, leaveType);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating LeaveType", ex);
            }
            return isSuccess;
        }

        public bool UpdateLeaveType(LeaveType leaveType)
        {
            bool isSuccess = true;
            try
            {
                leaveTypeRepository.Update(leaveType);
                this.SaveRecord();
                ServiceUtil<LeaveType>.WriteActionLog(leaveType.Id, ENUMOperation.UPDATE, leaveType);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating LeaveType", ex);
            }
            return isSuccess;
        }

        public bool DeleteLeaveType(int id)
        {
            bool isSuccess = true;
            var leaveType = leaveTypeRepository.GetById(id);
            try
            {
                leaveTypeRepository.Delete(leaveType);
                SaveRecord();
                ServiceUtil<LeaveType>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting LeaveType", ex);
            }
            return isSuccess;
        }

        public LeaveType GetLeaveType(int id)
        {
            return leaveTypeRepository.GetById(id);
        }
        
        
        public IEnumerable<LeaveType> GetAllLeaveType()
        {
            return leaveTypeRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
