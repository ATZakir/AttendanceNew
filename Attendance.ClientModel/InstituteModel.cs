namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class InstituteModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int DistrictId { get; set; }

        public string DistrictName { get; set; }

    }
}
