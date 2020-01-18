namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmployeeSalary")]
    public partial class EmployeeSalary
    {
        public Guid Id { get; set; }

        public int EmployeeId { get; set; }

        [StringLength(50)]
        public string SalaryAccount { get; set; }

        [StringLength(50)]
        public string GPFNo { get; set; }

        [StringLength(50)]
        public string MBNo { get; set; }

        [StringLength(50)]
        public string SalaryScale { get; set; }

        [StringLength(50)]
        public string BasicSalary { get; set; }

        public int? SchoolId { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual School School { get; set; }
    }
}
