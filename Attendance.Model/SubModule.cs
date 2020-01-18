namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SubModule")]
    public partial class SubModule
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SubModule()
        {
            SubModuleItems = new HashSet<SubModuleItem>();
        }

        public int Id { get; set; }

        public int? ModuleId { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public byte? Ordering { get; set; }

        public bool? IsActive { get; set; }

        public virtual Module Module { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SubModuleItem> SubModuleItems { get; set; }
    }
}
