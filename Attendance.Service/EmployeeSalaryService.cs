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
    public interface IEmployeeSalaryService
    {

        bool CreateEmployeeSalary(EmployeeSalary employeeSalary);
        bool UpdateEmployeeSalary(EmployeeSalary employeeSalary);
        bool DeleteEmployeeSalary(Guid id);
        EmployeeSalary GetEmployeeSalary(Guid id);
        
        IEnumerable<EmployeeSalary> GetAllEmployeeSalary();
        void SaveRecord();
        bool CheckIsExist(EmployeeSalary employeeSalary);
    }
    public class EmployeeSalaryService : IEmployeeSalaryService
    {
        public EmployeeSalaryService()
        {

        }
        private readonly IEmployeeSalaryRepository employeeSalaryRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(EmployeeSalaryService));

        public EmployeeSalaryService(IEmployeeSalaryRepository employeeSalaryRepository, IUnitOfWork unitOfWork)
        {
            this.employeeSalaryRepository = employeeSalaryRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(EmployeeSalary employeeSalary)
        {
            return employeeSalaryRepository.Get(chk => chk.EmployeeId == employeeSalary.EmployeeId && chk.BasicSalary==employeeSalary.BasicSalary) == null ? false : true;
        }

        public bool CreateEmployeeSalary(EmployeeSalary employeeSalary)
        {
            bool isSuccess = true;
            try
            {
                employeeSalaryRepository.Add(employeeSalary);
                this.SaveRecord();
                ServiceUtil<EmployeeSalary>.WriteActionLog(employeeSalary.Id, ENUMOperation.CREATE, employeeSalary);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating EmployeeSalary", ex);
            }
            return isSuccess;
        }

        public bool UpdateEmployeeSalary(EmployeeSalary employeeSalary)
        {
            bool isSuccess = true;
            try
            {
                employeeSalaryRepository.Update(employeeSalary);
                this.SaveRecord();
                ServiceUtil<EmployeeSalary>.WriteActionLog(employeeSalary.Id, ENUMOperation.UPDATE, employeeSalary);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating EmployeeSalary", ex);
            }
            return isSuccess;
        }

        public bool DeleteEmployeeSalary(Guid id)
        {
            bool isSuccess = true;
            var employeeSalary = employeeSalaryRepository.GetById(id);
            try
            {
                employeeSalaryRepository.Delete(employeeSalary);
                SaveRecord();
                ServiceUtil<EmployeeSalary>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting EmployeeSalary", ex);
            }
            return isSuccess;
        }

        public EmployeeSalary GetEmployeeSalary(Guid id)
        {
            return employeeSalaryRepository.GetById(id);
        }
        
        
        public IEnumerable<EmployeeSalary> GetAllEmployeeSalary()
        {
            return employeeSalaryRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
