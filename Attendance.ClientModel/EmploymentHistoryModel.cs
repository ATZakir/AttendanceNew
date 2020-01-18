namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;
   
    public partial class EmploymentHistoryModel
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int? DepartmentId { get; set; }

        public int? DesignationId { get; set; }

        public string DateFrom { get; set; }

        public string DateTo { get; set; }

        public int? DutyShiftId { get; set; }

        public int? SchoolId { get; set; }

        public string DepartmentName { get; set; }

        public string DesignationName { get; set; }

        public string DutyShiftName { get; set; }

        public string EmployeeName { get; set; }

        public string SchoolName { get; set; }
    }
}
