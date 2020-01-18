namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SubModuleItem")]
    public partial class SubModuleItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SubModuleItem()
        {
            NotificationSettings = new HashSet<NotificationSetting>();
            RoleSubModuleItems = new HashSet<RoleSubModuleItem>();
            SubModuleItem1 = new HashSet<SubModuleItem>();
            WorkflowactionSettings = new HashSet<WorkflowactionSetting>();
        }

        public int Id { get; set; }

        public int? SubModuleId { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(200)]
        public string UrlPath { get; set; }

        public byte? Ordering { get; set; }

        public bool? IsBaseItem { get; set; }

        public bool? IsActive { get; set; }

        public int? BaseItemId { get; set; }

        public bool? IsWFASettingsRequired { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificationSetting> NotificationSettings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RoleSubModuleItem> RoleSubModuleItems { get; set; }

        public virtual SubModule SubModule { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SubModuleItem> SubModuleItem1 { get; set; }

        public virtual SubModuleItem SubModuleItem2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkflowactionSetting> WorkflowactionSettings { get; set; }
    }
}
