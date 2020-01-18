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
    public interface IEmployeeAttendanceService
    {
        bool CreateEmployeeAttendance(EmployeeAttendance employeeAttendance);
        bool UpdateEmployeeAttendance(EmployeeAttendance employeeAttendance);
        bool DeleteEmployeeAttendance(Guid id);
        EmployeeAttendance GetEmployeeAttendance(Guid id);
        
        IEnumerable<EmployeeAttendance> GetAllEmployeeAttendance();
        void SaveRecord();
        bool CheckIsExist(EmployeeAttendance employeeAttendance);
    }
    public class EmployeeAttendanceService : IEmployeeAttendanceService
    {
        public EmployeeAttendanceService()
        {

        }
        private readonly IEmployeeAttendanceRepository employeeAttendanceRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(EmployeeAttendanceService));

        public EmployeeAttendanceService(IEmployeeAttendanceRepository employeeAttendanceRepository, IUnitOfWork unitOfWork)
        {
            this.employeeAttendanceRepository = employeeAttendanceRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(EmployeeAttendance employeeAttendance)
        {
            return employeeAttendanceRepository.Get(chk => chk.EmployeeId == employeeAttendance.EmployeeId && chk.Date == employeeAttendance.Date && chk.Time == employeeAttendance.Time && chk.RemarkId == employeeAttendance.RemarkId) == null ? false : true;
        }

        public bool CreateEmployeeAttendance(EmployeeAttendance employeeAttendance)
        {
            bool isSuccess = true;
            try
            {
                employeeAttendanceRepository.Add(employeeAttendance);
                this.SaveRecord();
                ServiceUtil<EmployeeAttendance>.WriteActionLog(employeeAttendance.Id, ENUMOperation.CREATE, employeeAttendance);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating EmployeeAttendance", ex);
            }
            return isSuccess;
        }

        public bool UpdateEmployeeAttendance(EmployeeAttendance employeeAttendance)
        {
            bool isSuccess = true;
            try
            {
                employeeAttendanceRepository.Update(employeeAttendance);
                this.SaveRecord();
                ServiceUtil<EmployeeAttendance>.WriteActionLog(employeeAttendance.Id, ENUMOperation.UPDATE, employeeAttendance);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating EmployeeAttendance", ex);
            }
            return isSuccess;
        }

        public bool DeleteEmployeeAttendance(Guid id)
        {
            bool isSuccess = true;
            var employeeAttendance = employeeAttendanceRepository.GetById(id);
            try
            {
                employeeAttendanceRepository.Delete(employeeAttendance);
                SaveRecord();
                ServiceUtil<EmployeeAttendance>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting EmployeeAttendance", ex);
            }
            return isSuccess;
        }

        public EmployeeAttendance GetEmployeeAttendance(Guid id)
        {
            return employeeAttendanceRepository.GetById(id);
        }
        
        
        public IEnumerable<EmployeeAttendance> GetAllEmployeeAttendance()
        {
            return employeeAttendanceRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
