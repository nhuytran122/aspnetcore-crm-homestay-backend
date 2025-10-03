using System.ComponentModel;

namespace CRM_Homestay.Core.Enums;

public enum BookingStatuses
{
    [Description("Chờ thanh toán")]
    Pending = 0,
    [Description("Đã xác nhận")]
    Confirmed = 1,
    [Description("Đã hoàn tất")]
    Completed = 10,
    [Description("Đã hủy")]
    Cancelled = -10,
}


