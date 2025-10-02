using CRM_Homestay.Contract.Reviews;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Models;

namespace CRM_Homestay.Contract.Bookings
{
    public class BookingDetailDto
    {
        public Guid Id { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int TotalGuests { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PaidAmount { get; set; }
        public BookingStatuses Status { get; set; }

        // Customer info
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        public List<BookingRoomDetailDto> Rooms { get; set; } = new();
        public DiscountData? DiscountData { get; set; }
        public List<BookingPaymentDto> Payments { get; set; } = new();
        public ReviewDto? Review { get; set; }
    }

    public class BookingRoomDetailDto
    {
        public Guid RoomId { get; set; }
        public int RoomNumber { get; set; }
        public string RoomTypeName { get; set; } = string.Empty;
        public int GuestCounts { get; set; }
    }

    public class BookingPaymentDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
        public string Method { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}