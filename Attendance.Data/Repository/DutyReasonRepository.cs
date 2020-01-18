using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attendance.Data.Infrastructure;
using Attendance.Model;

namespace Attendance.Data.Repository
{
    public class DutyReasonRepository: RepositoryBase<DutyReason>, IDutyReasonRepository
    {
        public DutyReasonRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }

    public interface IDutyReasonRepository : IRepository<DutyReason>
    {
    }
}
