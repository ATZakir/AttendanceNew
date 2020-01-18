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
    public interface IBoardOrUniversityService
    {

        bool CreateBoardOrUniversity(BoardOrUniversity boardorUniversity);
        bool UpdateBoardOrUniversity(BoardOrUniversity boardorUniversity);
        bool DeleteBoardOrUniversity(int id);
        BoardOrUniversity GetBoardOrUniversity(int id);
        
        IEnumerable<BoardOrUniversity> GetAllBoardOrUniversity();
        void SaveRecord();
        bool CheckIsExist(BoardOrUniversity boardorUniversity);
    }
    public class BoardOrUniversityService : IBoardOrUniversityService
    {
        public BoardOrUniversityService()
        {

        }
        private readonly IBoardOrUniversityRepository boardorUniversityRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(BoardOrUniversityService));

        public BoardOrUniversityService(IBoardOrUniversityRepository boardorUniversityRepository, IUnitOfWork unitOfWork)
        {
            this.boardorUniversityRepository = boardorUniversityRepository;
            this.unitOfWork = unitOfWork;
        }

        public bool CheckIsExist(BoardOrUniversity boardorUniversity)
        {
            return boardorUniversityRepository.Get(chk => chk.Name == boardorUniversity.Name) == null ? false : true;
        }

        public bool CreateBoardOrUniversity(BoardOrUniversity boardorUniversity)
        {
            bool isSuccess = true;
            try
            {
                boardorUniversityRepository.Add(boardorUniversity);
                this.SaveRecord();
                ServiceUtil<BoardOrUniversity>.WriteActionLog(boardorUniversity.Id, ENUMOperation.CREATE, boardorUniversity);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating BoardOrUniversity", ex);
            }
            return isSuccess;
        }

        public bool UpdateBoardOrUniversity(BoardOrUniversity boardorUniversity)
        {
            bool isSuccess = true;
            try
            {
                boardorUniversityRepository.Update(boardorUniversity);
                this.SaveRecord();
                ServiceUtil<BoardOrUniversity>.WriteActionLog(boardorUniversity.Id, ENUMOperation.UPDATE, boardorUniversity);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating BoardOrUniversity", ex);
            }
            return isSuccess;
        }

        public bool DeleteBoardOrUniversity(int id)
        {
            bool isSuccess = true;
            var boardorUniversity = boardorUniversityRepository.GetById(id);
            try
            {
                boardorUniversityRepository.Delete(boardorUniversity);
                SaveRecord();
                ServiceUtil<BoardOrUniversity>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting BoardOrUniversity", ex);
            }
            return isSuccess;
        }

        public BoardOrUniversity GetBoardOrUniversity(int id)
        {
            return boardorUniversityRepository.GetById(id);
        }
        
        
        public IEnumerable<BoardOrUniversity> GetAllBoardOrUniversity()
        {
            return boardorUniversityRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
