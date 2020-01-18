namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class SchoolModel
    {    
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public int DivisionId { get; set; }
        public int DistrictId { get; set; }
        public int UpazilaId { get; set; }

        public string Phone { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string EIN { get; set; }

        public bool? IsActive { get; set; }

        public string DivisionName { get; set; }
        public string DistrictName { get; set; }
        public string UpazilaName { get; set; }
    }
}
