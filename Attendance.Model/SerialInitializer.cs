namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SerialInitializer")]
    public partial class SerialInitializer
    {
        public int Id { get; set; }

        public int? ItemIssue { get; set; }

        public int? Year { get; set; }

        public int? ItemDemand { get; set; }

        public int? ItemScrapExpireOut { get; set; }

        public int? ItemRecieptVoucher { get; set; }
    }
}
