using System.ComponentModel;

namespace CRM_Homestay.Core.Enums;

public enum TransactionTypes
{
    [Description("Ứng tiền")]
    AdvanceMoney,

    [Description("Trả tiền")]
    Pay,

    [Description("Hoàn trả tiền ứng")]
    PayAdvance,

    [Description("Thanh toán cọc")]
    Deposit,

    [Description("Thanh toán phòng/dịch vụ còn lại")]
    PayForBooking,

    [Description("Thanh toán dịch vụ bổ sung")]
    ServiceMoney,

    [Description("Thanh toán đặt phòng")]
    MoneyOfBooking,

    [Description("Thu nợ khách hàng")]
    DebtCustomer,

    [Description("Hoàn tiền")]
    Refund,

    [Description("Chuyển khoản nội bộ")]
    InternalTransfer
}