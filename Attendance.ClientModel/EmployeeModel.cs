namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmployeeModel: EmployeeBaseModel
    {
        public EmployeeModel()
        {
            EmploymentHistories = new List<EmploymentHistoryModel>();
            AdminEmploymentHistories = new List<AdminEmploymentHistoryModel>();
            EmployeeEducations = new List<EmployeeEducationModel>();
            EmployeeLeaveBalances = new List<EmployeeLeaveBalanceModel>();
        }

        public string PresentAddress { get; set; }

        public string PermanentAddress { get; set; }

        public string FatherName { get; set; }

        public string MotherName { get; set; }

        public string SpouseName { get; set; }

        public string NationalId { get; set; }

        public string PassportNo { get; set; }

        public string OfficePhone { get; set; }

        public string OfficeMobile { get; set; }

        public string ResidentPhone { get; set; }

        public string ResidentMobile { get; set; }

        public string BloodGroup { get; set; }

        public DateTime? JoinDate { get; set; }

        public string Gender { get; set; }

        public string MaritalStatus { get; set; }

        public string Relgion { get; set; }

        public string Nationality { get; set; }

        public DateTime? DOB { get; set; }

        public int? LastDepartmentId { get; set; }

        public int? LastDesignationId { get; set; }

        public int? LastDutyShiftId { get; set; }

        public int? LastSchoolId { get; set; }

        public int? LastDivisionId { get; set; }
        public int? LastDistrictId { get; set; }
        public int? LastUpazilaId { get; set; }
        public bool IsSchoolAdmin { get; set; }

        public List<EmploymentHistoryModel> EmploymentHistories = new List<EmploymentHistoryModel>();
        public List<AdminEmploymentHistoryModel> AdminEmploymentHistories = new List<AdminEmploymentHistoryModel>();
        public List<EmployeeEducationModel> EmployeeEducations = new List<EmployeeEducationModel>();
        public List<EmployeeLeaveBalanceModel> EmployeeLeaveBalances { get; set; }

    }


    public class EmployeeBaseModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Designation { get; set; }

        public string Department { get; set; }

        public string PhotoPath { get; set; }

        public int? SchoolId { get; set; }

        public int? DivisionId { get; set; }

        public int? DistrictId { get; set; }

        public int? UpazilaId { get; set; }
    }

}
