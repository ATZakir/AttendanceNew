using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attendance.Data.Infrastructure;
using Attendance.Model;

namespace Attendance.Data.Repository
{
    public class LeaveTypeRepository: RepositoryBase<LeaveType>, ILeaveTypeRepository
    {
        public LeaveTypeRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }

    public interface ILeaveTypeRepository : IRepository<LeaveType>
    {
    }
}
