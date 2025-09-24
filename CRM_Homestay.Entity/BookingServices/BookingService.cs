using CRM_Homestay.Core.Enums;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.BookingPaymentDetails;
using CRM_Homestay.Entity.BookingRooms;
using CRM_Homestay.Entity.Bookings;
using CRM_Homestay.Entity.BookingServiceItems;
using CRM_Homestay.Entity.HomestayServices;
using CRM_Homestay.Entity.Users;

namespace CRM_Homestay.Entity.BookingServices
{
    public class BookingService : BaseEntity
    {
        public double Quantity { get; set; }
        public string? Description { get; set; }
        public Guid BookingId { get; set; }
        public Guid ServiceId { get; set; }
        public Guid? BookingRoomId { get; set; }
        public Guid? AssignedStaffId { get; set; }
        public decimal? TotalPrice { get; set; }
        public BookingServiceStatuses Status { get; set; } = BookingServiceStatuses.Pending;
        public DateTime? DeletedAt { get; set; }
        public Booking? Booking { get; set; }
        public HomestayService? Service { get; set; }
        public BookingRoom? BookingRoom { get; set; }
        public User? AssignedStaff { get; set; }
        public BookingPaymentDetail? BookingPaymentDetail { get; set; }
        public List<BookingServiceItem>? BookingServiceItems { get; set; }
    }
}
