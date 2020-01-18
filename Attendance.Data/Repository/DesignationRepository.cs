using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attendance.Data.Infrastructure;
using Attendance.Model;

namespace Attendance.Data.Repository
{
    public class DesignationRepository: RepositoryBase<Designation>, IDesignationRepository
    {
        public DesignationRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }

    public interface IDesignationRepository : IRepository<Designation>
    {
    }
}
