using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attendance.Data.Infrastructure;
using Attendance.Model;

namespace Attendance.Data.Repository
{
    public class EmployeeLeaveBalanceRepository: RepositoryBase<EmployeeLeaveBalance>, IEmployeeLeaveBalanceRepository
    {
        public EmployeeLeaveBalanceRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }

    public interface IEmployeeLeaveBalanceRepository : IRepository<EmployeeLeaveBalance>
    {
    }
}
