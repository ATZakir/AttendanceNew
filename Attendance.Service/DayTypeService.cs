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
    public interface IDayTypeService
    {

        bool CreateDayType(DayType dayType);
        bool UpdateDayType(DayType dayType);
        bool DeleteDayType(int id);
        DayType GetDayType(int id);
        
        IEnumerable<DayType> GetAllDayType();
        void SaveRecord();
        bool CheckIsExist(DayType dayType);
    }
    public class DayTypeService : IDayTypeService
    {
        public DayTypeService()
        {

        }
        private readonly IDayTypeRepository dayTypeRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(DayTypeService));

        public DayTypeService(IDayTypeRepository dayTypeRepository, IUnitOfWork unitOfWork)
        {
            this.dayTypeRepository = dayTypeRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(DayType dayType)
        {
            return dayTypeRepository.Get(chk => chk.DayType1
            == dayType.DayType1) == null ? false : true;
        }

        public bool CreateDayType(DayType dayType)
        {
            bool isSuccess = true;
            try
            {
                dayTypeRepository.Add(dayType);
                this.SaveRecord();
                ServiceUtil<DayType>.WriteActionLog(dayType.Id, ENUMOperation.CREATE, dayType);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating DayType", ex);
            }
            return isSuccess;
        }

        public bool UpdateDayType(DayType dayType)
        {
            bool isSuccess = true;
            try
            {
                dayTypeRepository.Update(dayType);
                this.SaveRecord();
                ServiceUtil<DayType>.WriteActionLog(dayType.Id, ENUMOperation.UPDATE, dayType);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating DayType", ex);
            }
            return isSuccess;
        }

        public bool DeleteDayType(int id)
        {
            bool isSuccess = true;
            var dayType = dayTypeRepository.GetById(id);
            try
            {
                dayTypeRepository.Delete(dayType);
                SaveRecord();
                ServiceUtil<DayType>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting DayType", ex);
            }
            return isSuccess;
        }

        public DayType GetDayType(int id)
        {
            return dayTypeRepository.GetById(id);
        }
        
        
        public IEnumerable<DayType> GetAllDayType()
        {
            return dayTypeRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
