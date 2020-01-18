using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attendance.Data.Infrastructure;
using Attendance.Model;

namespace Attendance.Data.Repository
{
    public class ActionLogRepository: RepositoryBase<ActionLog>, IActionLogRepository
    {
        public ActionLogRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }

    public interface IActionLogRepository : IRepository<ActionLog>
    {
    }
}
