using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attendance.Data.Infrastructure;
using Attendance.Model;

namespace Attendance.Data.Repository
{
    public class LeaveRepository: RepositoryBase<Leave>, ILeaveRepository
    {
        public LeaveRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }

    public interface ILeaveRepository : IRepository<Leave>
    {
    }
}
