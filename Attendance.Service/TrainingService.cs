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
    public interface ITrainingService
    {

        bool CreateTraining(Training training);
        bool UpdateTraining(Training training);
        bool DeleteTraining(int id);
        Training GetTraining(int id);
        long GetLastId();
        IEnumerable<Training> GetAllTraining();
        void SaveRecord();
        bool CheckIsExist(Training training);
    }
    public class TrainingService : ITrainingService
    {
        public TrainingService()
        {

        }
        private readonly ITrainingRepository trainingRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(TrainingService));

        public TrainingService(ITrainingRepository trainingRepository, IUnitOfWork unitOfWork)
        {
            this.trainingRepository = trainingRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(Training training)
        {
            return trainingRepository.Get(chk => chk.EmployeeId == training.EmployeeId && chk.StartDate==training.StartDate & chk.EndDate==training.EndDate) == null ? false : true;
        }

        public bool CreateTraining(Training training)
        {
            bool isSuccess = true;
            try
            {
                trainingRepository.Add(training);
                this.SaveRecord();
                ServiceUtil<Training>.WriteActionLog(training.Id, ENUMOperation.CREATE, training);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating Training", ex);
            }
            return isSuccess;
        }

        public bool UpdateTraining(Training training)
        {
            bool isSuccess = true;
            try
            {
                trainingRepository.Update(training);
                this.SaveRecord();
                ServiceUtil<Training>.WriteActionLog(training.Id, ENUMOperation.UPDATE, training);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating Training", ex);
            }
            return isSuccess;
        }

        public bool DeleteTraining(int id)
        {
            bool isSuccess = true;
            var training = trainingRepository.GetById(id);
            try
            {
                trainingRepository.Delete(training);
                SaveRecord();
                ServiceUtil<Training>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting Training", ex);
            }
            return isSuccess;
        }

        public Training GetTraining(int id)
        {
            return trainingRepository.GetById(id);
        }
        
        
        public IEnumerable<Training> GetAllTraining()
        {
            return trainingRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
        public long GetLastId()
        {
            var lastId = trainingRepository.GetAll().Select(x => x.Id).Max();
            return lastId;
        }
    }
}
