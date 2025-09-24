using System.ComponentModel;

namespace CRM_Homestay.Core.Enums;

public enum MaintenanceStatuses
{
    [Description("Chờ thực hiện")]
    Pending = 0,
    [Description("Đang thực hiện")]    
    InProgress = 1,
    [Description("Đã hoàn thành")]
    Completed = 2,
    [Description("Đã hủy")]
    Canceled = 3,
}


