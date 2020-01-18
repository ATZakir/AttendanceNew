namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;

    public partial class DistrictModel
    {

        public int Id { get; set; }

        public int DivisionId { get; set; }

        public string Name { get; set; }

        public string DivisionName { get; set; }

        public bool? IsActive { get; set; }
    }
}
