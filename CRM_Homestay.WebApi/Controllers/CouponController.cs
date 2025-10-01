using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.Coupons;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Homestay.App.Controllers;

/// <summary>
/// CouponController
/// </summary>
[ApiController]
[Route("api/coupons")]
[Authorize]
public class CouponController : BaseController
{
    private ICouponService _couponService;
    /// <summary>
    /// CouponController init
    /// </summary>
    /// <param name="couponService"></param>
    /// <param name="httpContextAccessor"></param>
    public CouponController(ICouponService couponService, [NotNull] IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        _couponService = couponService;
    }

    /// <summary>
    ///  Lấy ra danh sách mã giảm giá
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<PagedResultDto<CouponDto>> GetListAsync([FromQuery] CouponFilterDto filter)
    {
        return await _couponService.GetListAsync(filter);
    }

    /// <summary>
    /// Lấy chi tiết 1 mã
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>

    [HttpGet("{id}")]
    public async Task<CouponDto> GetById(Guid id)
    {
        return await _couponService.GetByIdAsync(id);
    }

    /// <summary>
    ///  Tạo mới mã giảm giá
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<CouponDto> CreateAsync(CreateUpdateCouponDto input)
    {
        return await _couponService.CreateAsync(input);
    }

    /// <summary>
    ///  Cập nhật mã giảm giá
    /// </summary>
    /// <param name="input"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("{id}")]
    public async Task<CouponDto> UpdateAsync(CreateUpdateCouponDto input, Guid id)
    {
        return await _couponService.UpdateAsync(input, id);
    }

    /// <summary>
    ///  Xóa mã giảm giá
    /// </summary>
    /// <param name="id"></param>
    [HttpDelete]
    [Route("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _couponService.DeleteAsync(id);
    }

    /// <summary>
    /// Áp mã giảm giá ước tính
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("estimate")]
    public async Task<ApplyCouponResultDto> ApplyCouponFromCart(ApplyCouponRequestDto input)
    {
        input.IsFromCart = true;
        return await _couponService.ApplyCoupon(input);
    }
}