namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class NoticeModel
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Validity { get; set; }

        public string AttachFile { get; set; }
    }
}
