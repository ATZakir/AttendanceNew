namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AccademicYearDetail
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccademicYear { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime DayDate { get; set; }

        public int DayType { get; set; }

        [StringLength(100)]
        public string DayDescription { get; set; }

        public virtual DayType DayType1 { get; set; }
    }
}
