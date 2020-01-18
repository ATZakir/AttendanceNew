namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;

    public partial class LeaveTypeModel
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string PaymentMode { get; set; }

        public int? PayDays { get; set; }

        public bool? CarryForward { get; set; }

        public string Remarks { get; set; }

        public bool? IsActive { get; set; }
    }
}
