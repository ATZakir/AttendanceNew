using System;
using Attendance.Data;

namespace Attendance.Data.Infrastructure
{
    public interface IDatabaseFactory : IDisposable
    {
        ApplicationEntities Get();
    }
}
