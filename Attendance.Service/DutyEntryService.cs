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
    public interface IDutyEntryService
    {

        bool CreateDutyEntry(DutyEntry dutyEntry);
        bool UpdateDutyEntry(DutyEntry dutyEntry);
        bool DeleteDutyEntry(Guid id);
        DutyEntry GetDutyEntry(Guid id);
        
        IEnumerable<DutyEntry> GetAllDutyEntry();
        void SaveRecord();
        bool CheckIsExist(DutyEntry dutyEntry);
    }
    public class DutyEntryService : IDutyEntryService
    {
        public DutyEntryService()
        {

        }
        private readonly IDutyEntryRepository dutyEntryRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(DutyEntryService));

        public DutyEntryService(IDutyEntryRepository dutyEntryRepository, IUnitOfWork unitOfWork)
        {
            this.dutyEntryRepository = dutyEntryRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(DutyEntry dutyEntry)
        {
            return dutyEntryRepository.Get(chk => chk.EmployeeId == dutyEntry.EmployeeId && chk.StartDate==dutyEntry.StartDate && chk.EndDate==dutyEntry.EndDate && chk.ReasonId==dutyEntry.ReasonId && chk.InTime==dutyEntry.InTime && chk.OutTime==dutyEntry.OutTime) == null ? false : true;
        }

        public bool CreateDutyEntry(DutyEntry dutyEntry)
        {
            bool isSuccess = true;
            try
            {
                dutyEntryRepository.Add(dutyEntry);
                this.SaveRecord();
                ServiceUtil<DutyEntry>.WriteActionLog(dutyEntry.Id, ENUMOperation.CREATE, dutyEntry);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating DutyEntry", ex);
            }
            return isSuccess;
        }

        public bool UpdateDutyEntry(DutyEntry dutyEntry)
        {
            bool isSuccess = true;
            try
            {
                dutyEntryRepository.Update(dutyEntry);
                this.SaveRecord();
                ServiceUtil<DutyEntry>.WriteActionLog(dutyEntry.Id, ENUMOperation.UPDATE, dutyEntry);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating DutyEntry", ex);
            }
            return isSuccess;
        }

        public bool DeleteDutyEntry(Guid id)
        {
            bool isSuccess = true;
            var dutyEntry = dutyEntryRepository.GetById(id);
            try
            {
                dutyEntryRepository.Delete(dutyEntry);
                SaveRecord();
                ServiceUtil<DutyEntry>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting DutyEntry", ex);
            }
            return isSuccess;
        }

        public DutyEntry GetDutyEntry(Guid id)
        {
            return dutyEntryRepository.GetById(id);
        }
        
        
        public IEnumerable<DutyEntry> GetAllDutyEntry()
        {
            return dutyEntryRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
