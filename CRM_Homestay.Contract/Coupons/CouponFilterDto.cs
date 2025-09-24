using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.Coupons;

public class CouponFilterDto : FilterBase
{
    public bool? IsActive { get; set; }
    public DiscountTypes? DiscountType { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}