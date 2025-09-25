using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.Bookings;
using CRM_Homestay.Entity.Media;

namespace CRM_Homestay.Entity.Reviews
{
    public class Review : BaseEntity
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public Guid BookingId { get; set; }
        public List<MediaReview>? Medias { get; set; }
        public Booking? Booking { get; set; }
    }
}