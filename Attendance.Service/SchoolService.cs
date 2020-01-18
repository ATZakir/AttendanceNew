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
    public interface ISchoolService
    {

        bool CreateSchool(School school);
        bool UpdateSchool(School school);
        bool DeleteSchool(int id);
        School GetSchool(int id);
        
        IEnumerable<School> GetAllSchool();
        void SaveRecord();
        bool CheckIsExist(School school);
        bool CheckIsExist(int schoolId);
    }
    public class SchoolService : ISchoolService
    {
        public SchoolService()
        {

        }
        private readonly ISchoolRepository schoolRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(SchoolService));

        public SchoolService(ISchoolRepository schoolRepository, IUnitOfWork unitOfWork)
        {
            this.schoolRepository = schoolRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(School school)
        {
            return schoolRepository.Get(chk => chk.Name == school.Name) == null ? false : true;
        }

        public bool CheckIsExist(int schoolId)
        {
            return schoolRepository.GetById(schoolId) != null;
        }

        public bool CreateSchool(School school)
        {
            bool isSuccess = true;
            try
            {
                schoolRepository.Add(school);
                this.SaveRecord();
                ServiceUtil<School>.WriteActionLog(school.Id, ENUMOperation.CREATE, school);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating School", ex);
            }
            return isSuccess;
        }

        public bool UpdateSchool(School school)
        {
            bool isSuccess = true;
            try
            {
                schoolRepository.Update(school);
                this.SaveRecord();
                ServiceUtil<School>.WriteActionLog(school.Id, ENUMOperation.UPDATE, school);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating School", ex);
            }
            return isSuccess;
        }

        public bool DeleteSchool(int id)
        {
            bool isSuccess = true;
            var school = schoolRepository.GetById(id);
            try
            {
                schoolRepository.Delete(school);
                SaveRecord();
                ServiceUtil<School>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting School", ex);
            }
            return isSuccess;
        }

        public School GetSchool(int id)
        {
            return schoolRepository.GetById(id);
        }
        
        
        public IEnumerable<School> GetAllSchool()
        {
            return schoolRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
