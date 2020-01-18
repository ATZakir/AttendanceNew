namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserPermission")]
    public partial class UserPermission
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }

        public int DistrictId { get; set; }

        public int DivisionId { get; set; }

        public int UpazilaId { get; set; }

        public int SchoolId { get; set; }

        public virtual User User { get; set; }
    }
}
