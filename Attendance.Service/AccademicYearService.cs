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
    public interface IAccademicYearService
    {

        bool CreateAccademicYear(AccademicYear accademicYear);
        bool UpdateAccademicYear(AccademicYear accademicYear);
        bool DeleteAccademicYear(int id);
        AccademicYear GetAccademicYear(int id);
        
        IEnumerable<AccademicYear> GetAllAccademicYear();
        void SaveRecord();
        bool CheckIsExist(AccademicYear accademicYear);
    }
    public class AccademicYearService : IAccademicYearService
    {
        public AccademicYearService()
        {

        }
        private readonly IAccademicYearRepository accademicYearRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(AccademicYearService));

        public AccademicYearService(IAccademicYearRepository accademicYearRepository, IUnitOfWork unitOfWork)
        {
            this.accademicYearRepository = accademicYearRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(AccademicYear accademicYear)
        {
            return accademicYearRepository.Get(chk => chk.AccademicYear1
            == accademicYear.AccademicYear1) == null ? false : true;
        }

        public bool CreateAccademicYear(AccademicYear accademicYear)
        {
            bool isSuccess = true;
            try
            {
                accademicYearRepository.Add(accademicYear);
                this.SaveRecord();
                ServiceUtil<AccademicYear>.WriteActionLog(accademicYear.AccademicYear1, ENUMOperation.CREATE, accademicYear);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating AccademicYear", ex);
            }
            return isSuccess;
        }

        public bool UpdateAccademicYear(AccademicYear accademicYear)
        {
            bool isSuccess = true;
            try
            {
                accademicYearRepository.Update(accademicYear);
                this.SaveRecord();
                ServiceUtil<AccademicYear>.WriteActionLog(accademicYear.AccademicYear1, ENUMOperation.UPDATE, accademicYear);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating AccademicYear", ex);
            }
            return isSuccess;
        }

        public bool DeleteAccademicYear(int id)
        {
            bool isSuccess = true;
            var accademicYear = accademicYearRepository.GetById(id);
            try
            {
                accademicYearRepository.Delete(accademicYear);
                SaveRecord();
                ServiceUtil<AccademicYear>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting AccademicYear", ex);
            }
            return isSuccess;
        }

        

        public AccademicYear GetAccademicYear(int id)
        {
            return accademicYearRepository.GetById(id);
        }
        
        
        public IEnumerable<AccademicYear> GetAllAccademicYear()
        {
            return accademicYearRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
