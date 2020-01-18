using System;
using System.Collections.Generic;

namespace Attendance.ClientModel
{
    public class RoleModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public bool? IsActive { get; set; }
    }
}
