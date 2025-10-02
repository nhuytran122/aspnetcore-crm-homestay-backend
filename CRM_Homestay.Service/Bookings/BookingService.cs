using AutoMapper;
using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Localization;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Volo.Abp.DependencyInjection;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Contract.Bookings;
using CRM_Homestay.Entity.Bookings;
using CRM_Homestay.Entity.RoomPricings;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Entity.RoomUsages;
using CRM_Homestay.Entity.BookingRooms;
using CRM_Homestay.Entity.Rooms;
using CRM_Homestay.Entity.Customers;
using CRM_Homestay.Core.Models;
using CRM_Homestay.Contract.Coupons;
using CRM_Homestay.Contract.RoomUsages;
using CRM_Homestay.Contract.Amenities;
using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Database.Helper;
using System.Linq.Dynamic.Core;
using CRM_Homestay.Contract.Reviews;
using CRM_Homestay.Contract.SystemSettings;
using CRM_Homestay.Core.Extensions;

namespace CRM_Homestay.Service.Amenities
{
    public class BookingService : BaseService, IBookingService, ITransientDependency
    {
        private readonly ICouponService _couponService;
        private readonly ISystemSettingService _systemSettingService;
        public BookingService(ICouponService couponService, ISystemSettingService systemSettingService, IUnitOfWork unitOfWork, IMapper mapper, ILocalizer l)
            : base(unitOfWork, mapper, l)
        {
            _couponService = couponService;
            _systemSettingService = systemSettingService;
        }

        #region Create Booking
        public async Task<BookingDto> CreateAsync(CreateBookingDto input)
        {
            var customer = await GetCustomerAsync(input.CustomerId);
            var hasPendingBooking = await _unitOfWork.GenericRepository<Booking>().AnyAsync(
                                        b => b.CustomerId == input.CustomerId && b.Status == BookingStatuses.Pending
                                    );
            if (hasPendingBooking)
            {
                throw new GlobalException(
                    code: BookingErrorCode.PendingBookingExists,
                    message: L[BookingErrorCode.PendingBookingExists],
                    statusCode: HttpStatusCode.BadRequest
                );
            }

            await ValidateBookingRoomsAsync(input);

            using (_unitOfWork.BeginTransaction())
            {
                try
                {
                    var booking = await CreateBookingEntityAsync(input);

                    await AddRoomUsagesAndPricingAsync(booking, input.CheckIn, input.CheckOut);

                    await ApplyDiscountAndCouponAsync(booking, customer, input.CouponCode);

                    await _unitOfWork.SaveChangeAsync();
                    _unitOfWork.Commit();

                    return ObjectMapper.Map<Booking, BookingDto>(booking);
                }
                catch (Exception e)
                {
                    _unitOfWork.Rollback();
                    throw new GlobalException(
                        code: BaseErrorCode.UnexpectedError,
                        message: e.Message,
                        statusCode: HttpStatusCode.BadRequest
                    );
                }
            }
        }
        #endregion
        private async Task<Booking> CreateBookingEntityAsync(CreateBookingDto input)
        {
            var booking = new Booking
            {
                CheckIn = input.CheckIn,
                CheckOut = input.CheckOut,
                CustomerId = input.CustomerId,
                TotalGuests = input.BookingRooms.Sum(br => br.GuestCounts) ?? 1,
                BookingParentId = input.BookingParentId,
            };

            booking.BookingRooms = input.BookingRooms.Select(br => new BookingRoom
            {
                RoomId = br.RoomId!.Value,
                GuestCounts = br.GuestCounts ?? 1,
                BookingId = booking.Id
            }).ToList();

            await _unitOfWork.GenericRepository<Booking>().AddAsync(booking);
            return booking;
        }

        private async Task AddRoomUsagesAndPricingAsync(Booking booking, DateTime checkIn, DateTime checkOut)
        {
            var cleaningMinutes = await _systemSettingService.GetCleaningMinutesAsync();
            var roomUsages = new List<RoomUsage>();
            decimal totalPrice = 0;
            var bookingRooms = booking.BookingRooms;
            if (bookingRooms != null && bookingRooms.Any())
            {
                foreach (var br in bookingRooms)
                {
                    var usage = new RoomUsage
                    {
                        RoomId = br.RoomId,
                        BookingRoomId = br.Id,
                        StartAt = checkIn,
                        EndAt = checkOut,
                        Status = RoomStatuses.Booked
                    };
                    roomUsages.Add(usage);

                    var cleaningUsage = new RoomUsage
                    {
                        RoomId = br.RoomId,
                        BookingRoomId = br.Id,
                        StartAt = checkOut,
                        EndAt = checkOut.AddMinutes(cleaningMinutes),
                        Status = RoomStatuses.Cleaning
                    };
                    roomUsages.Add(cleaningUsage);

                    var room = await _unitOfWork.GenericRepository<Room>().GetAsync(x => x.Id == br.RoomId);
                    if (room != null)
                    {
                        var priceDto = await GetRoomPriceDetailAsync(room.RoomTypeId, checkIn, checkOut);
                        br.PricingSnapshot = priceDto.PricingSnapshot;
                        totalPrice += priceDto.BaseTotalPrice;
                    }
                }
            }
            booking.TotalPrice = totalPrice;
            await _unitOfWork.GenericRepository<RoomUsage>().AddRangeAsync(roomUsages);
        }

