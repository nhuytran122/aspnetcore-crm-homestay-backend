using System.ComponentModel;

namespace CRM_Homestay.Core.Enums;

public enum PaymentPurposes
{
    [Description("Thanh toán tiền phòng ban đầu")]
    RoomBooking = 0,
    [Description("Thanh toán dịch vụ ban đầu")]    
    PrepaidService = 1,
    [Description("Thanh toán dịch vụ phát sinh")]
    AdditionalService = 2,
    [Description("Thanh toán gia hạn giờ")]
    ExtendedHours = 3,
}


