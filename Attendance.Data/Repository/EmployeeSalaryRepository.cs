using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attendance.Data.Infrastructure;
using Attendance.Model;

namespace Attendance.Data.Repository
{
    public class EmployeeSalaryRepository: RepositoryBase<EmployeeSalary>, IEmployeeSalaryRepository
    {
        public EmployeeSalaryRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }

    public interface IEmployeeSalaryRepository : IRepository<EmployeeSalary>
    {
    }
}
