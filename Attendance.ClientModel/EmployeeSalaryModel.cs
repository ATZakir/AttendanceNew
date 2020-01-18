namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmployeeSalaryModel
    {
        public Guid Id { get; set; }

        public int? SchoolId { get; set; }
        public int DivisionId { get; set; }
        public int DistrictId { get; set; }
        public int UpazilaId { get; set; }
        public int? EmployeeId { get; set; }

        public string SalaryAccount { get; set; }

        public string GPFNo { get; set; }

        public string MBNo { get; set; }

        public string SalaryScale { get; set; }

        public string BasicSalary { get; set; }

        public string EmployeeName { get; set; }
        public string SchoolName { get; set; }
        public string GeoName { get; set; }
    }
}
