using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attendance.Data.Infrastructure;
using Attendance.Model;

namespace Attendance.Data.Repository
{
    public class EmployeeEducationRepository: RepositoryBase<EmployeeEducation>, IEmployeeEducationRepository
    {
        public EmployeeEducationRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }

    public interface IEmployeeEducationRepository : IRepository<EmployeeEducation>
    {
    }
}
