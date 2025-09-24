using CRM_Homestay.Core.Enums;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.BookingServiceItems;
using CRM_Homestay.Entity.BranchInventories;
using CRM_Homestay.Entity.HomestayServices;

namespace CRM_Homestay.Entity.ServiceItems
{
    public class ServiceItem : BaseEntity
    {
        public Guid ServiceId { get; set; }
        public string? Identifier { get; set; }
        public ServiceItemStatuses Status { get; set; } = ServiceItemStatuses.Available;
        public string? Description { get; set; }

        public HomestayService? HomestayService { get; set; }
        public List<BookingServiceItem>? BookingServiceItems { get; set; }
    }
}
