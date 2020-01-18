namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DeviceAttendance")]
    public partial class DeviceAttendance
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string EmployeeCode { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime DateTime { get; set; }

        [StringLength(50)]
        public string DeviceCode { get; set; }

        [StringLength(50)]
        public string IP { get; set; }

        [StringLength(10)]
        public string Port { get; set; }

        [StringLength(10)]
        public string Inout { get; set; }
    }
}
