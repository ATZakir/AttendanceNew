namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmployeeAttendanceModel
    {
        public Guid Id { get; set; }

        public int? EmployeeId { get; set; }
        public int? SchoolId { get; set; }
        public int DivisionId { get; set; }
        public int DistrictId { get; set; }
        public int UpazilaId { get; set; }

        public string Date { get; set; }

        public short? Type { get; set; }

        public TimeSpan? Time { get; set; }

        public int? RemarkId { get; set; }

        public string Note { get; set; }

        public string AttendanceRemarkName { get; set; }

        public string EmployeeName { get; set; }
        public string SchoolName { get; set; }
        public string GeoName { get; set; }
    }
}
