using System;

namespace Attendance.ClientModel
{    
    public class WorkflowactionSettingModel
    {
        public Guid Id { get; set; }

        public int? SubMouduleItemId { get; set; }

        public int? UserId { get; set; }

        public int? WorkflowactionId { get; set; }

        public string SubModuleItemName { get; set; }

        public string UserName { get; set; }

        public string WorkflowactionName { get; set; }
    }
}
