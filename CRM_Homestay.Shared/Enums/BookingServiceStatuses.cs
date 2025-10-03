using System.ComponentModel;

namespace CRM_Homestay.Core.Enums;

public enum BookingServiceStatuses
{
    [Description("Chờ thanh toán")]
    Pending = 0,
    [Description("Đang phục vụ")]
    InProgress = 1,
    [Description("Đã hoàn thành")]
    Completed = 10,
    [Description("Đã hủy")]
    Cancelled = -10,
}


