namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmploymentHistory")]
    public partial class EmploymentHistory
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int? DepartmentId { get; set; }

        public int? DesignationId { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public int? DutyShiftId { get; set; }

        public int? SchoolId { get; set; }

        public virtual Department Department { get; set; }

        public virtual Designation Designation { get; set; }

        public virtual DutyShift DutyShift { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual School School { get; set; }
    }
}
