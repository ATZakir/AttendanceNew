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
    public interface IDutyReasonService
    {

        bool CreateDutyReason(DutyReason dutyReason);
        bool UpdateDutyReason(DutyReason dutyReason);
        bool DeleteDutyReason(int id);
        DutyReason GetDutyReason(int id);
        
        IEnumerable<DutyReason> GetAllDutyReason();
        void SaveRecord();
        bool CheckIsExist(DutyReason dutyReason);
    }
    public class DutyReasonService : IDutyReasonService
    {
        public DutyReasonService()
        {

        }
        private readonly IDutyReasonRepository dutyReasonRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(DutyReasonService));

        public DutyReasonService(IDutyReasonRepository dutyReasonRepository, IUnitOfWork unitOfWork)
        {
            this.dutyReasonRepository = dutyReasonRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(DutyReason dutyReason)
        {
            return dutyReasonRepository.Get(chk => chk.Name == dutyReason.Name) == null ? false : true;
        }

        public bool CreateDutyReason(DutyReason dutyReason)
        {
            bool isSuccess = true;
            try
            {
                dutyReasonRepository.Add(dutyReason);
                this.SaveRecord();
                ServiceUtil<DutyReason>.WriteActionLog(dutyReason.Id, ENUMOperation.CREATE, dutyReason);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating DutyReason", ex);
            }
            return isSuccess;
        }

        public bool UpdateDutyReason(DutyReason dutyReason)
        {
            bool isSuccess = true;
            try
            {
                dutyReasonRepository.Update(dutyReason);
                this.SaveRecord();
                ServiceUtil<DutyReason>.WriteActionLog(dutyReason.Id, ENUMOperation.UPDATE, dutyReason);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating DutyReason", ex);
            }
            return isSuccess;
        }

        public bool DeleteDutyReason(int id)
        {
            bool isSuccess = true;
            var dutyReason = dutyReasonRepository.GetById(id);
            try
            {
                dutyReasonRepository.Delete(dutyReason);
                SaveRecord();
                ServiceUtil<DutyReason>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting DutyReason", ex);
            }
            return isSuccess;
        }

        public DutyReason GetDutyReason(int id)
        {
            return dutyReasonRepository.GetById(id);
        }
        
        
        public IEnumerable<DutyReason> GetAllDutyReason()
        {
            return dutyReasonRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
