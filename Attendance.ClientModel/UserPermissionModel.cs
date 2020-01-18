namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserPermissionModel
    {
        public UserPermissionModel()
        {
            DistrictList = new List<DistrictModel>();
            UpazilaList = new List<UpazilaModel>();
            SchoolList = new List<SchoolModel>();
        }
        public Guid Id { get; set; }

        public int UserId { get; set; }
        public int DivisionId { get; set; }
        public int DistrictId { get; set; }
        public int UpazilaId { get; set; }
        public int SchoolId { get; set; }
                
        public string DivisionName { get; set; }
        public string DistrictName { get; set; }
        public string UpazilaName { get; set; }
        public string SchoolName { get; set; }
        public List<DistrictModel> DistrictList { get; set; }
        public List<UpazilaModel> UpazilaList { get; set; }
        public List<SchoolModel> SchoolList { get; set; }

    }
}
