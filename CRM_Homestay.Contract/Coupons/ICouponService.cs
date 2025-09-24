using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.Coupons;

public interface ICouponService
{
    public Task<PagedResultDto<CouponDto>> GetListAsync(CouponFilterDto filter);
    public Task<CouponDto> GetByIdAsync(Guid id);
    public Task<CouponDto> GetByCodeAsync(string code);
    public Task DecreaseUsedCountAsync(string code);
    public Task<CouponDto> CreateAsync(CreateUpdateCouponDto input);
    public Task<CouponDto> UpdateAsync(CreateUpdateCouponDto input, Guid id);
    public Task DeleteAsync(Guid id);
    public Task<ApplyCouponResultDto> ApplyCoupon(ApplyCouponRequestDto input);
    public Task<List<CouponDto>> GetAvailableCouponsForCustomerAsync(Guid customerId);
    public Task CreateIncentiveCouponBasedOnBookingAsync(Guid customerId, string? description);
}