using System.ComponentModel;

namespace CRM_Homestay.Core.Enums;

public enum DiscountTypes
{
    [Description("Giảm giá phần trăm")]
    Percentage = 1,
    [Description("Giảm giá số tiền cố định")]
    FixedAmount = 2
}