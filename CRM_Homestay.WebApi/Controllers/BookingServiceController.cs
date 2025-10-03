using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRM_Homestay.Contract.BookingServices;

namespace CRM_Homestay.App.Controllers
{
    /// <summary>
    /// BookingServiceController
    /// </summary>
    [Route("api/booking-services")]
    [ApiController]
    [Authorize]
    public class BookingServiceController : BaseController
    {
        private readonly IBookingServiceService _bookingServiceService;
        /// <summary>
        /// BookingServiceController init
        /// </summary>
        /// <param name="bookingServiceService"></param>
        /// <param name="httpContextAccessor"></param>
        public BookingServiceController(IBookingServiceService bookingServiceService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _bookingServiceService = bookingServiceService;
        }

        /// <summary>
        /// Tạo booking service từ booking
        /// </summary>
        /// /// <param name="bookingId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("{bookingId}")]
        public async Task<List<BookingServiceDto>> Create(Guid bookingId, [FromBody] CreateBookingServicesDto input)
        {
            return await _bookingServiceService.CreateBookingServicesFromBooking(bookingId, input);
        }

        /// <summary>
        /// Update booking service
        /// </summary>
        /// /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<BookingServiceDto> UpdateAsync(Guid id, [FromBody] UpdateBookingServiceDto input)
        {
            return await _bookingServiceService.UpdateAsync(id, input);
        }
        /// <summary>
        /// Hủy đơn
        /// </summary>
        /// <returns></returns>
        [HttpPut("cancel/{id}")]
        public async Task<bool> CancelBookingService(Guid id)
        {
            return await _bookingServiceService.CancelAsync(id);
        }

    }
}
