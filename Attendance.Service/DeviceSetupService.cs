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
    public interface IDeviceSetupService
    {

        bool CreateDeviceSetup(DeviceSetup deviceSetup);
        bool UpdateDeviceSetup(DeviceSetup deviceSetup);
        bool DeleteDeviceSetup(int id);
        DeviceSetup GetDeviceSetup(int id);
        
        IEnumerable<DeviceSetup> GetAllDeviceSetup();
        void SaveRecord();
        bool CheckIsExist(DeviceSetup deviceSetup);
    }
    public class DeviceSetupService : IDeviceSetupService
    {
        public DeviceSetupService()
        {

        }
        private readonly IDeviceSetupRepository deviceSetupRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(DeviceSetupService));

        public DeviceSetupService(IDeviceSetupRepository deviceSetupRepository, IUnitOfWork unitOfWork)
        {
            this.deviceSetupRepository = deviceSetupRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(DeviceSetup deviceSetup)
        {
            return deviceSetupRepository.Get(chk => chk.Name == deviceSetup.Name) == null ? false : true;
        }

        public bool CreateDeviceSetup(DeviceSetup deviceSetup)
        {
            bool isSuccess = true;
            try
            {
                deviceSetupRepository.Add(deviceSetup);
                this.SaveRecord();
                ServiceUtil<DeviceSetup>.WriteActionLog(deviceSetup.Id, ENUMOperation.CREATE, deviceSetup);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating DeviceSetup", ex);
            }
            return isSuccess;
        }

        public bool UpdateDeviceSetup(DeviceSetup deviceSetup)
        {
            bool isSuccess = true;
            try
            {
                deviceSetupRepository.Update(deviceSetup);
                this.SaveRecord();
                ServiceUtil<DeviceSetup>.WriteActionLog(deviceSetup.Id, ENUMOperation.UPDATE, deviceSetup);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating DeviceSetup", ex);
            }
            return isSuccess;
        }

        public bool DeleteDeviceSetup(int id)
        {
            bool isSuccess = true;
            var deviceSetup = deviceSetupRepository.GetById(id);
            try
            {
                deviceSetupRepository.Delete(deviceSetup);
                SaveRecord();
                ServiceUtil<DeviceSetup>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting DeviceSetup", ex);
            }
            return isSuccess;
        }

        public DeviceSetup GetDeviceSetup(int id)
        {
            return deviceSetupRepository.GetById(id);
        }
        
        
        public IEnumerable<DeviceSetup> GetAllDeviceSetup()
        {
            return deviceSetupRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
