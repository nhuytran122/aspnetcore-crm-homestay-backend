using System.Net;
using AutoMapper;
using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.Coupons;
using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Core.Helpers;
using CRM_Homestay.Database.Helper;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.Bookings;
using CRM_Homestay.Entity.Coupons;
using CRM_Homestay.Entity.Customers;
using CRM_Homestay.Entity.SystemSettings;
using CRM_Homestay.Localization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace CRM_Homestay.Service.Coupons;

public class CouponService : BaseService, ICouponService
{
    public CouponService([NotNull] IUnitOfWork unitOfWork, [NotNull] IMapper mapper, [NotNull] ILocalizer l) :
        base(unitOfWork, mapper, l)
    {
    }

    public async Task<PagedResultDto<CouponDto>> GetListAsync(CouponFilterDto filter)
    {
        string searchTerm = !string.IsNullOrEmpty(filter.Text) ? $" {NormalizeString.ConvertNormalizeString(filter.Text.Trim())} " : string.Empty;

        var pagedResult = await _unitOfWork.GenericRepository<Coupon>().GetQueryable()
            .WhereIf(filter.IsActive != null, x => x.IsActive == filter.IsActive)
            .WhereIf(!string.IsNullOrEmpty(filter.Text),
                x => x.Code!.Contains(searchTerm) ||
                x.Code.Contains(filter.Text!.Trim().ToUpper()))
            .WhereIf(filter.DiscountType != null,
                x => x.DiscountType == filter.DiscountType)
            .OrderByDescending(x => x.StartDate)
            .ThenByDescending(x => x.TotalUsageLimit)
            .ThenByDescending(x => x.UsedCount)
            .GetPaged(filter.PageIndex, filter.PageSize);
        return ObjectMapper.Map<PagedResult<Coupon>, PagedResultDto<CouponDto>>(pagedResult);
    }

