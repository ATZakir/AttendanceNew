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
    public interface IAdminEmploymentHistoryService
    {

        bool CreateAdminEmploymentHistory(AdminEmploymentHistory adminEmploymentHistory);
        bool UpdateAdminEmploymentHistory(AdminEmploymentHistory adminEmploymentHistory);
        bool DeleteAdminEmploymentHistory(int id);
        AdminEmploymentHistory GetAdminEmploymentHistory(int id);
        
        IEnumerable<AdminEmploymentHistory> GetAllAdminEmploymentHistory();
        void SaveRecord();
        bool CheckIsExist(AdminEmploymentHistory adminEmploymentHistory);
    }
    public class AdminEmploymentHistoryService : IAdminEmploymentHistoryService
    {
        public AdminEmploymentHistoryService()
        {

        }
        private readonly IAdminEmploymentHistoryRepository adminEmploymentHistoryRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(AdminEmploymentHistoryService));

        public AdminEmploymentHistoryService(IAdminEmploymentHistoryRepository adminEmploymentHistoryRepository, IUnitOfWork unitOfWork)
        {
            this.adminEmploymentHistoryRepository = adminEmploymentHistoryRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(AdminEmploymentHistory adminEmploymentHistory)
        {
            return adminEmploymentHistoryRepository.Get(chk => chk.EmployeeId == adminEmploymentHistory.EmployeeId && chk.DateFrom==adminEmploymentHistory.DateFrom && chk.DateTo==adminEmploymentHistory.DateTo && chk.DesignationId==adminEmploymentHistory.DesignationId) == null ? false : true;
        }

        public bool CreateAdminEmploymentHistory(AdminEmploymentHistory adminEmploymentHistory)
        {
            bool isSuccess = true;
            try
            {
                adminEmploymentHistoryRepository.Add(adminEmploymentHistory);
                this.SaveRecord();
                ServiceUtil<AdminEmploymentHistory>.WriteActionLog(adminEmploymentHistory.Id, ENUMOperation.CREATE, adminEmploymentHistory);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating AdminEmploymentHistory", ex);
            }
            return isSuccess;
        }

        public bool UpdateAdminEmploymentHistory(AdminEmploymentHistory adminEmploymentHistory)
        {
            bool isSuccess = true;
            try
            {
                adminEmploymentHistoryRepository.Update(adminEmploymentHistory);
                this.SaveRecord();
                ServiceUtil<AdminEmploymentHistory>.WriteActionLog(adminEmploymentHistory.Id, ENUMOperation.UPDATE, adminEmploymentHistory);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating AdminEmploymentHistory", ex);
            }
            return isSuccess;
        }

        public bool DeleteAdminEmploymentHistory(int id)
        {
            bool isSuccess = true;
            var adminEmploymentHistory = adminEmploymentHistoryRepository.GetById(id);
            try
            {
                adminEmploymentHistoryRepository.Delete(adminEmploymentHistory);
                SaveRecord();
                ServiceUtil<AdminEmploymentHistory>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting AdminEmploymentHistory", ex);
            }
            return isSuccess;
        }

        public AdminEmploymentHistory GetAdminEmploymentHistory(int id)
        {
            return adminEmploymentHistoryRepository.GetById(id);
        }
        
        
        public IEnumerable<AdminEmploymentHistory> GetAllAdminEmploymentHistory()
        {
            return adminEmploymentHistoryRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
