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
    public interface IEmploymentHistoryService
    {

        bool CreateEmploymentHistory(EmploymentHistory employmentHistory);
        bool UpdateEmploymentHistory(EmploymentHistory employmentHistory);
        bool DeleteEmploymentHistory(int id);
        EmploymentHistory GetEmploymentHistory(int id);
        int GetEmploymentHistoryBySchool(int id);
        
        IEnumerable<EmploymentHistory> GetAllEmploymentHistory();
        void SaveRecord();
        bool CheckIsExist(EmploymentHistory employmentHistory);
    }
    public class EmploymentHistoryService : IEmploymentHistoryService
    {
        public EmploymentHistoryService()
        {

        }
        private readonly IEmploymentHistoryRepository employmentHistoryRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(EmploymentHistoryService));

        public EmploymentHistoryService(IEmploymentHistoryRepository employmentHistoryRepository, IUnitOfWork unitOfWork)
        {
            this.employmentHistoryRepository = employmentHistoryRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(EmploymentHistory employmentHistory)
        {
            return employmentHistoryRepository.Get(chk => chk.EmployeeId == employmentHistory.EmployeeId && chk.DateFrom==employmentHistory.DateFrom && chk.DateTo==employmentHistory.DateTo && chk.DesignationId==employmentHistory.DesignationId) == null ? false : true;
        }

        public bool CreateEmploymentHistory(EmploymentHistory employmentHistory)
        {
            bool isSuccess = true;
            try
            {
                employmentHistoryRepository.Add(employmentHistory);
                this.SaveRecord();
                ServiceUtil<EmploymentHistory>.WriteActionLog(employmentHistory.Id, ENUMOperation.CREATE, employmentHistory);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating EmploymentHistory", ex);
            }
            return isSuccess;
        }

        public bool UpdateEmploymentHistory(EmploymentHistory employmentHistory)
        {
            bool isSuccess = true;
            try
            {
                employmentHistoryRepository.Update(employmentHistory);
                this.SaveRecord();
                ServiceUtil<EmploymentHistory>.WriteActionLog(employmentHistory.Id, ENUMOperation.UPDATE, employmentHistory);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating EmploymentHistory", ex);
            }
            return isSuccess;
        }

        public bool DeleteEmploymentHistory(int id)
        {
            bool isSuccess = true;
            var employmentHistory = employmentHistoryRepository.GetById(id);
            try
            {
                employmentHistoryRepository.Delete(employmentHistory);
                SaveRecord();
                ServiceUtil<EmploymentHistory>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting EmploymentHistory", ex);
            }
            return isSuccess;
        }

        public EmploymentHistory GetEmploymentHistory(int id)
        {
            return employmentHistoryRepository.GetById(id);
        }
        public int GetEmploymentHistoryBySchool(int id)
        {
            var schoolId = employmentHistoryRepository.Get(x => x.SchoolId == id).SchoolId;
            return schoolId ?? 0;
        }


        public IEnumerable<EmploymentHistory> GetAllEmploymentHistory()
        {
            return employmentHistoryRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
