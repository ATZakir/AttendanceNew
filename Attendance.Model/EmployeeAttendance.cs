namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmployeeAttendance")]
    public partial class EmployeeAttendance
    {
        public Guid Id { get; set; }

        public int? SchoolId { get; set; }

        public int EmployeeId { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        public short? Type { get; set; }

        public TimeSpan? Time { get; set; }

        public int? RemarkId { get; set; }

        [Column(TypeName = "ntext")]
        public string Note { get; set; }

        public bool IsManualAttendance { get; set; }

        public virtual AttendanceRemark AttendanceRemark { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual School School { get; set; }
    }
}
