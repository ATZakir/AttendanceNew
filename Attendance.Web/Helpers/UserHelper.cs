using Attendance.ClientModel;
using Attendance.Model;
using Attendance.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Results;

namespace Attendance.Web.Helpers
{
    public class UserHelper
    {
        public static UserModel PrepareUserModel(IRoleService roleService, User aUser, IDistrictService districtService = null, IUpazilaService upazilaService = null, ISchoolService schoolService = null )
        {
            UserModel um = new UserModel();

            um.Id = aUser.Id;
            um.LoginName = aUser.LoginName;
            um.Password = aUser.Password;
            um.Email = aUser.Email;
            um.Name = aUser.Employee.FullName;
            um.Email = aUser.Employee.Email;
            um.RoleId = aUser.RoleId;
            if (aUser.EmployeeId > 0) um.EmployeeId = aUser.EmployeeId;
            if (aUser.RoleId > 0)
            {
                var role = roleService.GetRole(Convert.ToInt32(aUser.RoleId));
                um.RoleName = role.Name;
                um.Level = role.Level;
            }

            var lastEmpHistory = aUser.Employee.EmploymentHistories
                .FirstOrDefault(x => x.DateTo == null);
            var lastAdminEmpHistory = aUser.Employee.AdminEmploymentHistories
                .FirstOrDefault(x => x.DateTo == null);
            if (lastEmpHistory != null)
            {
                um.SchoolId = lastEmpHistory.SchoolId;
            }

            if (lastAdminEmpHistory != null)
            {
                if (lastAdminEmpHistory.DivisionId != null) 
                    um.DivisionId = (int) lastAdminEmpHistory.DivisionId;
                if (lastAdminEmpHistory.DistrictlId != null) 
                    um.DistrictId = (int) lastAdminEmpHistory.DistrictlId;
            }
            um.IsActive = aUser.IsActive;
            if (districtService != null && upazilaService != null && schoolService != null)
            {
                if (aUser.UserPermissions != null)
                {
                    List<UserPermissionModel> userPermissionVMList = new List<UserPermissionModel>();
                    foreach (var userPermission in aUser.UserPermissions.OrderBy(a => a.UserId))
                    {
                        UserPermissionModel userTemp = new UserPermissionModel();
                        userTemp.Id = userPermission.Id;
                        userTemp.UserId = userPermission.UserId;
                        userTemp.DivisionId = userPermission.DivisionId;

                        var districtListObj = districtService.GetAllDistrict().Where(x => x.IsActive == true && x.DivisionId == userTemp.DivisionId || x.Id == 0);
                        foreach (var district in districtListObj)
                        {
                            DistrictModel districtTemp = new DistrictModel();
                            districtTemp.Id = district.Id;
                            districtTemp.Name = district.Name;
                            userTemp.DistrictList.Add(districtTemp);
                        }
                        userTemp.DistrictId = userPermission.DistrictId;
                        var upazilaListObj = upazilaService.GetAllUpazila().Where(x => x.DistrictId == userTemp.DistrictId && x.IsActive == true || x.Id == 0);

                        foreach (var upazila in upazilaListObj)
                        {
                            UpazilaModel upazilaTemp = new UpazilaModel();
                            upazilaTemp.Id = upazila.Id;
                            upazilaTemp.Name = upazila.Name;
                            userTemp.UpazilaList.Add(upazilaTemp);
                        }

                        userTemp.UpazilaId = userPermission.UpazilaId;
                        var schoolListObj = schoolService.GetAllSchool().Where(x => x.UpazilaId == userTemp.UpazilaId || x.Id == 0);

                        foreach (var school in schoolListObj)
                        {
                            SchoolModel schoolTemp = new SchoolModel();
                            schoolTemp.Id = school.Id;
                            schoolTemp.Name = school.Name;
                            userTemp.SchoolList.Add(schoolTemp);
                        }
                        userTemp.SchoolId = userPermission.SchoolId;
                        userPermissionVMList.Add(userTemp);
                    }
                    um.UserPermissions = userPermissionVMList;
                }
            }
            
            return um;
        }


    }
}