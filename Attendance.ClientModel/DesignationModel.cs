namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;

    public partial class DesignationModel
    {
    
        public int Id { get; set; }

        public int DepartmentId { get; set; }

        public string Name { get; set; }

        public string DepartmentName { get; set; }
        
        public bool? IsActive { get; set; }

        public bool? SchoolOnly { get; set; }
    }
}
