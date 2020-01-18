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
    public interface IInstituteService
    {

        bool CreateInstitute(Institute institute);
        bool UpdateInstitute(Institute institute);
        bool DeleteInstitute(int id);
        Institute GetInstitute(int id);
        
        IEnumerable<Institute> GetAllInstitute();
        void SaveRecord();
        bool CheckIsExist(Institute institute);
    }
    public class InstituteService : IInstituteService
    {
        public InstituteService()
        {

        }
        private readonly IInstituteRepository instituteRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(InstituteService));

        public InstituteService(IInstituteRepository instituteRepository, IUnitOfWork unitOfWork)
        {
            this.instituteRepository = instituteRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(Institute institute)
        {
            return instituteRepository.Get(chk => chk.Name == institute.Name) == null ? false : true;
        }

        public bool CreateInstitute(Institute institute)
        {
            bool isSuccess = true;
            try
            {
                instituteRepository.Add(institute);
                this.SaveRecord();
                ServiceUtil<Institute>.WriteActionLog(institute.Id, ENUMOperation.CREATE, institute);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating Institute", ex);
            }
            return isSuccess;
        }

        public bool UpdateInstitute(Institute institute)
        {
            bool isSuccess = true;
            try
            {
                instituteRepository.Update(institute);
                this.SaveRecord();
                ServiceUtil<Institute>.WriteActionLog(institute.Id, ENUMOperation.UPDATE, institute);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating Institute", ex);
            }
            return isSuccess;
        }

        public bool DeleteInstitute(int id)
        {
            bool isSuccess = true;
            var institute = instituteRepository.GetById(id);
            try
            {
                instituteRepository.Delete(institute);
                SaveRecord();
                ServiceUtil<Institute>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting Institute", ex);
            }
            return isSuccess;
        }

        public Institute GetInstitute(int id)
        {
            return instituteRepository.GetById(id);
        }
        
        
        public IEnumerable<Institute> GetAllInstitute()
        {
            return instituteRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
