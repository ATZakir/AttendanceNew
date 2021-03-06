﻿using Attendance.Data.Repository;
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
    public interface IDesignationService
    {

        bool CreateDesignation(Designation designation);
        bool UpdateDesignation(Designation designation);
        bool DeleteDesignation(int id);
        Designation GetDesignation(int id);
        IEnumerable<Designation> GetAllDesignationByDepartmentId(int departmentId);
        IEnumerable<Designation> GetAllDesignation();
        void SaveRecord();
        bool CheckIsExist(Designation designation);
    }
    public class DesignationService : IDesignationService
    {
        public DesignationService()
        {

        }
        private readonly IDesignationRepository designationRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(DesignationService));

        public DesignationService(IDesignationRepository designationRepository, IUnitOfWork unitOfWork)
        {
            this.designationRepository = designationRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(Designation designation)
        {
            return designationRepository.Get(chk => chk.Name == designation.Name) == null ? false : true;
        }

        public bool CreateDesignation(Designation designation)
        {
            bool isSuccess = true;
            try
            {
                designationRepository.Add(designation);
                this.SaveRecord();
                ServiceUtil<Designation>.WriteActionLog(designation.Id, ENUMOperation.CREATE, designation);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating Designation", ex);
            }
            return isSuccess;
        }

        public bool UpdateDesignation(Designation designation)
        {
            bool isSuccess = true;
            try
            {
                designationRepository.Update(designation);
                this.SaveRecord();
                ServiceUtil<Designation>.WriteActionLog(designation.Id, ENUMOperation.UPDATE, designation);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating Designation", ex);
            }
            return isSuccess;
        }

        public bool DeleteDesignation(int id)
        {
            bool isSuccess = true;
            var designation = designationRepository.GetById(id);
            try
            {
                designationRepository.Delete(designation);
                SaveRecord();
                ServiceUtil<Designation>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting Designation", ex);
            }
            return isSuccess;
        }

        public Designation GetDesignation(int id)
        {
            return designationRepository.GetById(id);
        }

        public IEnumerable<Designation> GetAllDesignationByDepartmentId(int departmentId)
        {
            return designationRepository.GetMany(d => d.DepartmentId == departmentId);
        }

        public IEnumerable<Designation> GetAllDesignation()
        {
            return designationRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
