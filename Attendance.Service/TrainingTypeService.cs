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
    public interface ITrainingTypeService
    {

        bool CreateTrainingType(TrainingType trainingType);
        bool UpdateTrainingType(TrainingType trainingType);
        bool DeleteTrainingType(int id);
        TrainingType GetTrainingType(int id);
        
        IEnumerable<TrainingType> GetAllTrainingType();
        void SaveRecord();
        bool CheckIsExist(TrainingType trainingType);
    }
    public class TrainingTypeService : ITrainingTypeService
    {
        public TrainingTypeService()
        {

        }
        private readonly ITrainingTypeRepository trainingTypeRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(TrainingTypeService));

        public TrainingTypeService(ITrainingTypeRepository trainingTypeRepository, IUnitOfWork unitOfWork)
        {
            this.trainingTypeRepository = trainingTypeRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(TrainingType trainingType)
        {
            return trainingTypeRepository.Get(chk => chk.Name == trainingType.Name) == null ? false : true;
        }

        public bool CreateTrainingType(TrainingType trainingType)
        {
            bool isSuccess = true;
            try
            {
                trainingTypeRepository.Add(trainingType);
                this.SaveRecord();
                ServiceUtil<TrainingType>.WriteActionLog(trainingType.Id, ENUMOperation.CREATE, trainingType);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating TrainingType", ex);
            }
            return isSuccess;
        }

        public bool UpdateTrainingType(TrainingType trainingType)
        {
            bool isSuccess = true;
            try
            {
                trainingTypeRepository.Update(trainingType);
                this.SaveRecord();
                ServiceUtil<TrainingType>.WriteActionLog(trainingType.Id, ENUMOperation.UPDATE, trainingType);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating TrainingType", ex);
            }
            return isSuccess;
        }

        public bool DeleteTrainingType(int id)
        {
            bool isSuccess = true;
            var trainingType = trainingTypeRepository.GetById(id);
            try
            {
                trainingTypeRepository.Delete(trainingType);
                SaveRecord();
                ServiceUtil<TrainingType>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting TrainingType", ex);
            }
            return isSuccess;
        }

        public TrainingType GetTrainingType(int id)
        {
            return trainingTypeRepository.GetById(id);
        }
        
        
        public IEnumerable<TrainingType> GetAllTrainingType()
        {
            return trainingTypeRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
