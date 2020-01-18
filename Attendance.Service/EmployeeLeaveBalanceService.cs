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
    public interface IEmployeeLeaveBalanceService
    {

        bool CreateEmployeeLeaveBalance(EmployeeLeaveBalance employeeLeaveBalance);
        bool UpdateEmployeeLeaveBalance(EmployeeLeaveBalance employeeLeaveBalance);
        bool DeleteEmployeeLeaveBalance(int id);
        EmployeeLeaveBalance GetEmployeeLeaveBalance(int id);
        
        IEnumerable<EmployeeLeaveBalance> GetAllEmployeeLeaveBalance();
        void SaveRecord();
        bool CheckIsExist(EmployeeLeaveBalance employeeLeaveBalance);
        IEnumerable<EmployeeLeaveBalance> GetEmployeeLeaveBalanceByEmpIdAndYear(int empId, int year);
    }
    public class EmployeeLeaveBalanceService : IEmployeeLeaveBalanceService
    {
        public EmployeeLeaveBalanceService()
        {

        }
        private readonly IEmployeeLeaveBalanceRepository employeeLeaveBalanceRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(EmployeeLeaveBalanceService));

        public EmployeeLeaveBalanceService(IEmployeeLeaveBalanceRepository employeeLeaveBalanceRepository, IUnitOfWork unitOfWork)
        {
            this.employeeLeaveBalanceRepository = employeeLeaveBalanceRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(EmployeeLeaveBalance employeeLeaveBalance)
        {
            return employeeLeaveBalanceRepository.Get(chk => chk.Year == employeeLeaveBalance.Year) != null;
        }

        public IEnumerable<EmployeeLeaveBalance> GetEmployeeLeaveBalanceByEmpIdAndYear(int empId, int year)
        {
            return  employeeLeaveBalanceRepository.GetMany(x => x.EmployeeId == empId && x.Year == year);
        }

        public bool CreateEmployeeLeaveBalance(EmployeeLeaveBalance employeeLeaveBalance)
        {
            bool isSuccess = true;
            try
            {
                employeeLeaveBalanceRepository.Add(employeeLeaveBalance);
                this.SaveRecord();
                ServiceUtil<EmployeeLeaveBalance>.WriteActionLog(employeeLeaveBalance.Id, ENUMOperation.CREATE, employeeLeaveBalance);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating EmployeeLeaveBalance", ex);
            }
            return isSuccess;
        }

        public bool UpdateEmployeeLeaveBalance(EmployeeLeaveBalance employeeLeaveBalance)
        {
            bool isSuccess = true;
            try
            {
                employeeLeaveBalanceRepository.Update(employeeLeaveBalance);
                this.SaveRecord();
                ServiceUtil<EmployeeLeaveBalance>.WriteActionLog(employeeLeaveBalance.Id, ENUMOperation.UPDATE, employeeLeaveBalance);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating EmployeeLeaveBalance", ex);
            }
            return isSuccess;
        }

        public bool DeleteEmployeeLeaveBalance(int id)
        {
            bool isSuccess = true;
            var employeeLeaveBalance = employeeLeaveBalanceRepository.GetById(id);
            try
            {
                employeeLeaveBalanceRepository.Delete(employeeLeaveBalance);
                SaveRecord();
                ServiceUtil<EmployeeLeaveBalance>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting EmployeeLeaveBalance", ex);
            }
            return isSuccess;
        }

        public EmployeeLeaveBalance GetEmployeeLeaveBalance(int id)
        {
            return employeeLeaveBalanceRepository.GetById(id);
        }
        
        
        public IEnumerable<EmployeeLeaveBalance> GetAllEmployeeLeaveBalance()
        {
            return employeeLeaveBalanceRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
