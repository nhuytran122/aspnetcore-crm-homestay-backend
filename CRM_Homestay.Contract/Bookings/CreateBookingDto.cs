namespace CRM_Homestay.Contract.Bookings
{
    public class CreateBookingDto
    {
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? BookingParentId { get; set; }
        public string? CouponCode { get; set; }
        public List<CreateBookingRoomDto> BookingRooms { get; set; } = new();
    }

    public class CreateBookingRoomDto
    {
        public Guid? RoomId { get; set; }
        public int? GuestCounts { get; set; }
    }
}