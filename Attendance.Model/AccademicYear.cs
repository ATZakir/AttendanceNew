namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AccademicYear")]
    public partial class AccademicYear
    {
        [Key]
        [Column("AccademicYear")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccademicYear1 { get; set; }

        public int Weekend1 { get; set; }

        public int Weekend2 { get; set; }
    }
}
