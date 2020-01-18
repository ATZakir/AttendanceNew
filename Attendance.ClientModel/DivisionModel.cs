namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class DivisionModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}
