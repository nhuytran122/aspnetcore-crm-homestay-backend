using System.ComponentModel;

namespace CRM_Homestay.Core.Enums;

public enum OtpStatuses
{
    [Description("Khởi tạo")]
    Init = 0,
    [Description("Chờ xác thực")]
    Pending = 1,
    [Description("Đã xác thực")]
    Verified = 2,
    [Description("Đã hết hạn")]
    Expired = 3,
    [Description("Xác thực không thành công")]
    Failed = 4,
    [Description("Đã sử dụng")]
    Used = 10
}
