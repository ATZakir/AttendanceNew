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
    public interface IDutyShiftMasterService
    {

        bool CreateDutyShiftMaster(DutyShiftMaster dutyShiftMaster);
        bool UpdateDutyShiftMaster(DutyShiftMaster dutyShiftMaster);
        bool DeleteDutyShiftMaster(int id);
        DutyShiftMaster GetDutyShiftMaster(int id);
        
        IEnumerable<DutyShiftMaster> GetAllDutyShiftMaster();
        void SaveRecord();
        bool CheckIsExist(DutyShiftMaster dutyShiftMaster);
    }
    public class DutyShiftMasterService : IDutyShiftMasterService
    {
        public DutyShiftMasterService()
        {

        }
        private readonly IDutyShiftMasterRepository dutyShiftMasterRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(DutyShiftMasterService));

        public DutyShiftMasterService(IDutyShiftMasterRepository dutyShiftMasterRepository, IUnitOfWork unitOfWork)
        {
            this.dutyShiftMasterRepository = dutyShiftMasterRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(DutyShiftMaster dutyShiftMaster)
        {
            return dutyShiftMasterRepository.Get(chk => chk.Name == dutyShiftMaster.Name) == null ? false : true;
        }

        public bool CreateDutyShiftMaster(DutyShiftMaster dutyShiftMaster)
        {
            bool isSuccess = true;
            try
            {
                dutyShiftMasterRepository.Add(dutyShiftMaster);
                this.SaveRecord();
                ServiceUtil<DutyShiftMaster>.WriteActionLog(dutyShiftMaster.Id, ENUMOperation.CREATE, dutyShiftMaster);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating DutyShiftMaster", ex);
            }
            return isSuccess;
        }

        public bool UpdateDutyShiftMaster(DutyShiftMaster dutyShiftMaster)
        {
            bool isSuccess = true;
            try
            {
                dutyShiftMasterRepository.Update(dutyShiftMaster);
                this.SaveRecord();
                ServiceUtil<DutyShiftMaster>.WriteActionLog(dutyShiftMaster.Id, ENUMOperation.UPDATE, dutyShiftMaster);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating DutyShiftMaster", ex);
            }
            return isSuccess;
        }

        public bool DeleteDutyShiftMaster(int id)
        {
            bool isSuccess = true;
            var dutyShiftMaster = dutyShiftMasterRepository.GetById(id);
            try
            {
                dutyShiftMasterRepository.Delete(dutyShiftMaster);
                SaveRecord();
                ServiceUtil<DutyShiftMaster>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting DutyShiftMaster", ex);
            }
            return isSuccess;
        }

        public DutyShiftMaster GetDutyShiftMaster(int id)
        {
            return dutyShiftMasterRepository.GetById(id);
        }
        
        
        public IEnumerable<DutyShiftMaster> GetAllDutyShiftMaster()
        {
            return dutyShiftMasterRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
