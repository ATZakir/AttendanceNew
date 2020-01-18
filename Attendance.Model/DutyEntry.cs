namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DutyEntry")]
    public partial class DutyEntry
    {
        public Guid Id { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }

        public int EmployeeId { get; set; }

        public TimeSpan? InTime { get; set; }

        public TimeSpan? OutTime { get; set; }

        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }

        public int? ReasonId { get; set; }

        [StringLength(250)]
        public string Remarks { get; set; }

        [StringLength(100)]
        public string Location { get; set; }

        public DateTime? RemovedOn { get; set; }

        public int? RemovedBy { get; set; }

        public int? SchoolId { get; set; }

        public virtual DutyReason DutyReason { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual School School { get; set; }
    }
}
