using System.ComponentModel;

namespace CRM_Homestay.Core.Enums;

public enum RoomStatuses
{
    [Description("Đã đặt")]
    Booked = 1,
    [Description("Đang dọn dẹp")]
    Cleaning = 2,
    [Description("Đang bảo trì")]
    Maintaining = 3,
}


