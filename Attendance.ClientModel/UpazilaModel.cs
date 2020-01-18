namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class UpazilaModel
    {
        public int Id { get; set; }

        public int DistrictId { get; set; }

        public string Name { get; set; }

        public string DistrictName { get; set; }

        public int DivisionId { get; set; }

        public string DivisionName { get; set; }

        public bool? IsActive { get; set; }
    }
}
