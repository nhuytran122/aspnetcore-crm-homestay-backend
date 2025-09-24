using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Core.Enums;
namespace CRM_Homestay.Contract.Coupons;

public class CouponDto : BaseEntityDto
{
    public string? Code { get; set; }
    public DiscountTypes DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public int? TotalUsageLimit { get; set; }
    public int UsedCount { get; set; }
    public int? RemainingQuantity => TotalUsageLimit != null ? Math.Max(TotalUsageLimit.Value - UsedCount, 0) : null;
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? CustomerId { get; set; }
}