namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ActionLog")]
    public partial class ActionLog
    {
        public Guid Id { get; set; }

        public int? Who { get; set; }

        public DateTime? When { get; set; }

        [StringLength(50)]
        public string AffectedRecordId { get; set; }

        [Column(TypeName = "xml")]
        public string What { get; set; }

        [StringLength(50)]
        public string ActionCRUD { get; set; }

        [StringLength(50)]
        public string Entity { get; set; }

        [StringLength(50)]
        public string IPAddress { get; set; }

        public virtual User User { get; set; }
    }
}
