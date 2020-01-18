namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;

    public partial class SchoolAttendanceSummaryModel
    {
        public SchoolAttendanceSummaryModel()
        {
            this.AttendenceList = new List<EmployeeAttendanceSummaryModel>();
        }

        public int SchoolId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public int TotalEmployee { get; set; }

        public int MaleEmployee { get; set; }

        public int FemaleEmployee { get; set; }

        public int PresentEmployee { get; set; }

        public int LatePresentEmployee { get; set; }

        public int AbsentEmployee { get; set; }

        public int EarlyOutEmployee { get; set; }

        public int EarlyOutLatePresentEmployee { get; set; }

        public int LeaveEmployee { get; set; }

        public int OnDutyEmployee { get; set; }

        public int InTrainingEmployee { get; set; }

        public double PresentPercent { get; set; }

        public List<EmployeeAttendanceSummaryModel> AttendenceList { get; set; }

    }

    public partial class EmployeeAttendanceSummaryModel
    {
        public long Id { get; set; }

        public int EmployeeId { get; set; }

        public int? SchoolId { get; set; }

        public int Year { get; set; }

        public short Month { get; set; }

        public string D1 { get; set; }

        public string D2 { get; set; }

        public string D3 { get; set; }

        public string D4 { get; set; }

        public string D5 { get; set; }

        public string D6 { get; set; }

        public string D7 { get; set; }

        public string D8 { get; set; }

        public string D9 { get; set; }

        public string D10 { get; set; }

        public string D11 { get; set; }

        public string D12 { get; set; }

        public string D13 { get; set; }

        public string D14 { get; set; }

        public string D15 { get; set; }

        public string D16 { get; set; }

        public string D17 { get; set; }

        public string D18 { get; set; }

        public string D19 { get; set; }

        public string D20 { get; set; }

        public string D21 { get; set; }

        public string D22 { get; set; }

        public string D23 { get; set; }

        public string D24 { get; set; }

        public string D25 { get; set; }

        public string D26 { get; set; }

        public string D27 { get; set; }

        public string D28 { get; set; }

        public string D29 { get; set; }

        public string D30 { get; set; }

        public string D31 { get; set; }

        public string EmployeeName { get; set; }

        public string SchoolName { get; set; }

        public int TP { get; set; }

        public int TA { get; set; }

        public int TL { get; set; }

        public double PP { get; set; }
    }

    public partial class EmployeeAttendanceLogModel
    {
        public long Id { get; set; }

        public int EmployeeId { get; set; }

        public int? SchoolId { get; set; }

        public string EmployeeName { get; set; }

        public string SchoolName { get; set; }

        public DateTime Date { get; set; }

        public string InTime { get; set; }

        public string OutTime { get; set; }

    }
}
