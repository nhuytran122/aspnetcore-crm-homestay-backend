using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Core.Models;

public class DiscountData
{
    public DiscountTypes? MembershipDiscountType { get; set; }
    public decimal MembershipDiscountValue { get; set; } = 0;
    public string? VoucherCode { get; set; }
    public DiscountTypes? VoucherType { get; set; }
    public decimal VoucherValue { get; set; } = 0;
    public decimal TotalDiscount => MembershipDiscountValue + VoucherValue;
    public decimal OriginalPrice { get; set; } = 0;
}
