using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.ClientModel
{
    public class NoticeDetailModel 
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Validity { get; set; }

        public string AttachFile { get; set; }

        public string FileType { get; set; }
    }
}
