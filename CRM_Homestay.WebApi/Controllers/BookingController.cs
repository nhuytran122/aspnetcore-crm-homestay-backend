using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.Amenities;
using CRM_Homestay.Core.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRM_Homestay.Contract.Bookings;
using CRM_Homestay.Contract.BookingServices;

namespace CRM_Homestay.App.Controllers
{
    /// <summary>
    /// BookingController
    /// </summary>
    [Route("api/bookings")]
    [ApiController]
    [Authorize]
    public class BookingController : BaseController
    {
        private readonly IBookingService _bookingService;
        private readonly IBookingServiceService _bookingServiceService;
        /// <summary>
        /// BookingController init
        /// </summary>
        /// <param name="bookingService"></param>
        /// <param name="bookingServiceService"></param>
        /// <param name="httpContextAccessor"></param>
        public BookingController(IBookingService bookingService, IBookingServiceService bookingServiceService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _bookingService = bookingService;
            _bookingServiceService = bookingServiceService;
        }

        /// <summary>
        /// Tạo booking
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<BookingDto> Create([FromBody] CreateBookingDto input)
        {
            return await _bookingService.CreateAsync(input);
        }

        /// <summary>
        /// Lấy thông tin booking theo ID booking
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ALL)]
        [HttpGet("{id}")]
        public async Task<BookingDetailDto> GetById(Guid id)
        {
            return await _bookingService.GetByIdAsync(id);
        }

        /// <summary>
        /// Lấy danh sách booking kết hợp filter(status, branchId, roomTypeID, fromDate, toDate, startDate, endDate) và phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<PagedResultDto<BookingDto>> GetWithFilter([FromQuery] BookingFilterDto request)
        {
            return await _bookingService.GetPagingWithFilterAsync(request);
        }

        /// <summary>
        /// Cập nhật thông tin booking
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_TECHNICAL_AND_RECEPTIONIST)]
        [HttpPut("{id}")]
        public async Task<BookingDto> Update(Guid id, [FromBody] UpdateBookingDto input)
        {
            return await _bookingService.UpdateAsync(id, input);
        }

        /// <summary>
        /// Xóa booking
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await _bookingService.DeleteAsync(id);
        }

        /// <summary>
        /// Lấy toàn bộ booking 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = RoleCodes.ALL)]
        public async Task<List<BookingDto>> GetAll()
        {
            return await _bookingService.GetAllAsync();
        }

        /// <summary>
        /// Tính toán giá
        /// </summary>
        /// <returns></returns>
        [HttpPost("review-price")]
        public async Task<ReviewBookingResultDto> ReviewBooking([FromBody] ReviewBookingRequestDto input)
        {
            return await _bookingService.ReviewBookingPriceAsync(input);
        }

        /// <summary>
        /// Hủy booking
        /// </summary>
        /// <returns></returns>
        [HttpPut("cancel/{id}")]
        public async Task<bool> CancelBooking(Guid id)
        {
            return await _bookingService.CancelBookingAsync(id);
        }

        /// <summary>
        /// Đặt dịch vụ (sau khi book phòng)
        /// </summary>
        /// <param name="input"></param>
        /// /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/booking-services")]
        public async Task<List<BookingServiceDto>> CreateBookingServices(Guid id, [FromBody] CreateBookingServicesDto input)
        {
            return await _bookingServiceService.CreateBookingServicesFromBooking(id, input);
        }

    }
}
