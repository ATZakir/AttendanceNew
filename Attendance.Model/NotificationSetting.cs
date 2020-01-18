namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NotificationSetting")]
    public partial class NotificationSetting
    {
        public Guid Id { get; set; }

        public int? SubModuleItemId { get; set; }

        public int? NotifiedEmployeeId { get; set; }

        public int? WorkflowactionId { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual SubModuleItem SubModuleItem { get; set; }

        public virtual Workflowaction Workflowaction { get; set; }
    }
}
