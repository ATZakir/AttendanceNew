namespace Attendance.ClientModel
{
    using System;
    
    public class NotificationSettingModel
    {
        public Guid Id { get; set; }

        public int? SubModuleItemId { get; set; }

        public int? NotifiedUserId { get; set; }

        public int? WorkflowactionId { get; set; }

        public string SubModuleItemName { get; set; }

        public string UserName { get; set; }

        public string WorkflowactionName { get; set; }
    }
}
