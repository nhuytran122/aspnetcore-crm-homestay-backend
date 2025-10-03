namespace CRM_Homestay.Contract.BookingServices
{
    public interface IBookingServiceService
    {
        Task<List<BookingServiceDto>> CreateBookingServicesFromBooking(Guid bookingId, CreateBookingServicesDto input);
        Task<BookingServiceDto> UpdateAsync(Guid bookingServiceId, UpdateBookingServiceDto input);
        Task<bool> CancelAsync(Guid bookingServiceId);
    }
}