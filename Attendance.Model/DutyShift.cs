namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DutyShift")]
    public partial class DutyShift
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DutyShift()
        {
            EmploymentHistories = new HashSet<EmploymentHistory>();
        }

        public int Id { get; set; }

        public int DutyShiftMasterId { get; set; }

        public TimeSpan? InTime { get; set; }

        public TimeSpan? MaxInTime { get; set; }

        public TimeSpan? OutTime { get; set; }

        public int? SchoolId { get; set; }

        public virtual DutyShiftMaster DutyShiftMaster { get; set; }

        public virtual School School { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmploymentHistory> EmploymentHistories { get; set; }
    }
}
