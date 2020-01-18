namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmployeeLeaveBalance")]
    public partial class EmployeeLeaveBalance
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int? Year { get; set; }

        public int? LeaveTypeId { get; set; }

        public int? DafaultAllocatedDays { get; set; }

        public int? Achieved { get; set; }

        public bool? CarryForward { get; set; }

        public int? Balance { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual LeaveType LeaveType { get; set; }
    }
}
