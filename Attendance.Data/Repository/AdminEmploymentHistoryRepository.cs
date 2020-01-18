using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attendance.Data.Infrastructure;
using Attendance.Model;

namespace Attendance.Data.Repository
{
    public class AdminEmploymentHistoryRepository: RepositoryBase<AdminEmploymentHistory>, IAdminEmploymentHistoryRepository
    {
        public AdminEmploymentHistoryRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }

    public interface IAdminEmploymentHistoryRepository : IRepository<AdminEmploymentHistory>
    {
    }
}
