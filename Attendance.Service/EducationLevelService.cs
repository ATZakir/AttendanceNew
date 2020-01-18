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
    public interface IEducationLevelService
    {

        bool CreateEducationLevel(EducationLevel educationLevel);
        bool UpdateEducationLevel(EducationLevel educationLevel);
        bool DeleteEducationLevel(int id);
        EducationLevel GetEducationLevel(int id);
        
        IEnumerable<EducationLevel> GetAllEducationLevel();
        void SaveRecord();
        bool CheckIsExist(EducationLevel educationLevel);
    }
    public class EducationLevelService : IEducationLevelService
    {
        public EducationLevelService()
        {

        }
        private readonly IEducationLevelRepository educationLevelRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(EducationLevelService));

        public EducationLevelService(IEducationLevelRepository educationLevelRepository, IUnitOfWork unitOfWork)
        {
            this.educationLevelRepository = educationLevelRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(EducationLevel educationLevel)
        {
            return educationLevelRepository.Get(chk => chk.Name == educationLevel.Name) == null ? false : true;
        }

        public bool CreateEducationLevel(EducationLevel educationLevel)
        {
            bool isSuccess = true;
            try
            {
                educationLevelRepository.Add(educationLevel);
                this.SaveRecord();
                ServiceUtil<EducationLevel>.WriteActionLog(educationLevel.Id, ENUMOperation.CREATE, educationLevel);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating EducationLevel", ex);
            }
            return isSuccess;
        }

        public bool UpdateEducationLevel(EducationLevel educationLevel)
        {
            bool isSuccess = true;
            try
            {
                educationLevelRepository.Update(educationLevel);
                this.SaveRecord();
                ServiceUtil<EducationLevel>.WriteActionLog(educationLevel.Id, ENUMOperation.UPDATE, educationLevel);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating EducationLevel", ex);
            }
            return isSuccess;
        }

        public bool DeleteEducationLevel(int id)
        {
            bool isSuccess = true;
            var educationLevel = educationLevelRepository.GetById(id);
            try
            {
                educationLevelRepository.Delete(educationLevel);
                SaveRecord();
                ServiceUtil<EducationLevel>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting EducationLevel", ex);
            }
            return isSuccess;
        }

        public EducationLevel GetEducationLevel(int id)
        {
            return educationLevelRepository.GetById(id);
        }
        
        
        public IEnumerable<EducationLevel> GetAllEducationLevel()
        {
            return educationLevelRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
