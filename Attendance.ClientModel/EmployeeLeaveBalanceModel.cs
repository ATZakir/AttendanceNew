using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.ClientModel
{
    public class EmployeeLeaveBalanceModel
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int Year { get; set; }

        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }

        public int? DafaultAllocatedDays { get; set; }

        public int? Achieved { get; set; }

        public bool? CarryForward { get; set; }

        public int? Balance { get; set; }
    }
}
