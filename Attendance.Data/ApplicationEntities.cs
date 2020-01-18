
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attendance.Model;

namespace Attendance.Data
{
    public class ApplicationEntities : DbContext
    {
        public ApplicationEntities()
            : base("DBConnection")
        {
          
        }

        public virtual DbSet<AccademicYear> AccademicYears { get; set; }
        public virtual DbSet<AccademicYearDetail> AccademicYearDetails { get; set; }
        public virtual DbSet<ActionLog> ActionLogs { get; set; }
        public virtual DbSet<AdminEmploymentHistory> AdminEmploymentHistories { get; set; }
        public virtual DbSet<AttendanceRemark> AttendanceRemarks { get; set; }
        public virtual DbSet<BoardOrUniversity> BoardOrUniversities { get; set; }
        public virtual DbSet<DayType> DayTypes { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Designation> Designations { get; set; }
        public virtual DbSet<DeviceAttendance> DeviceAttendances { get; set; }
        public virtual DbSet<DeviceSetup> DeviceSetups { get; set; }
        public virtual DbSet<DeviceType> DeviceTypes { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Division> Divisions { get; set; }
        public virtual DbSet<DutyEntry> DutyEntries { get; set; }
        public virtual DbSet<DutyReason> DutyReasons { get; set; }
        public virtual DbSet<DutyShift> DutyShifts { get; set; }
        public virtual DbSet<DutyShiftMaster> DutyShiftMasters { get; set; }
        public virtual DbSet<EducationLevel> EducationLevels { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeeAttendance> EmployeeAttendances { get; set; }
        public virtual DbSet<EmployeeAttendanceSummary> EmployeeAttendanceSummaries { get; set; }
        public virtual DbSet<EmployeeEducation> EmployeeEducations { get; set; }
        public virtual DbSet<EmployeeLeaveBalance> EmployeeLeaveBalances { get; set; }
        public virtual DbSet<EmployeeSalary> EmployeeSalaries { get; set; }
        public virtual DbSet<EmploymentHistory> EmploymentHistories { get; set; }
        public virtual DbSet<Institute> Institutes { get; set; }
        public virtual DbSet<Leave> Leaves { get; set; }
        public virtual DbSet<LeaveType> LeaveTypes { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Notice> Notices { get; set; }
        public virtual DbSet<NotificationSetting> NotificationSettings { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RoleSubModuleItem> RoleSubModuleItems { get; set; }
        public virtual DbSet<School> Schools { get; set; }
        public virtual DbSet<SerialInitializer> SerialInitializers { get; set; }
        public virtual DbSet<SubModule> SubModules { get; set; }
        public virtual DbSet<SubModuleItem> SubModuleItems { get; set; }
        public virtual DbSet<Training> Trainings { get; set; }
        public virtual DbSet<TrainingType> TrainingTypes { get; set; }
        public virtual DbSet<Upazila> Upazilas { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserPermission> UserPermissions { get; set; }
        public virtual DbSet<Workflowaction> Workflowactions { get; set; }
        public virtual DbSet<WorkflowactionSetting> WorkflowactionSettings { get; set; }

        public virtual void Commit()
        {
            base.SaveChanges();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttendanceRemark>()
                .HasMany(e => e.EmployeeAttendances)
                .WithOptional(e => e.AttendanceRemark)
                .HasForeignKey(e => e.RemarkId);

            modelBuilder.Entity<BoardOrUniversity>()
                .HasMany(e => e.EmployeeEducations)
                .WithRequired(e => e.BoardOrUniversity)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DayType>()
                .HasMany(e => e.AccademicYearDetails)
                .WithRequired(e => e.DayType1)
                .HasForeignKey(e => e.DayType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Designations)
                .WithRequired(e => e.Department)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DeviceAttendance>()
                .Property(e => e.Inout)
                .IsFixedLength();

            modelBuilder.Entity<DeviceType>()
                .HasMany(e => e.DeviceSetups)
                .WithOptional(e => e.DeviceType)
                .HasForeignKey(e => e.TypeId);

            modelBuilder.Entity<District>()
                .HasMany(e => e.AdminEmploymentHistories)
                .WithOptional(e => e.District)
                .HasForeignKey(e => e.DistrictlId);

            modelBuilder.Entity<District>()
                .HasMany(e => e.Institutes)
                .WithRequired(e => e.District)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<District>()
                .HasMany(e => e.Upazilas)
                .WithRequired(e => e.District)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Division>()
                .HasMany(e => e.Districts)
                .WithRequired(e => e.Division)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DutyReason>()
                .HasMany(e => e.DutyEntries)
                .WithOptional(e => e.DutyReason)
                .HasForeignKey(e => e.ReasonId);

            modelBuilder.Entity<DutyShiftMaster>()
                .HasMany(e => e.DutyShifts)
                .WithRequired(e => e.DutyShiftMaster)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<EducationLevel>()
                .HasMany(e => e.EmployeeEducations)
                .WithRequired(e => e.EducationLevel)
                .HasForeignKey(e => e.LevelId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.AdminEmploymentHistories)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.DutyEntries)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.EmployeeAttendanceSummaries)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.EmployeeEducations)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.EmployeeLeaveBalances)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.EmployeeSalaries)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.EmploymentHistories)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Leaves)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.EmployeeAttendances)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.NotificationSettings)
                .WithOptional(e => e.Employee)
                .HasForeignKey(e => e.NotifiedEmployeeId);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Trainings)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Leave>()
                .Property(e => e.Id)
                .IsFixedLength();

            modelBuilder.Entity<LeaveType>()
                .HasMany(e => e.Leaves)
                .WithRequired(e => e.LeaveType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Role>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.Role)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SubModuleItem>()
                .HasMany(e => e.SubModuleItem1)
                .WithOptional(e => e.SubModuleItem2)
                .HasForeignKey(e => e.BaseItemId);

            modelBuilder.Entity<SubModuleItem>()
                .HasMany(e => e.WorkflowactionSettings)
                .WithOptional(e => e.SubModuleItem)
                .HasForeignKey(e => e.SubMouduleItemId);

            modelBuilder.Entity<TrainingType>()
                .HasMany(e => e.Trainings)
                .WithOptional(e => e.TrainingType)
                .HasForeignKey(e => e.TypeId);

            modelBuilder.Entity<Upazila>()
                .HasMany(e => e.AdminEmploymentHistories)
                .WithOptional(e => e.Upazila)
                .HasForeignKey(e => e.UpazillalId);

            modelBuilder.Entity<Upazila>()
                .HasMany(e => e.Schools)
                .WithRequired(e => e.Upazila)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ActionLogs)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.Who);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserPermissions)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);
        }
    }
}