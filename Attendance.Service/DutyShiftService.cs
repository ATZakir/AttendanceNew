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
    public interface IDutyShiftService
    {

        bool CreateDutyShift(DutyShift dutyShift);
        bool UpdateDutyShift(DutyShift dutyShift);
        bool DeleteDutyShift(int id);
        DutyShift GetDutyShift(int id);
        int GetDutyShiftByMasterId(int id);
        
        IEnumerable<DutyShift> GetAllDutyShift();
        void SaveRecord();
        bool CheckIsExist(DutyShift dutyShift);
    }
    public class DutyShiftService : IDutyShiftService
    {
        public DutyShiftService()
        {

        }
        private readonly IDutyShiftRepository dutyShiftRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(DutyShiftService));

        public DutyShiftService(IDutyShiftRepository dutyShiftRepository, IUnitOfWork unitOfWork)
        {
            this.dutyShiftRepository = dutyShiftRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(DutyShift dutyShift)
        {
            return dutyShiftRepository.Get(chk => chk.SchoolId == dutyShift.SchoolId && chk.DutyShiftMasterId==dutyShift.DutyShiftMasterId) == null ? false : true;
        }

        public bool CreateDutyShift(DutyShift dutyShift)
        {
            bool isSuccess = true;
            try
            {
                dutyShiftRepository.Add(dutyShift);
                this.SaveRecord();
                ServiceUtil<DutyShift>.WriteActionLog(dutyShift.Id, ENUMOperation.CREATE, dutyShift);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating DutyShift", ex);
            }
            return isSuccess;
        }

        public bool UpdateDutyShift(DutyShift dutyShift)
        {
            bool isSuccess = true;
            try
            {
                dutyShiftRepository.Update(dutyShift);
                this.SaveRecord();
                ServiceUtil<DutyShift>.WriteActionLog(dutyShift.Id, ENUMOperation.UPDATE, dutyShift);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating DutyShift", ex);
            }
            return isSuccess;
        }

        public bool DeleteDutyShift(int id)
        {
            bool isSuccess = true;
            var dutyShift = dutyShiftRepository.GetById(id);
            try
            {
                dutyShiftRepository.Delete(dutyShift);
                SaveRecord();
                ServiceUtil<DutyShift>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting DutyShift", ex);
            }
            return isSuccess;
        }

        public DutyShift GetDutyShift(int id)
        {
            return dutyShiftRepository.GetById(id);
        }
        public int GetDutyShiftByMasterId(int id)
        {
            var dutyShiftId = dutyShiftRepository.Get(x => x.DutyShiftMasterId == id).Id;
            return dutyShiftId;
        }
        
        
        public IEnumerable<DutyShift> GetAllDutyShift()
        {
            return dutyShiftRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
