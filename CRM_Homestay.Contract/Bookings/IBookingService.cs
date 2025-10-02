using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.Bookings;

namespace CRM_Homestay.Contract.Amenities
{
    public interface IBookingService
    {
        Task<BookingDto> CreateAsync(CreateBookingDto input);
        Task<BookingDetailDto> GetByIdAsync(Guid id);
        Task<BookingDto> UpdateAsync(Guid id, UpdateBookingDto input);
        Task<PagedResultDto<BookingDto>> GetPagingWithFilterAsync(BookingFilterDto input);
        Task<List<BookingDto>> GetAllAsync();
        Task DeleteAsync(Guid id);
        Task<bool> CancelBookingAsync(Guid id);

        Task<ReviewBookingResultDto> ReviewBookingPriceAsync(ReviewBookingRequestDto input);
    }
}
