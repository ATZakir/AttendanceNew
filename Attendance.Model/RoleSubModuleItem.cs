namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RoleSubModuleItem")]
    public partial class RoleSubModuleItem
    {
        public int Id { get; set; }

        public int? RoleId { get; set; }

        public int? SubModuleItemId { get; set; }

        public bool? CreateOperation { get; set; }

        public bool? ReadOperation { get; set; }

        public bool? UpdateOperation { get; set; }

        public bool? DeleteOperation { get; set; }

        public virtual Role Role { get; set; }

        public virtual SubModuleItem SubModuleItem { get; set; }
    }
}
