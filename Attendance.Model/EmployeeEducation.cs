namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmployeeEducation")]
    public partial class EmployeeEducation
    {
        public long Id { get; set; }

        public int EmployeeId { get; set; }

        public int LevelId { get; set; }

        [Required]
        [StringLength(200)]
        public string DegreeTitle { get; set; }

        [StringLength(200)]
        public string DegreeMajor { get; set; }

        [Required]
        [StringLength(50)]
        public string Result { get; set; }

        public int BoardOrUniversityId { get; set; }

        public int? InstituteId { get; set; }

        [StringLength(50)]
        public string Scale { get; set; }

        public virtual BoardOrUniversity BoardOrUniversity { get; set; }

        public virtual EducationLevel EducationLevel { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual Institute Institute { get; set; }
    }
}
