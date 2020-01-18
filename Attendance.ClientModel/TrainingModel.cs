namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;
   
    public partial class TrainingModel
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public int? SchoolId { get; set; }
        public int DivisionId { get; set; }
        public int DistrictId { get; set; }
        public int UpazilaId { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string Description { get; set; }

        public short? Status { get; set; }

        public string RemovedOn { get; set; }

        public int? RemovedBy { get; set; }

        public int? TypeId { get; set; }

        public string TypeName { get; set; }

        public string EmployeeName { get; set; }
        public string SchoolName { get; set; }
        public string GeoName { get; set; }
    }
}