    public async Task<CouponDto> GetByIdAsync(Guid id)
    {
        var coupon = await _unitOfWork.GenericRepository<Coupon>().GetQueryable().AsNoTracking()
                                                            .Where(x => x.Id == id && x.IsActive)
                                                            .FirstOrDefaultAsync();
        if (coupon == null)
        {
            throw new GlobalException(code: CouponErrorCode.NotFound,
                        message: L[CouponErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
        }
        var couponDto = ObjectMapper.Map<Coupon, CouponDto>(coupon);
        return couponDto!;
    }

    public async Task<CouponDto> GetByCodeAsync(string code)
    {
        code = code.Trim().ToUpper();
        var coupon = await _unitOfWork.GenericRepository<Coupon>().GetQueryable().AsNoTracking()
                                                            .Where(x => x.Code == code && x.IsActive)
                                                            .FirstOrDefaultAsync();
        if (coupon == null)
        {
            throw new GlobalException(code: CouponErrorCode.NotFound,
                        message: L[CouponErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
        }
        var couponDto = ObjectMapper.Map<Coupon, CouponDto>(coupon);
        return couponDto!;
    }

    public async Task<CouponDto> CreateAsync(CreateUpdateCouponDto input)
    {
        if (input.StartDate.HasValue && input.StartDate.Value < DateTime.Now)
        {
            throw new GlobalException(code: BaseErrorCode.StartDateCannotBeInThePast,
                        message: L[BaseErrorCode.StartDateCannotBeInThePast],
                        statusCode: HttpStatusCode.BadRequest);
        }

        var code = await GenerateCode();

        var coupon = ObjectMapper.Map<CreateUpdateCouponDto, Coupon>(input);
        coupon.Code = code;

        await _unitOfWork.GenericRepository<Coupon>().AddAsync(coupon);
        await _unitOfWork.SaveChangeAsync();
        return ObjectMapper.Map<Coupon, CouponDto>(coupon);
    }

    public async Task<CouponDto> UpdateAsync(CreateUpdateCouponDto input, Guid id)
    {
        var coupon = await _unitOfWork.GenericRepository<Coupon>().GetAsync(x => x.Id == id);
        if (coupon == null)
        {
            throw new GlobalException(code: CouponErrorCode.NotFound,
                        message: L[CouponErrorCode.NotFound],
                        statusCode: HttpStatusCode.BadRequest);
        }
        if (input.TotalUsageLimit < coupon.UsedCount)
        {
            throw new GlobalException(
                code: CouponErrorCode.InvalidTotalUsageLimit,
                message: L[CouponErrorCode.InvalidTotalUsageLimit],
                statusCode: HttpStatusCode.BadRequest
            );
        }
        // coupon.DiscountType = input.DiscountType;
        // coupon.DiscountValue = input.DiscountValue;
        // coupon.TotalUsageLimit = input.TotalUsageLimit;
        // coupon.IsActive = input.IsActive;
        // coupon.StartDate = input.StartDate;
        // coupon.EndDate = input.EndDate;
        // coupon.Description = input.Description;
        // coupon.CustomerId = input.CustomerId;
        coupon = ObjectMapper.Map(input, coupon);
        coupon.Id = id;
        _unitOfWork.GenericRepository<Coupon>().Update(coupon);
        await _unitOfWork.SaveChangeAsync();
        return ObjectMapper.Map<Coupon, CouponDto>(coupon);
    }

    public async Task DeleteAsync(Guid id)
    {
        var coupon = await _unitOfWork.GenericRepository<Coupon>().GetAsync(x => x.Id == id);
        if (coupon == null)
        {
            throw new GlobalException(code: CouponErrorCode.NotFound,
                        message: L[CouponErrorCode.NotFound],
                        statusCode: HttpStatusCode.BadRequest);
        }
        _unitOfWork.GenericRepository<Coupon>().Remove(coupon);
        await _unitOfWork.SaveChangeAsync();
    }

    public async Task<ApplyCouponResultDto> ApplyCoupon(ApplyCouponRequestDto input)
    {
        var total = input.Total;
        var code = input.Code?.Trim().ToUpper();
        var customerId = input.CustomerId;
        if (total <= 0)
        {
            throw new GlobalException(code: BookingErrorCode.MoneyMustGreaterThanZero,
                        message: L[BookingErrorCode.MoneyMustGreaterThanZero],
                        statusCode: HttpStatusCode.BadRequest);
        }

        var repo = _unitOfWork.GenericRepository<Coupon>();
        var now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        var coupon = await repo.GetAsync(x =>
                                x.Code == code &&
                                x.IsActive &&
                                (!x.StartDate.HasValue || x.StartDate.Value <= now)
                            );
        if (coupon == null)
        {
            throw new GlobalException(code: CouponErrorCode.NotFound,
                        message: L[CouponErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
        }

        if (coupon.EndDate.HasValue && coupon.EndDate.Value < now)
        {
            throw new GlobalException(code: CouponErrorCode.CouponExpired,
                        message: L[CouponErrorCode.CouponExpired],
                        statusCode: HttpStatusCode.BadRequest);
        }

        if (coupon.CustomerId.HasValue)
        {
            if (coupon.CustomerId.Value != customerId)
            {
                throw new GlobalException(code: CouponErrorCode.NotFound,
                        message: L[CouponErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            // if (coupon.UsedCount > 0)
            // {
            //     throw new GlobalException(code: CouponErrorCode.CouponAlreadyUsedByCustomer,
            //             message: L[CouponErrorCode.CouponAlreadyUsedByCustomer],
            //             statusCode: HttpStatusCode.BadRequest);
            // }
        }

        bool isSameOrder = false;
        if (input.BookingId != null && input.BookingId != Guid.Empty && !string.IsNullOrEmpty(coupon.Code))
        {
            // Kiểm tra xem đơn này đã dùng mã này chưa
            isSameOrder = await _unitOfWork.GenericRepository<Booking>().GetQueryable().AsNoTracking().AnyAsync(x => x.Id == input.BookingId
            && !string.IsNullOrEmpty(x.DiscountData!.VoucherCode)
            && x.DiscountData!.VoucherCode.ToLower() == coupon.Code.ToLower());
        }

        // Check lượt sử dụng tổng nếu không phải đơn cũ đang dùng mã này
        if (!isSameOrder)
        {
            var copyUsedCount = coupon.UsedCount;
            if (coupon.TotalUsageLimit.HasValue && (++copyUsedCount > coupon.TotalUsageLimit.Value))
            {
                throw new GlobalException(code: CouponErrorCode.CouponExhausted,
                            message: L[CouponErrorCode.CouponExhausted],
                            statusCode: HttpStatusCode.BadRequest);
            }
        }

        decimal finalAmount = coupon.DiscountType == DiscountTypes.FixedAmount ?
            Math.Max(total - coupon.DiscountValue, 0) : total * (100 - coupon.DiscountValue) / 100;

        var result = new ApplyCouponResultDto()
        {
            OriginalTotal = total,
            SubTotal = finalAmount,
            CouponPrice = total - finalAmount,
            CouponCode = coupon.Code!,
            DiscountType = coupon.DiscountType,
            DiscountValue = coupon.DiscountValue
        };
        if (input.IsFromCart)
            return result;

        coupon.UsedCount++;

        repo.Update(coupon);
        return result;
    }

    public async Task DecreaseUsedCountAsync(string code)
    {
        var repo = _unitOfWork.GenericRepository<Coupon>();
        code = code.Trim().ToUpper();
        var coupon = await repo.GetAsync(x => x.Code == code);
        if (coupon == null)
        {
            return;
        }
        coupon.UsedCount--;
        repo.Update(coupon);
    }

    public async Task<List<CouponDto>> GetAvailableCouponsForCustomerAsync(Guid customerId)
    {
        var now = DateTime.Now;

        var coupons = await _unitOfWork.GenericRepository<Coupon>()
            .GetQueryable()
            .AsNoTracking()
            .Where(x =>
                x.IsActive &&
                (!x.StartDate.HasValue || x.StartDate <= now) &&
                (!x.EndDate.HasValue || x.EndDate >= now) &&
                x.UsedCount < x.TotalUsageLimit &&
                (x.CustomerId == null || x.CustomerId == customerId)
            )
            .OrderByDescending(x => x.CustomerId == customerId)
            .ThenByDescending(x => x.DiscountValue)
            .ThenBy(x => x.EndDate ?? DateTime.MaxValue)
            .ThenByDescending(x => x.CreationTime)
            .ToListAsync();

        return ObjectMapper.Map<List<CouponDto>>(coupons);
    }

    public async Task CreateIncentiveCouponBasedOnOrderAsync(Guid customerId, string? description)
    {
        var configSettingRepo = _unitOfWork.GenericRepository<SystemSetting>();

        var configDiscountType = await configSettingRepo.GetQueryable().AsNoTracking()
                .Where(x => x.SystemName == ConfigKey.IncentiveCoupon && x.ConfigKey == ConfigKey.DiscountType)
                .FirstOrDefaultAsync();
        if (configDiscountType == null)
            throw new GlobalException(code: SystemSettingErrorCode.NotFound,
                            message: L[SystemSettingErrorCode.NotFound],
                            statusCode: HttpStatusCode.BadRequest);
        var discountType = Enum.Parse<DiscountTypes>(configDiscountType.ConfigValue);

        var configDiscountValue = await configSettingRepo.GetQueryable().AsNoTracking()
                    .Where(x => x.SystemName == ConfigKey.IncentiveCoupon && x.ConfigKey == ConfigKey.DiscountValue)
                    .FirstOrDefaultAsync();

        if (configDiscountValue == null)
            throw new GlobalException("Chưa cấu hình giá trị giảm giá cho mã giảm giá", HttpStatusCode.NotFound);

        int discountValue = int.Parse(configDiscountValue.ConfigValue);
        var couponRepo = _unitOfWork.GenericRepository<Coupon>();
        var code = await GenerateCode();
        var coupon = new Coupon()
        {
            Code = code,
            CustomerId = customerId,
            DiscountType = discountType,
            DiscountValue = discountValue,
            TotalUsageLimit = 1,
            Description = description,
            IsActive = true,
        };
        await couponRepo.AddAsync(coupon);
    }

    private async Task<string> GenerateCode()
    {
        string code = RandomCodeHelper.GenerateCouponCode();
        var count = await _unitOfWork.GenericRepository<Coupon>().GetCountAsync();
        code = $"{code}{count + 1:D3}";
        return code;
    }

}