        private async Task ApplyDiscountAndCouponAsync(Booking booking, Customer customer, string? couponCode)
        {
            var discountData = new DiscountData
            {
                MembershipDiscountType = customer.Group?.DiscountType,
                MembershipDiscountValue = customer.Group?.DiscountValue ?? 0,
                OriginalPrice = booking.TotalPrice
            };

            if (!string.IsNullOrWhiteSpace(couponCode))
            {
                var couponResult = await _couponService.ApplyCoupon(new ApplyCouponRequestDto
                {
                    Total = booking.TotalPrice - discountData.MembershipDiscountValue,
                    Code = couponCode,
                    CustomerId = booking.CustomerId,
                    BookingId = booking.Id,
                    IsFromCart = false
                });

                discountData.VoucherCode = couponResult.CouponCode;
                discountData.VoucherType = couponResult.DiscountType;
                discountData.VoucherValue = couponResult.DiscountValue;

                booking.TotalPrice = couponResult.SubTotal;
            }
            else
            {
                var afterMembership = booking.TotalPrice - discountData.MembershipDiscountValue;
                booking.TotalPrice = afterMembership >= 0 ? afterMembership : 0;
            }
            booking.DiscountData = discountData;
        }

        private async Task ValidateBookingRoomsAsync(CreateBookingDto input)
        {
            var roomIds = input.BookingRooms.Select(a => a.RoomId).Where(id => id.HasValue).Select(id => id!.Value).ToList();

            var existingRooms = await _unitOfWork.GenericRepository<Room>()
                .GetQueryable()
                .Where(x => roomIds.Contains(x.Id))
                .Select(x => new { x.Id, x.RoomTypeId })
                .ToListAsync();

            var invalidRoomIds = roomIds.Except(existingRooms.Select(r => r.Id)).ToList();
            if (invalidRoomIds.Any())
            {
                throw new GlobalException(RoomErrorCode.NotFound, L[RoomErrorCode.NotFound], HttpStatusCode.BadRequest);
            }

            foreach (var br in input.BookingRooms)
            {
                if (!br.RoomId.HasValue) continue;

                var conflicts = await GetOverlappingUsagesAsync(br.RoomId.Value, input.CheckIn, input.CheckOut);
                if (conflicts.Any())
                {
                    var firstConflict = conflicts.First();
                    throw new GlobalException(
                        BookingErrorCode.RoomUnavailable,
                        string.Format(
                            L[BookingErrorCode.RoomUnavailable],
                            string.Join(", ", conflicts.Select(c => c.RoomNumber)),
                            firstConflict.StartAt.ToString("dd/MM/yyyy HH:mm"),
                            firstConflict.EndAt.ToString("dd/MM/yyyy HH:mm")
                        ),
                        HttpStatusCode.BadRequest
                    );
                }

                var room = await _unitOfWork.GenericRepository<Room>()
                    .GetQueryable()
                    .Include(r => r.RoomType)
                    .FirstOrDefaultAsync(r => r.Id == br.RoomId);

                if (room?.RoomType == null)
                {
                    throw new GlobalException(RoomErrorCode.NotFound, L[RoomErrorCode.NotFound], HttpStatusCode.BadRequest);
                }

                var guestCount = br.GuestCounts ?? 1;
                if (guestCount > room.RoomType.MaxGuests)
                {
                    throw new GlobalException(
                        BookingErrorCode.ExceedMaxGuests,
                        L[BookingErrorCode.ExceedMaxGuests, room.RoomNumber, room.RoomType.MaxGuests, guestCount],
                        HttpStatusCode.BadRequest
                    );
                }
            }
        }

        public async Task<BookingDetailDto> GetByIdAsync(Guid id)
        {
            var booking = await _unitOfWork.GenericRepository<Booking>()
                .GetQueryable()
                .Include(x => x.Customer)
                .Include(x => x.BookingRooms)!.ThenInclude(br => br.Room)!.ThenInclude(r => r!.RoomType)
                .Include(x => x.BookingPayments)
                .Include(x => x.Review)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeletedAt.HasValue);

            if (booking == null)
            {
                throw new GlobalException(
                    code: BookingErrorCode.NotFound,
                    message: L[BookingErrorCode.NotFound],
                    statusCode: HttpStatusCode.NotFound
                );
            }

