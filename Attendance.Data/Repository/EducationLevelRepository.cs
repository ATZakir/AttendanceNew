﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attendance.Data.Infrastructure;
using Attendance.Model;

namespace Attendance.Data.Repository
{
    public class EducationLevelRepository: RepositoryBase<EducationLevel>, IEducationLevelRepository
    {
        public EducationLevelRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }

    public interface IEducationLevelRepository : IRepository<EducationLevel>
    {
    }
}
