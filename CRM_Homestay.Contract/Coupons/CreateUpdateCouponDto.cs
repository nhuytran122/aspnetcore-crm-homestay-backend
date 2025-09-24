using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.Coupons;

public class CreateUpdateCouponDto : BaseEntityDto
{
    public DiscountTypes DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public int TotalUsageLimit { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public Guid? CustomerId { get; set; }
}