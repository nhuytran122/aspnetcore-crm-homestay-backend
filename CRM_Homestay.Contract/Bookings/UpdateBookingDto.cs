namespace CRM_Homestay.Contract.Bookings
{
    public class UpdateBookingDto
    {
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public List<UpdateBookingRoomDto>? BookingRooms { get; set; }
        public string? CouponCode { get; set; }
    }

    public class UpdateBookingRoomDto
    {
        public Guid RoomId { get; set; }
        public int? GuestCounts { get; set; } = 1;
    }

}