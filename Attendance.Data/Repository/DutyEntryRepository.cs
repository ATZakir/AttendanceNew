﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attendance.Data.Infrastructure;
using Attendance.Model;

namespace Attendance.Data.Repository
{
    public class DutyEntryRepository: RepositoryBase<DutyEntry>, IDutyEntryRepository
    {
        public DutyEntryRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }

    public interface IDutyEntryRepository : IRepository<DutyEntry>
    {
    }
}
