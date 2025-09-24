using System.ComponentModel.DataAnnotations.Schema;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Models;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.BookingRooms;
using CRM_Homestay.Entity.Customers;
using CRM_Homestay.Entity.Reviews;

namespace CRM_Homestay.Entity.Bookings
{
    public class Booking : BaseEntity
    {
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int TotalGuests { get; set; } = 1;
        public decimal TotalPrice { get; set; } = 0;
        public decimal PaidAmount { get; set; } = 0;
        public bool IsGatePassSent { get; set; } = false;
        public BookingStatuses Status { get; set; } = BookingStatuses.Pending;
        public DateTime? DeletedAt { get; set; }
        public Guid? BookingParentId { get; set; }
        [Column(TypeName = "json")]
        public BookingPricingSnapshot? PricingSnapshot { get; set; }
        [Column(TypeName = "json")]
        public DiscountData? DiscountData { get; set; }
        public Guid CustomerId { get; set; }

        public Booking? BookingParent { get; set; }
        public List<BookingRoom>? BookingRooms { get; set; }
        public List<Booking>? SubBookings { get; set; }
        public Customer? Customer { get; set; }
        public Review? Review { get; set; }
    }
}
