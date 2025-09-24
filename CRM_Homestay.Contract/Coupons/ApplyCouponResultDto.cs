using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.Coupons;

public class ApplyCouponResultDto
{
    public decimal OriginalTotal { get; set; }
    public decimal SubTotal { get; set; }
    public decimal CouponPrice { get; set; }
    public string CouponCode { get; set; } = string.Empty;
    public DiscountTypes DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
}
