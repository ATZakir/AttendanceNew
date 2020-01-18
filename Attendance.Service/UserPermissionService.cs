using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attendance.Model;
using Attendance.Data.Repository;
using Attendance.Data.Infrastructure;
using Attendance.LoggerService;
using Attendance.Core.Common;

namespace Attendance.Service
{
    public interface IUserPermissionService
    {
        bool CreateUserPermission(UserPermission userPermission);
        bool UpdateUserPermission(UserPermission userPermission);
        bool DeleteUserPermission(Guid id);
        UserPermission GetUserPermission(Guid id);
        IEnumerable<UserPermission> GetUserPermissionByUserId(int userId);

        IEnumerable<UserPermission> GetAllUserPermission();
        bool CheckIsExist(UserPermission userPermission);

        void SaveRecord();
    }


    public class UserPermissionService : IUserPermissionService
    {
        private readonly IUserPermissionRepository userPermissionRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(UserPermissionService));

        public UserPermissionService(IUserPermissionRepository userPermissionRepository, IUnitOfWork unitOfWork)
        {
            this.userPermissionRepository = userPermissionRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CreateUserPermission(UserPermission userPermission)
        {
            bool isSuccess = true;
            try
            {
                userPermissionRepository.Add(userPermission);
                this.SaveRecord();
                ServiceUtil<UserPermission>.WriteActionLog(userPermission.Id, ENUMOperation.CREATE, userPermission);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating UserPermission", ex);
            }
            return isSuccess;
        }

        public bool UpdateUserPermission(UserPermission userPermission)
        {
            bool isSuccess = true;
            try
            {
                userPermissionRepository.Update(userPermission);
                this.SaveRecord();
                ServiceUtil<UserPermission>.WriteActionLog(userPermission.Id, ENUMOperation.UPDATE, userPermission);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating UserPermission", ex);
            }
            return isSuccess;
        }

        public bool DeleteUserPermission(Guid id)
        {
            bool isSuccess = true;
            try
            {
                UserPermission userPermission = GetUserPermission(id);
                userPermissionRepository.Delete(userPermission);
                SaveRecord();
                ServiceUtil<UserPermission>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting UserPermission", ex);
            }
            return isSuccess;
        }

        public UserPermission GetUserPermission(Guid id)
        {
            return userPermissionRepository.GetById(id);
        }

        public IEnumerable<UserPermission> GetUserPermissionByUserId(int userId)
        {
            return userPermissionRepository.GetMany(x=>x.UserId== userId);
        }

        public IEnumerable<UserPermission> GetAllUserPermission()
        {
            return userPermissionRepository.GetAll();
        }

        public bool CheckIsExist(UserPermission userPermission)
        {
            return userPermissionRepository.Get(u => u.UserId == userPermission.UserId && u.DivisionId==userPermission.DivisionId && u.DistrictId==userPermission.DistrictId && u.UpazilaId==userPermission.UpazilaId) != null;
        }

        public void SaveRecord()
        {
            unitOfWork.Commit();
        }

    }
}
