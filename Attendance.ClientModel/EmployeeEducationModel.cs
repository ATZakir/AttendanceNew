namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmployeeEducationModel
    {
        public long Id { get; set; }

        public int EmployeeId { get; set; }

        public int LevelId { get; set; }

        public string DegreeTitle { get; set; }

        public string DegreeMajor { get; set; }

        public string Result { get; set; }

        public int BoardOrUniversityId { get; set; }

        public int? InstituteId { get; set; }

        public string Scale { get; set; }

        public string BoardorUniversityName { get; set; }

        public string EducationLevel { get; set; }

        public string EmployeeName { get; set; }

        public string InstituteName { get; set; }
    }
}
