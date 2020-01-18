namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Leave")]
    public partial class Leave
    {
        [StringLength(10)]
        public string Id { get; set; }

        public int LeaveTypeId { get; set; }

        public int EmployeeId { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }

        [StringLength(250)]
        public string LeaveReason { get; set; }

        [StringLength(100)]
        public string ContactDuringLeave { get; set; }

        [StringLength(250)]
        public string AddressDuringLeave { get; set; }

        public short? Status { get; set; }

        public DateTime? RemovedOn { get; set; }

        public int? RemovedBy { get; set; }

        public int? NoOfDays { get; set; }

        public int? SchoolId { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual LeaveType LeaveType { get; set; }

        public virtual School School { get; set; }
    }
}
