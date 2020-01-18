namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class DutyEntryModel
    {
        public Guid Id { get; set; }

        public DateTime? Date { get; set; }
        public int? SchoolId { get; set; }
        public int DivisionId { get; set; }
        public int DistrictId { get; set; }
        public int UpazilaId { get; set; }
        public int? EmployeeId { get; set; }

        public TimeSpan? InTime { get; set; }
        public string InTimeString { get; set; }

        public TimeSpan? OutTime { get; set; }
        public string OutTimeString { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public int? ReasonId { get; set; }

        public string Remarks { get; set; }

        public string Location { get; set; }

        public DateTime? RemovedOn { get; set; }

        public int? RemovedBy { get; set; }

        public string DutyReasonName { get; set; }

        public string EmployeeName { get; set; }
        public string SchoolName { get; set; }
        public string GeoName { get; set; }
    }
}
