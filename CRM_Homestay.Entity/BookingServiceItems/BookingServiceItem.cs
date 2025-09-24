using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.BookingServices;
using CRM_Homestay.Entity.ServiceItems;

namespace CRM_Homestay.Entity.BookingServiceItems
{
    public class BookingServiceItem : BaseEntity
    {
        public Guid BookingServiceId { get; set; }
        public Guid ServiceItemId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public BookingService? BookingService { get; set; }
        public ServiceItem? ServiceItem { get; set; }
    }
}