            var dto = new BookingDetailDto
            {
                Id = booking.Id,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                TotalGuests = booking.TotalGuests,
                TotalPrice = booking.TotalPrice,
                PaidAmount = booking.PaidAmount,
                Status = booking.Status,
                CustomerId = booking.CustomerId,
                CustomerName = booking.Customer != null
                            ? (booking.Customer.Type == CustomerTypes.Individual
                                ? $"{booking.Customer.FirstName ?? string.Empty} {booking.Customer.LastName ?? string.Empty}".Trim()
                                : booking.Customer.CompanyName ?? string.Empty)
                            : string.Empty,
                CustomerPhone = booking.Customer?.PhoneNumber ?? string.Empty,
                CustomerEmail = booking.Customer?.Email,
                DiscountData = booking.DiscountData,

                Rooms = booking.BookingRooms?.Select(br => new BookingRoomDetailDto
                {
                    RoomId = br.RoomId,
                    RoomNumber = br.Room!.RoomNumber,
                    RoomTypeName = br.Room.RoomType!.Name,
                    GuestCounts = br.GuestCounts
                }).ToList() ?? new List<BookingRoomDetailDto>(),

                Payments = booking.BookingPayments?.Select(p => new BookingPaymentDto
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    PaidAt = p.PaymentDate,
                    Method = p.PaymentMethod.ToString(),
                    Status = p.PaymentStatus.ToString()
                }).ToList() ?? new List<BookingPaymentDto>(),

