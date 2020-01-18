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
    public interface IEmployeeAttendanceSummaryService
    {

        bool CreateEmployeeAttendanceSummary(EmployeeAttendanceSummary employeeAttendanceSummary);
        bool UpdateEmployeeAttendanceSummary(EmployeeAttendanceSummary employeeAttendanceSummary);
        bool DeleteEmployeeAttendanceSummary(int id);
        EmployeeAttendanceSummary GetEmployeeAttendanceSummary(int id);
        
        IEnumerable<EmployeeAttendanceSummary> GetAllEmployeeAttendanceSummary();
        void SaveRecord();
        bool CheckIsExist(int empId, int year, int month);
        bool InsertEmployeeAttendanceSummary(int empId, int year, int month, int day, string flag);
        bool InsertEmployeeAttendanceSummary(int empId, DateTime? fromDate, DateTime? toDate, string flag);
    }
    public class EmployeeAttendanceSummaryService : IEmployeeAttendanceSummaryService
    {
        public EmployeeAttendanceSummaryService()
        {

        }
        private readonly IEmployeeAttendanceSummaryRepository employeeAttendanceSummaryRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly LoggingService logger = new LoggingService(typeof(EmployeeAttendanceSummaryService));

        public EmployeeAttendanceSummaryService(IEmployeeAttendanceSummaryRepository employeeAttendanceSummaryRepository, IUnitOfWork unitOfWork)
        {
            this.employeeAttendanceSummaryRepository = employeeAttendanceSummaryRepository;
            this.unitOfWork = unitOfWork;
        }


        public bool CheckIsExist(int empId, int year, int month)
        {
            var summaryFound = employeeAttendanceSummaryRepository.Get(chk => chk.EmployeeId == empId
                                                                              && chk.Year == year
                                                                              && chk.Month == month) != null;
            return summaryFound;
        }

        public bool InsertEmployeeAttendanceSummary(int empId, int year, int month, int day, string flag)
        {
            bool isSuccess = false;
            
            if (!CheckIsExist(empId, year, month))
            {
                var attendanceSummary = new EmployeeAttendanceSummary();
                attendanceSummary.EmployeeId = empId;
                attendanceSummary.Year = year;
                attendanceSummary.Month = (short)month;
                switch (day)
                {
                    case 1:
                    attendanceSummary.D1 = flag;
                    break;
                    case 2:
                    attendanceSummary.D2 = flag;
                    break;
                    case 3:
                    attendanceSummary.D3 = flag;
                    break;
                    case 4:
                    attendanceSummary.D4 = flag;
                    break;
                    case 5:
                    attendanceSummary.D5 = flag;
                    break;
                    case 6:
                    attendanceSummary.D6 = flag;
                    break;
                    case 7:
                    attendanceSummary.D7 = flag;
                    break;
                    case 8:
                    attendanceSummary.D8 = flag;
                    break;
                    case 9:
                    attendanceSummary.D9 = flag;
                    break;
                    case 10:
                    attendanceSummary.D10 = flag;
                    break;
                    case 11:
                    attendanceSummary.D11 = flag;
                    break;
                    case 12:
                    attendanceSummary.D12 = flag;
                    break;
                    case 13:
                    attendanceSummary.D13 = flag;
                    break;
                    case 14:
                    attendanceSummary.D14 = flag;
                    break;
                    case 15:
                    attendanceSummary.D15 = flag;
                    break;
                    case 16:
                    attendanceSummary.D16 = flag;
                    break;
                    case 17:
                    attendanceSummary.D17 = flag;
                    break;
                    case 18:
                    attendanceSummary.D18 = flag;
                    break;
                    case 19:
                    attendanceSummary.D19 = flag;
                    break;
                    case 20:
                    attendanceSummary.D20 = flag;
                    break;
                    case 21:
                    attendanceSummary.D21 = flag;
                    break;
                    case 22:
                    attendanceSummary.D22 = flag;
                    break;
                    case 23:
                    attendanceSummary.D23 = flag;
                    break;
                    case 24:
                    attendanceSummary.D24 = flag;
                    break;
                    case 25:
                    attendanceSummary.D25 = flag;
                    break;
                    case 26:
                    attendanceSummary.D26 = flag;
                    break;
                    case 27:
                    attendanceSummary.D27 = flag;
                    break;
                    case 28:
                    attendanceSummary.D28 = flag;
                    break;
                    case 29:
                    attendanceSummary.D29 = flag;
                    break;
                    case 30:
                    attendanceSummary.D30 = flag;
                    break;
                    case 31:
                    attendanceSummary.D31 = flag;
                    break;
                }

                isSuccess = CreateEmployeeAttendanceSummary(attendanceSummary);
            }
            else
            {

                var summary = GetEmployeeAttendanceSummary(empId, year, month);
                if (summary != null)
                {
                    switch (day)
                    {
                        case 1:
                        summary.D1 = flag;
                        break;
                        case 2:
                        summary.D2 = flag;
                        break;
                        case 3:
                        summary.D3 = flag;
                        break;
                        case 4:
                        summary.D4 = flag;
                        break;
                        case 5:
                        summary.D5 = flag;
                        break;
                        case 6:
                        summary.D6 = flag;
                        break;
                        case 7:
                        summary.D7 = flag;
                        break;
                        case 8:
                        summary.D8 = flag;
                        break;
                        case 9:
                        summary.D9 = flag;
                        break;
                        case 10:
                        summary.D10 = flag;
                        break;
                        case 11:
                        summary.D11 = flag;
                        break;
                        case 12:
                        summary.D12 = flag;
                        break;
                        case 13:
                        summary.D13 = flag;
                        break;
                        case 14:
                        summary.D14 = flag;
                        break;
                        case 15:
                        summary.D15 = flag;
                        break;
                        case 16:
                        summary.D16 = flag;
                        break;
                        case 17:
                        summary.D17 = flag;
                        break;
                        case 18:
                        summary.D18 = flag;
                        break;
                        case 19:
                        summary.D19 = flag;
                        break;
                        case 20:
                        summary.D20 = flag;
                        break;
                        case 21:
                        summary.D21 = flag;
                        break;
                        case 22:
                        summary.D22 = flag;
                        break;
                        case 23:
                        summary.D23 = flag;
                        break;
                        case 24:
                        summary.D24 = flag;
                        break;
                        case 25:
                        summary.D25 = flag;
                        break;
                        case 26:
                        summary.D26 = flag;
                        break;
                        case 27:
                        summary.D27 = flag;
                        break;
                        case 28:
                        summary.D28 = flag;
                        break;
                        case 29:
                        summary.D29 = flag;
                        break;
                        case 30:
                        summary.D30 = flag;
                        break;
                        case 31:
                        summary.D31 = flag;
                        break;
                    }

                    isSuccess = UpdateEmployeeAttendanceSummary(summary);
                }
            }

            return isSuccess;
        }


        public bool InsertEmployeeAttendanceSummary(int empId, DateTime? fromDate, DateTime? toDate, string flag)
        {
            bool isSuccess = false;
            if (fromDate == null || toDate == null)
            {
                return false;
            }
            for (DateTime date = fromDate.Value.Date; date <= toDate.Value.Date; date = date.AddDays(1))
            {
                if (!CheckIsExist(empId, date.Year, date.Month))
                {
                    var attendanceSummary = new EmployeeAttendanceSummary();
                    attendanceSummary.EmployeeId = empId;
                    attendanceSummary.Year = date.Year;
                    attendanceSummary.Month = (short)date.Month;
                    switch (date.Day)
                    {
                        case 1:
                        attendanceSummary.D1 = flag;
                        break;
                        case 2:
                        attendanceSummary.D2 = flag;
                        break;
                        case 3:
                        attendanceSummary.D3 = flag;
                        break;
                        case 4:
                        attendanceSummary.D4 = flag;
                        break;
                        case 5:
                        attendanceSummary.D5 = flag;
                        break;
                        case 6:
                        attendanceSummary.D6 = flag;
                        break;
                        case 7:
                        attendanceSummary.D7 = flag;
                        break;
                        case 8:
                        attendanceSummary.D8 = flag;
                        break;
                        case 9:
                        attendanceSummary.D9 = flag;
                        break;
                        case 10:
                        attendanceSummary.D10 = flag;
                        break;
                        case 11:
                        attendanceSummary.D11 = flag;
                        break;
                        case 12:
                        attendanceSummary.D12 = flag;
                        break;
                        case 13:
                        attendanceSummary.D13 = flag;
                        break;
                        case 14:
                        attendanceSummary.D14 = flag;
                        break;
                        case 15:
                        attendanceSummary.D15 = flag;
                        break;
                        case 16:
                        attendanceSummary.D16 = flag;
                        break;
                        case 17:
                        attendanceSummary.D17 = flag;
                        break;
                        case 18:
                        attendanceSummary.D18 = flag;
                        break;
                        case 19:
                        attendanceSummary.D19 = flag;
                        break;
                        case 20:
                        attendanceSummary.D20 = flag;
                        break;
                        case 21:
                        attendanceSummary.D21 = flag;
                        break;
                        case 22:
                        attendanceSummary.D22 = flag;
                        break;
                        case 23:
                        attendanceSummary.D23 = flag;
                        break;
                        case 24:
                        attendanceSummary.D24 = flag;
                        break;
                        case 25:
                        attendanceSummary.D25 = flag;
                        break;
                        case 26:
                        attendanceSummary.D26 = flag;
                        break;
                        case 27:
                        attendanceSummary.D27 = flag;
                        break;
                        case 28:
                        attendanceSummary.D28 = flag;
                        break;
                        case 29:
                        attendanceSummary.D29 = flag;
                        break;
                        case 30:
                        attendanceSummary.D30 = flag;
                        break;
                        case 31:
                        attendanceSummary.D31 = flag;
                        break;
                    }

                    isSuccess = CreateEmployeeAttendanceSummary(attendanceSummary);
                }
                else
                {
                    var summary = GetEmployeeAttendanceSummary(empId, date.Year, date.Month);
                    if (summary != null)
                    {
                        switch (date.Day)
                        {
                            case 1:
                            summary.D1 = flag;
                            break;
                            case 2:
                            summary.D2 = flag;
                            break;
                            case 3:
                            summary.D3 = flag;
                            break;
                            case 4:
                            summary.D4 = flag;
                            break;
                            case 5:
                            summary.D5 = flag;
                            break;
                            case 6:
                            summary.D6 = flag;
                            break;
                            case 7:
                            summary.D7 = flag;
                            break;
                            case 8:
                            summary.D8 = flag;
                            break;
                            case 9:
                            summary.D9 = flag;
                            break;
                            case 10:
                            summary.D10 = flag;
                            break;
                            case 11:
                            summary.D11 = flag;
                            break;
                            case 12:
                            summary.D12 = flag;
                            break;
                            case 13:
                            summary.D13 = flag;
                            break;
                            case 14:
                            summary.D14 = flag;
                            break;
                            case 15:
                            summary.D15 = flag;
                            break;
                            case 16:
                            summary.D16 = flag;
                            break;
                            case 17:
                            summary.D17 = flag;
                            break;
                            case 18:
                            summary.D18 = flag;
                            break;
                            case 19:
                            summary.D19 = flag;
                            break;
                            case 20:
                            summary.D20 = flag;
                            break;
                            case 21:
                            summary.D21 = flag;
                            break;
                            case 22:
                            summary.D22 = flag;
                            break;
                            case 23:
                            summary.D23 = flag;
                            break;
                            case 24:
                            summary.D24 = flag;
                            break;
                            case 25:
                            summary.D25 = flag;
                            break;
                            case 26:
                            summary.D26 = flag;
                            break;
                            case 27:
                            summary.D27 = flag;
                            break;
                            case 28:
                            summary.D28 = flag;
                            break;
                            case 29:
                            summary.D29 = flag;
                            break;
                            case 30:
                            summary.D30 = flag;
                            break;
                            case 31:
                            summary.D31 = flag;
                            break;
                        }

                        isSuccess = UpdateEmployeeAttendanceSummary(summary);
                    }
                }

            }
            return isSuccess;
        }

        public EmployeeAttendanceSummary GetEmployeeAttendanceSummary(int empId, int year, int month)
        {
            return employeeAttendanceSummaryRepository.Get(chk => chk.EmployeeId == empId
                                                                 && chk.Year == year
                                                                 && chk.Month == month);
        }

        public bool CreateEmployeeAttendanceSummary(EmployeeAttendanceSummary employeeAttendanceSummary)
        {
            bool isSuccess = true;
            try
            {
                employeeAttendanceSummaryRepository.Add(employeeAttendanceSummary);
                this.SaveRecord();
                ServiceUtil<EmployeeAttendanceSummary>.WriteActionLog(employeeAttendanceSummary.Id, ENUMOperation.CREATE, employeeAttendanceSummary);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in creating EmployeeAttendanceSummary", ex);
            }
            return isSuccess;
        }

        public bool UpdateEmployeeAttendanceSummary(EmployeeAttendanceSummary employeeAttendanceSummary)
        {
            bool isSuccess = true;
            try
            {
                employeeAttendanceSummaryRepository.Update(employeeAttendanceSummary);
                this.SaveRecord();
                ServiceUtil<EmployeeAttendanceSummary>.WriteActionLog(employeeAttendanceSummary.Id, ENUMOperation.UPDATE, employeeAttendanceSummary);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in updating EmployeeAttendanceSummary", ex);
            }
            return isSuccess;
        }

        public bool DeleteEmployeeAttendanceSummary(int id)
        {
            bool isSuccess = true;
            var employeeAttendanceSummary = employeeAttendanceSummaryRepository.GetById(id);
            try
            {
                employeeAttendanceSummaryRepository.Delete(employeeAttendanceSummary);
                SaveRecord();
                ServiceUtil<EmployeeAttendanceSummary>.WriteActionLog(id, ENUMOperation.DELETE);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in deleting EmployeeAttendanceSummary", ex);
            }
            return isSuccess;
        }

        public EmployeeAttendanceSummary GetEmployeeAttendanceSummary(int id)
        {
            return employeeAttendanceSummaryRepository.GetById(id);
        }
        
        
        public IEnumerable<EmployeeAttendanceSummary> GetAllEmployeeAttendanceSummary()
        {
            return employeeAttendanceSummaryRepository.GetAll();
        }
        public void SaveRecord()
        {
            unitOfWork.Commit();
        }
    }
}
