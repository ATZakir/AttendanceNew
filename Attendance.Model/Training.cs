namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Training")]
    public partial class Training
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public short? Status { get; set; }

        public DateTime? RemovedOn { get; set; }

        public int? RemovedBy { get; set; }

        public int? TypeId { get; set; }

        public int? SchoolId { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual School School { get; set; }

        public virtual TrainingType TrainingType { get; set; }
    }
}
