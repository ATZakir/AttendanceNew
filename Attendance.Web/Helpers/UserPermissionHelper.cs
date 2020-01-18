using Attendance.Model;
using Attendance.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attendance.Web.Helpers
{
    public class UserPermissionHelper
    {
        public static List<int> GetSchoolIdByAccess(IUserPermissionService userPermissionService, ISchoolService schoolService)
        {
            int userId = UserSession.GetUserIdFromSession();
            List<int> schoolIds = new List<int>();
            List<School> schools = new List<School>();
            var userPerm = userPermissionService.GetUserPermissionByUserId(userId);
            var schoolAll = userPerm.Where(x => x.DivisionId == 0);
            if (schoolAll.Count() > 0)
            {
                schools = schoolService.GetAllSchool().Where(x => x.IsActive == true).ToList();
            }
            else
            {
                var divisionIds = userPerm.Select(x => x.DivisionId).Distinct();
                foreach (var divisionId in divisionIds)
                {
                    schoolAll = userPerm.Where(x => x.DivisionId == divisionId && x.DistrictId == 0);
                    if (schoolAll.Count() > 0)
                    {
                        schools.AddRange(schoolService.GetAllSchool().Where(x => x.IsActive == true && x.Upazila.District.DivisionId == divisionId));
                    }
                    else
                    {
                        var districtIds = userPerm.Where(x => x.DivisionId == divisionId).Select(x => x.DistrictId).Distinct();
                        foreach (var districtId in districtIds)
                        {
                            schoolAll = userPerm.Where(x => x.DivisionId == divisionId && x.DistrictId == districtId && x.UpazilaId == 0);
                            if (schoolAll.Count() > 0)
                            {
                                schools.AddRange(schoolService.GetAllSchool().Where(x => x.IsActive == true && x.Upazila.District.DivisionId == divisionId && x.Upazila.DistrictId == districtId));
                            }
                            else
                            {
                                var upazilaIds = userPerm.Where(x => x.DivisionId == divisionId && x.DistrictId == districtId).Select(x => x.UpazilaId).Distinct();
                                foreach (var upazilaId in upazilaIds)
                                {
                                    schools.AddRange(schoolService.GetAllSchool().Where(x => x.IsActive == true && x.Upazila.District.DivisionId == divisionId && x.Upazila.DistrictId == districtId && x.UpazilaId == upazilaId));
                                }
                            }
                        }
                    }
                }
            }

            return schools.Select(x => x.Id).Distinct().ToList();
        }
    }
}