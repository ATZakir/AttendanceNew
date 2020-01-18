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
    public interface IUserService
    {
        bool CreateUser(User user);
        bool UpdateUser(User user);
        bool DeleteUser(int id);
        User GetUser(int id);
        User GetUserByLoginNameAndPassword(User user);
        User GetUserByEmail(string email);
        //User GetUserByMobile(string mobile);
        
        User AuthenticateUser(User user);
        IEnumerable<User> GetAllUser();
        bool CheckIsExist(User user);

        void SaveRecord();
        bool CheckEmailExists(User user);
    }


    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(UserService));

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            this.userRepository = userRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CreateUser(User user)
        {
            bool isSuccess = true;
            try
            {
                userRepository.Add(user);
                this.SaveRecord();
                ServiceUtil<User>.WriteActionLog(user.Id, ENUMOperation.CREATE, user);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating User", ex);
            }
            return isSuccess;
        }

        public bool UpdateUser(User user)
        {
            bool isSuccess = true;
            try
            {
                userRepository.Update(user);
                this.SaveRecord();
                ServiceUtil<User>.WriteActionLog(user.Id, ENUMOperation.UPDATE, user);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating User", ex);
            }
            return isSuccess;
        }

        public bool DeleteUser(int id)
        {
            bool isSuccess = true;
            try
            {
                User user = GetUser(id);
                userRepository.Delete(user);
                SaveRecord();
                ServiceUtil<User>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting User", ex);
            }
            return isSuccess;
        }

        public User GetUser(int id)
        {
            return userRepository.GetById(id);
        }

        public User GetUserByLoginNameAndPassword(User user)
        {
            return userRepository.Get(u => u.LoginName == user.LoginName && u.Password == user.Password);
        }

        public User GetUserByEmail(string email)
        {
            return userRepository.Get(u => u.Email == email);
        }

        //public User GetUserByMobile(string mobile)
        //{
        //    return userRepository.Get(u => u.Mobile == mobile);
        //}

        public User AuthenticateUser(User user)
        {
            User getUserInfo = new User();
            try
            {
                getUserInfo = userRepository.Get(u => u.LoginName.ToUpper() == user.LoginName.ToUpper() && u.Password == user.Password);
            }
            catch (Exception e)
            {
                getUserInfo = null;
                logger.Error("Error in authenticating user", e);
            }
            return getUserInfo;
        }

        public IEnumerable<User> GetAllUser()
        {
            return userRepository.GetAll();
        }

        public bool CheckIsExist(User user)
        {
            return userRepository.Get(u => u.LoginName == user.LoginName) != null;
        }

        public void SaveRecord()
        {
            unitOfWork.Commit();
        }

        public bool CheckEmailExists(User user)
        {
            return userRepository.Get(u => u.Email == user.Email) != null;
        }
    }
}
