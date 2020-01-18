using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attendance.Data.Infrastructure;
using Attendance.Model;

namespace Attendance.Data.Repository
{
    public class SchoolRepository: RepositoryBase<School>, ISchoolRepository
    {
        public SchoolRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }

    public interface ISchoolRepository : IRepository<School>
    {
    }
}
