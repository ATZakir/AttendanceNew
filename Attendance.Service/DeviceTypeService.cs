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
    public interface IDeviceTypeService
    {

        bool CreateDeviceType(DeviceType deviceType);
        bool UpdateDeviceType(DeviceType deviceType);
        bool DeleteDeviceType(int id);
        DeviceType GetDeviceType(int id);
        
        IEnumerable<DeviceType> GetAllDeviceType();
        void SaveRecord();
        bool CheckIsExist(DeviceType deviceType);
    }
    public class DeviceTypeService : IDeviceTypeService
    {
        public DeviceTypeService()
        {

        }
        private readonly IDeviceTypeRepository deviceTypeRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(DeviceTypeService));

        public DeviceTypeService(IDeviceTypeRepository deviceTypeRepository, IUnitOfWork unitOfWork)
        {
            this.deviceTypeRepository = deviceTypeRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(DeviceType deviceType)
        {
            return deviceTypeRepository.Get(chk => chk.Name == deviceType.Name) == null ? false : true;
        }

        public bool CreateDeviceType(DeviceType deviceType)
        {
            bool isSuccess = true;
            try
            {
                deviceTypeRepository.Add(deviceType);
                this.SaveRecord();
                ServiceUtil<DeviceType>.WriteActionLog(deviceType.Id, ENUMOperation.CREATE, deviceType);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating DeviceType", ex);
            }
            return isSuccess;
        }

        public bool UpdateDeviceType(DeviceType deviceType)
        {
            bool isSuccess = true;
            try
            {
                deviceTypeRepository.Update(deviceType);
                this.SaveRecord();
                ServiceUtil<DeviceType>.WriteActionLog(deviceType.Id, ENUMOperation.UPDATE, deviceType);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating DeviceType", ex);
            }
            return isSuccess;
        }

        public bool DeleteDeviceType(int id)
        {
            bool isSuccess = true;
            var deviceType = deviceTypeRepository.GetById(id);
            try
            {
                deviceTypeRepository.Delete(deviceType);
                SaveRecord();
                ServiceUtil<DeviceType>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting DeviceType", ex);
            }
            return isSuccess;
        }

        public DeviceType GetDeviceType(int id)
        {
            return deviceTypeRepository.GetById(id);
        }
        
        
        public IEnumerable<DeviceType> GetAllDeviceType()
        {
            return deviceTypeRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
