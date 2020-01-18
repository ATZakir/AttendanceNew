namespace Attendance.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Employee")]
    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            AdminEmploymentHistories = new HashSet<AdminEmploymentHistory>();
            DutyEntries = new HashSet<DutyEntry>();
            EmployeeAttendanceSummaries = new HashSet<EmployeeAttendanceSummary>();
            EmployeeEducations = new HashSet<EmployeeEducation>();
            EmployeeLeaveBalances = new HashSet<EmployeeLeaveBalance>();
            EmployeeSalaries = new HashSet<EmployeeSalary>();
            EmploymentHistories = new HashSet<EmploymentHistory>();
            Leaves = new HashSet<Leave>();
            EmployeeAttendances = new HashSet<EmployeeAttendance>();
            NotificationSettings = new HashSet<NotificationSetting>();
            Trainings = new HashSet<Training>();
            Users = new HashSet<User>();
            WorkflowactionSettings = new HashSet<WorkflowactionSetting>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [StringLength(200)]
        public string PresentAddress { get; set; }

        [StringLength(200)]
        public string PermanentAddress { get; set; }

        [StringLength(100)]
        public string FatherName { get; set; }

        [StringLength(100)]
        public string MotherName { get; set; }

        [StringLength(100)]
        public string SpouseName { get; set; }

        [StringLength(50)]
        public string NationalId { get; set; }

        [StringLength(50)]
        public string PassportNo { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(50)]
        public string OfficePhone { get; set; }

        [StringLength(50)]
        public string OfficeMobile { get; set; }

        [StringLength(50)]
        public string ResidentPhone { get; set; }

        [StringLength(50)]
        public string ResidentMobile { get; set; }

        [StringLength(10)]
        public string BloodGroup { get; set; }

        [Column(TypeName = "date")]
        public DateTime? JoinDate { get; set; }

        [StringLength(50)]
        public string Gender { get; set; }

        [StringLength(50)]
        public string MaritalStatus { get; set; }

        [StringLength(50)]
        public string Relgion { get; set; }

        [StringLength(50)]
        public string Nationality { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DOB { get; set; }

        [StringLength(250)]
        public string PhotoPath { get; set; }

        [StringLength(100)]
        public string FullNameBangla { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdminEmploymentHistory> AdminEmploymentHistories { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DutyEntry> DutyEntries { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeAttendanceSummary> EmployeeAttendanceSummaries { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeEducation> EmployeeEducations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeLeaveBalance> EmployeeLeaveBalances { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeSalary> EmployeeSalaries { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmploymentHistory> EmploymentHistories { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Leave> Leaves { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeAttendance> EmployeeAttendances { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificationSetting> NotificationSettings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Training> Trainings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Users { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkflowactionSetting> WorkflowactionSettings { get; set; }
    }
}
