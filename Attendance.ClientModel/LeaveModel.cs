namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;
   
    public partial class LeaveModel
    {
        public string Id { get; set; }

        public int LeaveTypeId { get; set; }

        public int? SchoolId { get; set; }
        public int DivisionId { get; set; }
        public int DistrictId { get; set; }
        public int UpazilaId { get; set; }
        public int EmployeeId { get; set; }

        public DateTime Date { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string LeaveReason { get; set; }

        public string ContactDuringLeave { get; set; }

        public string AddressDuringLeave { get; set; }

        public short? Status { get; set; }

        public DateTime? RemovedOn { get; set; }

        public int? RemovedBy { get; set; }

        public int? NoofDays { get; set; }

        public string EmployeeName { get; set; }

        public string LeaveTypeName { get; set; }
        public string SchoolName { get; set; }
        public string GeoName { get; set; }
    }
}
