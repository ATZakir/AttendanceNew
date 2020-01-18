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
    public interface IEmployeeEducationService
    {

        bool CreateEmployeeEducation(EmployeeEducation employeeEducation);
        bool UpdateEmployeeEducation(EmployeeEducation employeeEducation);
        bool DeleteEmployeeEducation(int id);
        EmployeeEducation GetEmployeeEducation(int id);
        
        IEnumerable<EmployeeEducation> GetAllEmployeeEducation();
        void SaveRecord();
        bool CheckIsExist(EmployeeEducation employeeEducation);
    }
    public class EmployeeEducationService : IEmployeeEducationService
    {
        public EmployeeEducationService()
        {

        }
        private readonly IEmployeeEducationRepository employeeEducationRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(EmployeeEducationService));

        public EmployeeEducationService(IEmployeeEducationRepository employeeEducationRepository, IUnitOfWork unitOfWork)
        {
            this.employeeEducationRepository = employeeEducationRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(EmployeeEducation employeeEducation)
        {
            return employeeEducationRepository.Get(chk => chk.EmployeeId == employeeEducation.EmployeeId && chk.LevelId==employeeEducation.LevelId) == null ? false : true;
        }

        public bool CreateEmployeeEducation(EmployeeEducation employeeEducation)
        {
            bool isSuccess = true;
            try
            {
                employeeEducationRepository.Add(employeeEducation);
                this.SaveRecord();
                ServiceUtil<EmployeeEducation>.WriteActionLog(employeeEducation.Id, ENUMOperation.CREATE, employeeEducation);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating EmployeeEducation", ex);
            }
            return isSuccess;
        }

        public bool UpdateEmployeeEducation(EmployeeEducation employeeEducation)
        {
            bool isSuccess = true;
            try
            {
                employeeEducationRepository.Update(employeeEducation);
                this.SaveRecord();
                ServiceUtil<EmployeeEducation>.WriteActionLog(employeeEducation.Id, ENUMOperation.UPDATE, employeeEducation);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating EmployeeEducation", ex);
            }
            return isSuccess;
        }

        public bool DeleteEmployeeEducation(int id)
        {
            bool isSuccess = true;
            var employeeEducation = employeeEducationRepository.GetById(id);
            try
            {
                employeeEducationRepository.Delete(employeeEducation);
                SaveRecord();
                ServiceUtil<EmployeeEducation>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting EmployeeEducation", ex);
            }
            return isSuccess;
        }

        public EmployeeEducation GetEmployeeEducation(int id)
        {
            return employeeEducationRepository.GetById(id);
        }
        
        
        public IEnumerable<EmployeeEducation> GetAllEmployeeEducation()
        {
            return employeeEducationRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
