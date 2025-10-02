using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Models;

namespace CRM_Homestay.Contract.Bookings
{
    public class BookingDto : BaseEntityDto
    {
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int TotalGuests { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PaidAmount { get; set; }
        public BookingStatuses Status { get; set; }
        public Guid CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public List<BookingRoomDto> Rooms { get; set; } = new();
    }

    public class BookingRoomDto
    {
        public Guid RoomId { get; set; }
        public int RoomNumber { get; set; }
        public Guid RoomTypeId { get; set; }
        public string RoomTypeName { get; set; } = string.Empty;
        public int GuestCounts { get; set; }
    }
}