                Review = booking.Review != null
                    ? new ReviewDto
                    {
                        Id = booking.Review.Id,
                        Rating = booking.Review.Rating,
                        Comment = booking.Review?.Comment!,
                        CreationTime = booking.Review!.CreationTime
                    }
                    : null
            };
            return dto;
        }

        public async Task<BookingDto> UpdateAsync(Guid bookingId, UpdateBookingDto input)
        {
            var bookingRepo = _unitOfWork.GenericRepository<Booking>();
            var booking = await bookingRepo.GetQueryable()
                .Include(b => b.BookingRooms!)
                .ThenInclude(br => br.Room)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
            {
                throw new GlobalException(
                    code: BookingErrorCode.NotFound,
                    message: L[BookingErrorCode.NotFound],
                    statusCode: HttpStatusCode.NotFound
                );
            }
            if (booking.Status == BookingStatuses.Completed || booking.Status == BookingStatuses.Cancelled)
            {
                throw new GlobalException(
                    code: BookingErrorCode.CannotUpdate,
                    message: L[BookingErrorCode.CannotUpdate, booking.Status.GetDescription()],
                    statusCode: HttpStatusCode.BadRequest
                );
            }

            // kiểm tra tất cả RoomId tồn tại
            var inputRoomIds = input.BookingRooms!.Select(x => x.RoomId).ToList();
            var existingRoomIds = await _unitOfWork.GenericRepository<Room>()
                .GetQueryable()
                .Where(r => inputRoomIds.Contains(r.Id))
                .Select(r => r.Id)
                .ToListAsync();

            var notFoundRoomIds = inputRoomIds.Except(existingRoomIds).ToList();
            if (notFoundRoomIds.Any())
            {
                throw new GlobalException(
                    code: RoomErrorCode.NotFound,
                    message: L[RoomErrorCode.NotFound],
                    statusCode: HttpStatusCode.BadRequest
                );
            }

            using (_unitOfWork.BeginTransaction())
            {
                try
                {
                    booking.CheckIn = input.CheckIn;
                    booking.CheckOut = input.CheckOut;
                    booking.TotalGuests = input.BookingRooms!.Sum(br => br.GuestCounts) ?? 1;

                    foreach (var br in input.BookingRooms!)
                    {
                        var room = await _unitOfWork.GenericRepository<Room>()
                            .GetQueryable()
                            .Include(r => r.RoomType)
                            .FirstOrDefaultAsync(r => r.Id == br.RoomId);

                        if (room == null)
                            throw new GlobalException(
                                code: RoomErrorCode.NotFound,
                                message: L[RoomErrorCode.NotFound],
                                statusCode: HttpStatusCode.BadRequest
                            );
                        var guestCount = br.GuestCounts ?? 1;
                        if (guestCount > room.RoomType!.MaxGuests)
                        {
                            throw new GlobalException(
                                code: BookingErrorCode.ExceedMaxGuests,
                                message: L[BookingErrorCode.ExceedMaxGuests, room.RoomNumber, room.RoomType.MaxGuests, guestCount],
                                statusCode: HttpStatusCode.BadRequest
                            );
                        }
                    }// Validate conflict (skip usages chính booking hiện tại)
                    await ValidateBookingConflictsAsync(booking, input);

                    // Xử lý rooms (xóa/add/update) và cập nhật RoomUsage (gồm cleaning)
                    await UpdateBookingRoomsAsync(booking, input);

                    // Tính lại giá, áp membership + coupon
                    await RecalculateBookingPriceWithDiscountsAsync(booking, input);

                    // Re-check conflicts
                    await ValidateBookingConflictsAsync(booking, input);

                    _unitOfWork.GenericRepository<Booking>().Update(booking);
                    await _unitOfWork.SaveChangeAsync();
                    _unitOfWork.Commit();

                    var updatedBooking = await bookingRepo.GetQueryable()
                        .Include(b => b.BookingRooms!).ThenInclude(br => br.Room)
                        .Include(b => b.Customer)
                        .FirstOrDefaultAsync(b => b.Id == booking.Id);

                    return ObjectMapper.Map<Booking, BookingDto>(updatedBooking!);
                }
                catch (GlobalException)
                {
                    _unitOfWork.Rollback();
                    throw;
                }
                catch (Exception e)
                {
                    _unitOfWork.Rollback();
                    throw new GlobalException(
                        code: BaseErrorCode.UnexpectedError,
                        message: e.Message,
                        statusCode: HttpStatusCode.BadRequest
                    );
                }
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var booking = await _unitOfWork.GenericRepository<Booking>()
                .GetQueryable()
                .Include(x => x.BookingRooms!)
                .ThenInclude(y => y.RoomUsages)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (booking == null)
            {
                throw new GlobalException(
                    code: BookingErrorCode.NotFound,
                    message: L[BookingErrorCode.NotFound],
                    statusCode: HttpStatusCode.NotFound
                );
            }

            if (booking.Status != BookingStatuses.Pending && booking.Status != BookingStatuses.Cancelled)
            {
                throw new GlobalException(
                    code: BookingErrorCode.NotDelete,
                    message: L[BookingErrorCode.NotDelete],
                    statusCode: HttpStatusCode.BadRequest
                );
            }
            // Lấy BookingRoomId liên quan
            // var bookingRoomIds = booking.BookingRooms!.Select(br => br.Id).ToList();

            // Xóa RoomUsage liên quan
            var usages = booking.BookingRooms!
                        .SelectMany(br => br.RoomUsages ?? new List<RoomUsage>())
                        .ToList();

            using (_unitOfWork.BeginTransaction())
                try
                {
                    if (usages.Any())
                    {
                        _unitOfWork.GenericRepository<RoomUsage>().RemoveRange(usages);
                    }
                    // Xóa BookingRooms
                    // if (booking.BookingRooms!.Any())
                    // {
                    //     _unitOfWork.GenericRepository<BookingRoom>().RemoveRange(booking.BookingRooms!);
                    // }
                    booking.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                    _unitOfWork.GenericRepository<Booking>().Update(booking);

                    await _unitOfWork.SaveChangeAsync();
                    _unitOfWork.Commit();
                }
                catch (Exception e)
                {
                    _unitOfWork.Rollback();
                    throw new GlobalException(
                        code: BaseErrorCode.UnexpectedError,
                        message: e.Message,
                        statusCode: HttpStatusCode.BadRequest
                    );
                }
        }

        #region Price Calculation
        public async Task<BookingPriceDto> GetRoomPriceDetailAsync(Guid roomTypeId, DateTime checkIn, DateTime checkOut)
        {
            var checkInDate = checkIn.Date;
            var checkOutDate = checkOut.Date;

            var matchedPricing = await _unitOfWork.GenericRepository<RoomPricing>()
                .GetAsync(p =>
                    p.RoomTypeId == roomTypeId &&
                    p.StartAt <= checkInDate &&
                    p.EndAt >= checkOutDate
                );

            // Nếu không có => lấy default
            if (matchedPricing == null)
            {
                matchedPricing = await _unitOfWork.GenericRepository<RoomPricing>()
                    .GetAsync(p => p.RoomTypeId == roomTypeId && p.IsDefault);
            }

            if (matchedPricing == null)
            {
                return new BookingPriceDto();
            }
            var result = await CalculateDetailAsync(matchedPricing, checkIn, checkOut);
            result.PricingSnapshot = ObjectMapper.Map<RoomPricing, BookingPricingSnapshot>(matchedPricing);
            return result;
        }

        private async Task<BookingPriceDto> CalculateDetailAsync(RoomPricing pricing, DateTime checkIn, DateTime checkOut)
        {
            decimal totalPrice = 0;
            double totalMinutes = (checkOut - checkIn).TotalMinutes;

            double totalDays = Math.Floor(totalMinutes / (60.0 * 24));
            double totalNights = 0;
            double extraHours = 0;

            DateTime currentStart = checkIn;
            if (totalMinutes >= 1440) // >= 1 ngày
            {
                totalPrice += pricing.DailyPrice * (decimal)totalDays;
                currentStart = checkIn.AddDays((int)totalDays);
            }

            var overnightPeriod = await _systemSettingService.GetOvernightPeriodAsync();
            var overnightStartTime = overnightPeriod.OvernightStart;
            var overnightEndTime = overnightPeriod.OvernightEnd;

            if (IsOvernightPeriod(currentStart, checkOut, overnightStartTime, overnightEndTime))
            {
                totalPrice += pricing.OvernightPrice;
                totalNights++;

                var overnightHours = CalculateOvernightDuration(currentStart, checkOut, overnightStartTime, overnightEndTime);
                double totalRemainingHours = (checkOut - currentStart).TotalHours;
                double remainingHours = totalRemainingHours - overnightHours;

                if (remainingHours > 0)
                {
                    totalPrice += CalculateHourlyPrice(pricing, remainingHours, false);
                    extraHours = remainingHours;
                }
            }
            else
            {
                double remainingHours = (checkOut - currentStart).TotalHours;
                totalPrice += CalculateHourlyPrice(pricing, remainingHours, totalDays == 0);
                extraHours = remainingHours;
            }

            RoomPricingTypes pricingType;
            if (totalDays > 0 && (totalNights > 0 || extraHours > 0))
                pricingType = RoomPricingTypes.Mixed;
            else if (totalDays > 0)
                pricingType = RoomPricingTypes.Daily;
            else if (totalNights > 0 && extraHours > 0)
                pricingType = RoomPricingTypes.Mixed;
            else if (totalNights > 0)
                pricingType = RoomPricingTypes.Overnight;
            else
                pricingType = RoomPricingTypes.Hourly;

            return new BookingPriceDto
            {
                BaseTotalPrice = totalPrice,
                FinalTotalPrice = totalPrice,      // Ban đầu = base, sau trừ discount sẽ thay đổi
                PricingType = pricingType,
                ExtraHours = extraHours,
                TotalDays = totalDays,
                TotalNights = totalNights
            };
        }

        private double CalculateOvernightDuration(DateTime start, DateTime end, TimeSpan overnightStartTime, TimeSpan overnightEndTime)
        {
            var overnightDate = start.Date;
            var overnightStart = overnightDate.Add(overnightStartTime);
            var overnightEnd = overnightEndTime < overnightStartTime
                ? overnightDate.AddDays(1).Add(overnightEndTime)
                : overnightDate.Add(overnightEndTime);

            var overlapStart = start > overnightStart ? start : overnightStart;
            var overlapEnd = end < overnightEnd ? end : overnightEnd;

            var overlap = overlapEnd - overlapStart;
            return overlap.TotalHours > 0 ? overlap.TotalHours : 0;
        }

        private bool IsOvernightPeriod(DateTime start, DateTime end, TimeSpan overnightStartTime, TimeSpan overnightEndTime)
        {
            var overnightDate = start.Date;
            var overnightStart = overnightDate.Add(overnightStartTime);
            var overnightEnd = overnightEndTime < overnightStartTime
                ? overnightDate.AddDays(1).Add(overnightEndTime)
                : overnightDate.Add(overnightEndTime);

            var overlapStart = start > overnightStart ? start : overnightStart;
            var overlapEnd = end < overnightEnd ? end : overnightEnd;

            var overlap = overlapEnd - overlapStart;

            return overlap.TotalHours >= 6;
        }

        private decimal CalculateHourlyPrice(RoomPricing pricing, double hours, bool isFirst)
        {
            if (isFirst)
            {
                if (hours <= pricing.BaseDuration)
                    return pricing.BasePrice;
                else
                    return pricing.BasePrice + pricing.ExtraHourPrice * ((decimal)hours - pricing.BaseDuration);
            }
            return pricing.ExtraHourPrice * (decimal)hours;
        }
        #endregion

        #region RoomUsage Check
        private async Task<List<RoomUsageDto>> GetOverlappingUsagesAsync(Guid roomId, DateTime checkIn, DateTime checkOut)
        {
            var usages = await _unitOfWork.GenericRepository<RoomUsage>()
                .GetQueryable()
                .Where(u =>
                    u.RoomId == roomId &&
                    u.StartAt < checkOut &&
                    u.EndAt > checkIn
                )
                .Include(u => u.Room)
                .ToListAsync();

            return usages.Select(u => new RoomUsageDto
            {
                RoomId = u.RoomId,
                RoomNumber = u.Room!.RoomNumber,
                StartAt = u.StartAt,
                EndAt = u.EndAt
            }).ToList();
        }
        #endregion

        #region Update Booking
        private async Task ValidateBookingConflictsAsync(Booking booking, UpdateBookingDto input)
        {
            foreach (var br in input.BookingRooms!)
            {
                var conflicts = await _unitOfWork.GenericRepository<RoomUsage>()
                    .GetQueryable()
                    .Include(u => u.Room)
                    .Include(u => u.BookingRoom)
                    .Where(u =>
                        u.RoomId == br.RoomId &&
                        u.BookingRoom!.BookingId != booking.Id && // bỏ qua chính booking
                        u.StartAt < input.CheckOut &&
                        u.EndAt > input.CheckIn
                    )
                    .ToListAsync();

                if (conflicts.Any())
                {
                    var firstConflict = conflicts.First();
                    throw new GlobalException(
                        code: BookingErrorCode.RoomUnavailable,
                        message: string.Format(
                            L[BookingErrorCode.RoomUnavailable],
                            string.Join(", ", conflicts.Select(c => c.Room!.RoomNumber)),
                            firstConflict.StartAt.ToString("dd/MM/yyyy HH:mm"),
                            firstConflict.EndAt.ToString("dd/MM/yyyy HH:mm")
                        ),
                        statusCode: HttpStatusCode.BadRequest
                    );
                }
            }
        }

        private async Task UpdateBookingRoomsAsync(Booking booking, UpdateBookingDto input)
        {
            var existingRooms = booking.BookingRooms!.ToList();
            var inputRoomIds = input.BookingRooms!.Select(x => x.RoomId).ToList();
            var cleaningMinutes = await _systemSettingService.GetCleaningMinutesAsync();

            var toRemove = existingRooms.Where(er => !inputRoomIds.Contains(er.RoomId)).ToList();
            foreach (var remove in toRemove)
            {
                var usages = await _unitOfWork.GenericRepository<RoomUsage>()
                    .GetQueryable()
                    .Where(u => u.BookingRoomId == remove.Id)
                    .ToListAsync();

                _unitOfWork.GenericRepository<RoomUsage>().RemoveRange(usages);
                _unitOfWork.GenericRepository<BookingRoom>().Remove(remove);
            }

            foreach (var br in existingRooms.Where(er => inputRoomIds.Contains(er.RoomId)))
            {
                var inputRoom = input.BookingRooms!.First(x => x.RoomId == br.RoomId);
                br.GuestCounts = inputRoom.GuestCounts ?? br.GuestCounts;

                var usages = await _unitOfWork.GenericRepository<RoomUsage>()
                    .GetQueryable()
                    .Where(u => u.BookingRoomId == br.Id)
                    .ToListAsync();

                var bookedUsage = usages.FirstOrDefault(u => u.Status == RoomStatuses.Booked);
                var cleaningUsage = usages.FirstOrDefault(u => u.Status == RoomStatuses.Cleaning);

                if (bookedUsage != null)
                {
                    bookedUsage.StartAt = booking.CheckIn;
                    bookedUsage.EndAt = booking.CheckOut;
                }

                if (cleaningUsage != null)
                {
                    cleaningUsage.StartAt = booking.CheckOut;
                    cleaningUsage.EndAt = booking.CheckOut.AddMinutes(cleaningMinutes);
                }
            }

            var newRooms = input.BookingRooms!.Where(ir => !existingRooms.Any(er => er.RoomId == ir.RoomId)).ToList();
            foreach (var nr in newRooms)
            {
                var newBookingRoom = new BookingRoom
                {
                    BookingId = booking.Id,
                    RoomId = nr.RoomId,
                    GuestCounts = nr.GuestCounts ?? 1
                };

                await _unitOfWork.GenericRepository<BookingRoom>().AddAsync(newBookingRoom);

                var bookedUsage = new RoomUsage
                {
                    RoomId = nr.RoomId,
                    BookingRoomId = newBookingRoom.Id,
                    StartAt = booking.CheckIn,
                    EndAt = booking.CheckOut,
                    Status = RoomStatuses.Booked
                };
                var cleaningUsage = new RoomUsage
                {
                    RoomId = nr.RoomId,
                    BookingRoomId = newBookingRoom.Id,
                    StartAt = booking.CheckOut,
                    EndAt = booking.CheckOut.AddMinutes(cleaningMinutes),
                    Status = RoomStatuses.Cleaning
                };

                await _unitOfWork.GenericRepository<RoomUsage>().AddRangeAsync(new[] { bookedUsage, cleaningUsage });
            }
        }
        
        private async Task RecalculateBookingPriceWithDiscountsAsync(Booking booking, UpdateBookingDto input)
        {
            decimal totalPrice = 0;
            foreach (var br in booking.BookingRooms!)
            {
                var room = await _unitOfWork.GenericRepository<Room>().GetAsync(x => x.Id == br.RoomId);
                if (room != null)
                {
                    var priceDto = await GetRoomPriceDetailAsync(room.RoomTypeId, booking.CheckIn, booking.CheckOut);
                    totalPrice += priceDto.BaseTotalPrice;

                    var pricingSnapshot = priceDto.PricingSnapshot;
                    br.PricingSnapshot = pricingSnapshot;
                }
            }
            var customer = await GetCustomerAsync(booking.CustomerId);

            var discountData = new DiscountData
            {
                MembershipDiscountType = customer?.Group?.DiscountType,
                MembershipDiscountValue = customer?.Group?.DiscountValue ?? 0,
                OriginalPrice = totalPrice
            };

            // Áp dụng coupon nếu có
            if (!string.IsNullOrWhiteSpace(input.CouponCode))
            {
                var couponResult = await _couponService.ApplyCoupon(new ApplyCouponRequestDto
                {
                    Total = totalPrice - discountData.MembershipDiscountValue,
                    Code = input.CouponCode,
                    CustomerId = booking.CustomerId,
                    BookingId = booking.Id,
                    IsFromCart = false
                });

                discountData.VoucherCode = couponResult.CouponCode;
                discountData.VoucherType = couponResult.DiscountType;
                discountData.VoucherValue = couponResult.DiscountValue;

                booking.TotalPrice = couponResult.SubTotal;
            }
            else
            {
                var afterMembership = totalPrice - discountData.MembershipDiscountValue;
                booking.TotalPrice = afterMembership >= 0 ? afterMembership : 0;
            }
            booking.DiscountData = discountData;
        }
        #endregion

        public async Task<PagedResultDto<BookingDto>> GetPagingWithFilterAsync(BookingFilterDto input)
        {
            var searchTerm = !string.IsNullOrEmpty(input.Text)
                ? $" {NormalizeString.ConvertNormalizeString(input.Text)} "
                : string.Empty;

            var query = _unitOfWork.GenericRepository<Booking>()
                .GetQueryable()
                .Include(x => x.Customer)
                .Include(x => x.BookingRooms)!.ThenInclude(y => y.Room)!.ThenInclude(z => z!.RoomType)
                .Include(x => x.BookingRooms)!.ThenInclude(y => y.Room)!.ThenInclude(z => z!.Branch)
                .Where(x => !x.DeletedAt.HasValue)
                .WhereIf(!string.IsNullOrEmpty(input.Text),
                    x => (" " + x.Customer!.NormalizeFullInfo + " ").Contains(searchTerm) ||
                         (" " + x.Customer!.PhoneNumber + " ").Contains(searchTerm))
                .WhereIf(input.BranchId.HasValue, x => x.BookingRooms!.Any(br => br.Room!.BranchId == input.BranchId))
                .WhereIf(input.RoomTypeId.HasValue, x => x.BookingRooms!.Any(br => br.Room!.RoomTypeId == input.RoomTypeId))
                .WhereIf(input.Status.HasValue, x => x.Status == input.Status)
                .WhereIf(input.FromDate.HasValue, x => x.CheckIn.Date >= input.FromDate!.Value.Date)
                .WhereIf(input.ToDate.HasValue, x => x.CheckOut.Date <= input.ToDate!.Value.Date)
                .WhereIf(input.StartDate != null, x => x.CreationTime.AddHours(7).Date >= input.StartDate!.Value.Date)
                .WhereIf(input.EndDate != null, x => x.CreationTime.AddHours(7).Date <= input.EndDate!.Value.Date)
                .OrderByDescending(x => x.CreationTime);

            var data = await query.GetPaged(input.CurrentPage, input.PageSize);

            return ObjectMapper.Map<CRM_Homestay.Entity.Bases.PagedResult<Booking>, PagedResultDto<BookingDto>>(data);
        }

        public async Task<List<BookingDto>> GetAllAsync()
        {
            var threeMonthsAgo = DateTime.SpecifyKind(DateTime.UtcNow.AddMonths(-3), DateTimeKind.Unspecified);
            var bookings = await _unitOfWork.GenericRepository<Booking>()
                .GetQueryable()
                .Where(x => !x.DeletedAt.HasValue || (x.DeletedAt.HasValue && x.DeletedAt.Value.Date >= threeMonthsAgo.Date))
                .Include(x => x.Customer)
                .Include(x => x.BookingRooms!).ThenInclude(y => y.Room!)
                .AsNoTracking()
                .OrderByDescending(x => x.CreationTime)
                .ToListAsync();

            return ObjectMapper.Map<List<Booking>, List<BookingDto>>(bookings);
        }

        public async Task<bool> CancelBookingAsync(Guid id)
        {
            var bookingRepo = _unitOfWork.GenericRepository<Booking>();

            var booking = await bookingRepo
                .GetQueryable()
                .Include(x => x.BookingRooms!)
                    .ThenInclude(br => br.RoomUsages)
                .Include(x => x.BookingPayments)
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeletedAt.HasValue);

            if (booking == null)
            {
                throw new GlobalException(
                    code: BookingErrorCode.NotFound,
                    message: L[BookingErrorCode.NotFound],
                    statusCode: HttpStatusCode.NotFound
                );
            }

            if (booking.Status == BookingStatuses.Cancelled)
            {
                throw new GlobalException(
                    code: BookingErrorCode.Cancelled,
                    message: L[BookingErrorCode.Cancelled],
                    statusCode: HttpStatusCode.BadRequest
                );
            }

            if (booking.Status == BookingStatuses.Completed)
            {
                throw new GlobalException(
                    code: BookingErrorCode.NotCancel,
                    message: L[BookingErrorCode.NotCancel],
                    statusCode: HttpStatusCode.BadRequest
                );
            }
            using (_unitOfWork.BeginTransaction())
                try
                {

                    // update trạng thái
                    booking.Status = BookingStatuses.Cancelled;

                    // hoàn tiền nếu có
                    if (booking.BookingPayments != null && booking.BookingPayments.Any(p => p.PaymentStatus == PaymentStatuses.FullyPaid || p.PaymentStatus == PaymentStatuses.PartialPayment))
                    {
                        //TODO: Xử lý tạo yêu cầu hoàn tiền
                        foreach (var payment in booking.BookingPayments)
                        {
                            payment.PaymentStatus = PaymentStatuses.PendingRefund;
                            // payment.ReturnDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                        }
                    }
                    var roomUsageRepo = _unitOfWork.GenericRepository<RoomUsage>();
                    var usagesToRemove = booking.BookingRooms?
                        .Where(br => br.RoomUsages != null)
                        .SelectMany(br => br.RoomUsages!)
                        .ToList();

                    if (usagesToRemove != null && usagesToRemove.Any())
                    {
                        roomUsageRepo.RemoveRange(usagesToRemove);
                    }
                    bookingRepo.Update(booking);
                    await _unitOfWork.SaveChangeAsync();
                    _unitOfWork.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    _unitOfWork.Rollback();
                    throw new GlobalException(
                        code: BaseErrorCode.UnexpectedError,
                        message: e.Message,
                        statusCode: HttpStatusCode.BadRequest
                    );
                }
        }

        public async Task<ReviewBookingResultDto> ReviewBookingPriceAsync(ReviewBookingRequestDto input)
        {
            var customer = await GetCustomerAsync(input.CustomerId);

            decimal baseTotal = 0;
            var roomPrices = new List<RoomPriceDetailDto>();

            var aggregatedBooking = new BookingPriceDto();
            bool isFirstRoom = true;
            RoomPricingTypes? pricingType = null;

            foreach (var roomId in input.RoomIds)
            {
                var room = await _unitOfWork.GenericRepository<Room>()
                    .GetAsync(r => r.Id == roomId);

                if (room == null)
                    throw new GlobalException(
                        code: RoomErrorCode.NotFound,
                        message: L[RoomErrorCode.NotFound],
                        statusCode: HttpStatusCode.BadRequest);

                var priceDto = await GetRoomPriceDetailAsync(room.RoomTypeId, input.CheckIn, input.CheckOut);

                // Tổng tiền gốc cộng dồn theo room
                baseTotal += priceDto.BaseTotalPrice;
                aggregatedBooking.BaseTotalPrice += priceDto.BaseTotalPrice;

                // Set các thông tin thời gian từ room đầu tiên (share cùng checkin/checkout)
                if (isFirstRoom)
                {
                    aggregatedBooking.TotalDays = priceDto.TotalDays;
                    aggregatedBooking.TotalNights = priceDto.TotalNights;
                    aggregatedBooking.ExtraHours = priceDto.ExtraHours;
                    pricingType = priceDto.PricingType;
                    isFirstRoom = false;
                }

                roomPrices.Add(new RoomPriceDetailDto
                {
                    RoomId = room.Id,
                    Price = priceDto.BaseTotalPrice
                });
            }

            aggregatedBooking.PricingType = pricingType ?? RoomPricingTypes.Hourly;

            // Membership discount
            decimal membershipDiscount = 0;
            if (customer.Group?.DiscountType != null)
            {
                membershipDiscount = customer.Group.DiscountType == DiscountTypes.FixedAmount
                    ? customer.Group.DiscountValue ?? 0
                    : baseTotal * (customer.Group.DiscountValue ?? 0) / 100;
            }

            // Coupon discount
            decimal couponDiscount = 0;
            if (!string.IsNullOrWhiteSpace(input.CouponCode))
            {
                var couponResult = await _couponService.ApplyCoupon(new ApplyCouponRequestDto
                {
                    Total = baseTotal - membershipDiscount,
                    Code = input.CouponCode,
                    CustomerId = input.CustomerId,
                    IsFromCart = true
                });
                couponDiscount = couponResult.CouponPrice;
            }

            // Final total
            var finalTotal = baseTotal - membershipDiscount - couponDiscount;

            // Update lại cho BookingPriceDto
            aggregatedBooking.MembershipDiscount = membershipDiscount;
            aggregatedBooking.CouponDiscount = couponDiscount;
            aggregatedBooking.FinalTotalPrice = finalTotal;

            return new ReviewBookingResultDto
            {
                OriginalTotal = baseTotal,
                MembershipDiscount = membershipDiscount,
                CouponDiscount = couponDiscount,
                FinalTotal = finalTotal,
                RoomPrices = roomPrices,
                BookingPrice = aggregatedBooking
            };
        }


        private async Task<Customer> GetCustomerAsync(Guid customerId)
        {
            var customer = await _unitOfWork.GenericRepository<Customer>()
                .GetQueryable()
                .Include(x => x.Group!)
                .FirstOrDefaultAsync(x => x.Id == customerId);

            if (customer == null)
            {
                throw new GlobalException(
                    code: CustomerErrorCode.NotFound,
                    message: L[CustomerErrorCode.NotFound],
                    statusCode: HttpStatusCode.NotFound
                );
            }
            return customer;
        }
    }
}
