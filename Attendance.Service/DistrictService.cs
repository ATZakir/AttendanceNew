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
    public interface IDistrictService
    {

        bool CreateDistrict(District district);
        bool UpdateDistrict(District district);
        bool DeleteDistrict(int id);
        District GetDistrict(int id);
        
        IEnumerable<District> GetAllDistrict();
        IEnumerable<District> GetAllDistrictByDivisionId(int divisionId);
        void SaveRecord();
        bool CheckIsExist(District district);
    }
    public class DistrictService : IDistrictService
    {
        public DistrictService()
        {

        }
        private readonly IDistrictRepository districtRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(DistrictService));

        public DistrictService(IDistrictRepository districtRepository, IUnitOfWork unitOfWork)
        {
            this.districtRepository = districtRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(District district)
        {
            return districtRepository.Get(chk => chk.Name == district.Name) == null ? false : true;
        }

        public bool CreateDistrict(District district)
        {
            bool isSuccess = true;
            try
            {
                districtRepository.Add(district);
                this.SaveRecord();
                ServiceUtil<District>.WriteActionLog(district.Id, ENUMOperation.CREATE, district);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating District", ex);
            }
            return isSuccess;
        }

        public bool UpdateDistrict(District district)
        {
            bool isSuccess = true;
            try
            {
                districtRepository.Update(district);
                this.SaveRecord();
                ServiceUtil<District>.WriteActionLog(district.Id, ENUMOperation.UPDATE, district);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating District", ex);
            }
            return isSuccess;
        }

        public bool DeleteDistrict(int id)
        {
            bool isSuccess = true;
            var district = districtRepository.GetById(id);
            try
            {
                districtRepository.Delete(district);
                SaveRecord();
                ServiceUtil<District>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting District", ex);
            }
            return isSuccess;
        }

        public District GetDistrict(int id)
        {
            return districtRepository.GetById(id);
        }
        
        
        public IEnumerable<District> GetAllDistrict()
        {
            return districtRepository.GetAll();
        }
        public IEnumerable<District> GetAllDistrictByDivisionId(int divisionId)
        {
            return districtRepository.GetMany(d => d.DivisionId == divisionId && d.IsActive == true);
        }

        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
