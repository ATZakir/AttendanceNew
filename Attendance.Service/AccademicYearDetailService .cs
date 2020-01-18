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
    public interface IAccademicYearDetailService
    {

        bool CreateAccademicYearDetail(AccademicYearDetail accademicYearDetail);
        bool UpdateAccademicYearDetail(AccademicYearDetail accademicYearDetail);
        bool DeleteAccademicYearDetail(int id, DateTime date);
        AccademicYearDetail GetAccademicYearDetail(int id);
        AccademicYearDetail GetAccademicYearDetail(int id, DateTime date);
        IEnumerable<AccademicYearDetail> GetAllAccademicYearDetail();
        void SaveRecord();
        bool CheckIsExist(AccademicYearDetail accademicYearDetail);
    }
    public class AccademicYearDetailService : IAccademicYearDetailService
    {
        public AccademicYearDetailService()
        {

        }
        private readonly IAccademicYearDetailRepository accademicYearDetailRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(AccademicYearDetailService));

        public AccademicYearDetailService(IAccademicYearDetailRepository accademicYearDetailRepository, IUnitOfWork unitOfWork)
        {
            this.accademicYearDetailRepository = accademicYearDetailRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(AccademicYearDetail accademicYearDetail)
        {
            return accademicYearDetailRepository.Get(chk => chk.DayDate
            == accademicYearDetail.DayDate && chk.AccademicYear == accademicYearDetail.AccademicYear) == null ? false : true;
        }

        public bool CreateAccademicYearDetail(AccademicYearDetail accademicYearDetail)
        {
            bool isSuccess = true;
            try
            {
                accademicYearDetailRepository.Add(accademicYearDetail);
                this.SaveRecord();
                ServiceUtil<AccademicYearDetail>.WriteActionLog(accademicYearDetail.AccademicYear, ENUMOperation.CREATE, accademicYearDetail);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating AccademicYearDetail", ex);
            }
            return isSuccess;
        }

        public bool UpdateAccademicYearDetail(AccademicYearDetail accademicYearDetail)
        {
            bool isSuccess = true;
            try
            {
                accademicYearDetailRepository.Update(accademicYearDetail);
                this.SaveRecord();
                ServiceUtil<AccademicYearDetail>.WriteActionLog(accademicYearDetail.AccademicYear, ENUMOperation.UPDATE, accademicYearDetail);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating AccademicYearDetail", ex);
            }
            return isSuccess;
        }


        public bool DeleteAccademicYearDetail(int id, DateTime date)
        {
            bool isSuccess = true;
            var accademicYear = accademicYearDetailRepository.GetById(id, date);
            try
            {
                accademicYearDetailRepository.Delete(accademicYear);
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


        public bool DeleteAccademicYearDetail(int id)
        {
            bool isSuccess = true;
            var accademicYearDetail = accademicYearDetailRepository.GetById(id);
            try
            {
                accademicYearDetailRepository.Delete(accademicYearDetail);
                SaveRecord();
                ServiceUtil<AccademicYearDetail>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting AccademicYearDetail", ex);
            }
            return isSuccess;
        }

        public AccademicYearDetail GetAccademicYearDetail(int id)
        {
            return accademicYearDetailRepository.GetById(id);
        }

        public AccademicYearDetail GetAccademicYearDetail(int id, DateTime date)
        {
            return accademicYearDetailRepository.GetById(id, date);
        }


        public IEnumerable<AccademicYearDetail> GetAllAccademicYearDetail()
        {
            return accademicYearDetailRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
