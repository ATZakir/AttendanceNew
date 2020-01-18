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
    public interface IUpazilaService
    {

        bool CreateUpazila(Upazila upazila);
        bool UpdateUpazila(Upazila upazila);
        bool DeleteUpazila(int id);
        Upazila GetUpazila(int id);
        
        IEnumerable<Upazila> GetAllUpazila();
        void SaveRecord();
        bool CheckIsExist(Upazila upazila);
    }
    public class UpazilaService : IUpazilaService
    {
        public UpazilaService()
        {

        }
        private readonly IUpazilaRepository upazilaRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(UpazilaService));

        public UpazilaService(IUpazilaRepository upazilaRepository, IUnitOfWork unitOfWork)
        {
            this.upazilaRepository = upazilaRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(Upazila upazila)
        {
            return upazilaRepository.Get(chk => chk.Name == upazila.Name && chk.DistrictId==upazila.DistrictId) == null ? false : true;
        }

        public bool CreateUpazila(Upazila upazila)
        {
            bool isSuccess = true;
            try
            {
                upazilaRepository.Add(upazila);
                this.SaveRecord();
                ServiceUtil<Upazila>.WriteActionLog(upazila.Id, ENUMOperation.CREATE, upazila);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating Upazila", ex);
            }
            return isSuccess;
        }

        public bool UpdateUpazila(Upazila upazila)
        {
            bool isSuccess = true;
            try
            {
                upazilaRepository.Update(upazila);
                this.SaveRecord();
                ServiceUtil<Upazila>.WriteActionLog(upazila.Id, ENUMOperation.UPDATE, upazila);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating Upazila", ex);
            }
            return isSuccess;
        }

        public bool DeleteUpazila(int id)
        {
            bool isSuccess = true;
            var upazila = upazilaRepository.GetById(id);
            try
            {
                upazilaRepository.Delete(upazila);
                SaveRecord();
                ServiceUtil<Upazila>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting Upazila", ex);
            }
            return isSuccess;
        }

        public Upazila GetUpazila(int id)
        {
            return upazilaRepository.GetById(id);
        }
        
        
        public IEnumerable<Upazila> GetAllUpazila()
        {
            return upazilaRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
