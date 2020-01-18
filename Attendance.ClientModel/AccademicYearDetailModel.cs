namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;

    public partial class AccademicYearDetailModel
    {
        public int AccademicYear { get; set; }

        public string DayDate { get; set; }

        public int DayType { get; set; }

        public string DayTypeName { get; set; }

        public string DayDescription { get; set; }
    }
}
