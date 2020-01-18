namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DeviceSetup")]
    public partial class DeviceSetup
    {
        public int Id { get; set; }

        public int? SchoolId { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public int? TypeId { get; set; }

        [StringLength(100)]
        public string Location { get; set; }

        [StringLength(100)]
        public string Gate { get; set; }

        [StringLength(50)]
        public string IPAddress { get; set; }

        [StringLength(50)]
        public string Port { get; set; }

        public bool? IsActive { get; set; }

        public virtual DeviceType DeviceType { get; set; }

        public virtual School School { get; set; }
    }
}
