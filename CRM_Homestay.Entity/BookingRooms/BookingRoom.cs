using System.ComponentModel.DataAnnotations.Schema;
using CRM_Homestay.Core.Models;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.Bookings;
using CRM_Homestay.Entity.Rooms;
using CRM_Homestay.Entity.RoomUsages;

namespace CRM_Homestay.Entity.BookingRooms
{
    public class BookingRoom : BaseEntity
    {
        public Guid BookingId { get; set; }
        public Guid RoomId { get; set; }
        public int GuestCounts { get; set; } = 1;

        [Column(TypeName = "json")]
        public BookingPricingSnapshot? PricingSnapshot { get; set; }
        public Booking? Booking { get; set; }
        public Room? Room { get; set; }
        public List<RoomUsage>? RoomUsages { get; set; }
    }
}
