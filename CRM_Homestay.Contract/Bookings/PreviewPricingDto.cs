namespace CRM_Homestay.Contract.Bookings
{
    public class ReviewBookingRequestDto
    {
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public Guid CustomerId { get; set; }
        public string? CouponCode { get; set; }
        public List<Guid> RoomIds { get; set; } = new();
    }

    public class ReviewBookingResultDto
    {
        public decimal OriginalTotal { get; set; }
        public decimal MembershipDiscount { get; set; }
        public decimal CouponDiscount { get; set; }
        public decimal FinalTotal { get; set; }
        public List<RoomPriceDetailDto> RoomPrices { get; set; } = new();
        public BookingPriceDto BookingPrice { get; set; } = new();
    }

    public class RoomPriceDetailDto
    {
        public Guid RoomId { get; set; }
        public decimal Price { get; set; }
    }
}