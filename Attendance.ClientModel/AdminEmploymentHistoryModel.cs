namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;

    public partial class AdminEmploymentHistoryModel
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int? DepartmentId { get; set; }

        public int? DesignationId { get; set; }

        public string DateFrom { get; set; }

        public string DateTo { get; set; }

        public int? DivisionId { get; set; }

        public int? DistrictId { get; set; }

        public int? UpazilaId { get; set; }

        public string DepartmentName { get; set; }

        public string DesignationName { get; set; }

        public string EmployeeName { get; set; }

        public string DivisionName { get; set; }

        public string DistrictName { get; set; }

        public string UpazilaName { get; set; }
    }
}
