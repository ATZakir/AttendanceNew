namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;

    public partial class DutyShiftModel
    {
        public int Id { get; set; }

        public int DutyShiftMasterId { get; set; }

        public string InTime { get; set; }

        public string MaxInTime { get; set; }

        public string OutTime { get; set; }

        public int? SchoolId { get; set; }
        public int DivisionId { get; set; }
        public int DistrictId { get; set; }
        public int UpazilaId { get; set; }

        public string DutyShiftMasterName { get; set; }

        public string SchoolName { get; set; }
        public string GeoName { get; set; }
    }
}
