namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Notice")]
    public partial class Notice
    {
        public long Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Title { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Validity { get; set; }

        [Required]
        [StringLength(250)]
        public string AttachFile { get; set; }
    }
}
