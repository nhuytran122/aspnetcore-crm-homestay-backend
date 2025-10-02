using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.Customers;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Models;

namespace CRM_Homestay.Entity.Coupons;

public class Coupon : BaseEntity, IAuditable
{
    public string Code { get; set; } = string.Empty;

    public DiscountTypes DiscountType { get; set; }

    public decimal DiscountValue { get; set; }

    public int? TotalUsageLimit { get; set; } = 1;

    public int UsedCount { get; set; } = 0;

    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? CustomerId { get; set; }

    public Customer? Customer { get; set; }

}