using System.Collections.Generic;

namespace Attendance.ClientModel
{
    using System;

    public class UserModel
    {
        public UserModel()
        {
            UserPermissions = new List<UserPermissionModel>();

        }
        public int Id { get; set; }

        public string LoginName { get; set; }

        public string Password { get; set; }

        public int? RoleId { get; set; }

        public int? Level { get; set; }

        public string Name { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string CreationDateTime { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? PwdTimeStamp { get; set; }

        public string RoleName { get; set; }

        public string ShowName { get; set; }

        public int EmployeeId { get; set; }

        public int? SchoolId { get; set; }

        public int DivisionId { get; set; }

        public int DistrictId { get; set; }

        public int UpazilaId { get; set; }

        public List<UserPermissionModel> UserPermissions = new List<UserPermissionModel>();
    }
}
