namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkflowactionSetting")]
    public partial class WorkflowactionSetting
    {
        public Guid Id { get; set; }

        public int? SubMouduleItemId { get; set; }

        public int? EmployeeId { get; set; }

        public int? WorkflowactionId { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual SubModuleItem SubModuleItem { get; set; }

        public virtual Workflowaction Workflowaction { get; set; }
    }
}
