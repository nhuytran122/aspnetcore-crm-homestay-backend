using CRM_Homestay.Core.Enums;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.BookingServiceItems;
using CRM_Homestay.Entity.HomestayServices;

namespace CRM_Homestay.Entity.ServiceItems
{
    public class ServiceItem : BaseEntity
    {
        public Guid ServiceId { get; set; }
        public string? Identifier { get; set; }
        public string Brand { get; set; } = string.Empty; // Hãng xe (Honda, Yamaha)
        public string Model { get; set; } = string.Empty; // Mẫu xe (Vision, Air Blade)
        public ServiceItemStatuses Status { get; set; } = ServiceItemStatuses.Available;
        public ServiceItemTypes Type { get; set; } = ServiceItemTypes.Others;
        public string? Description { get; set; }
        public string? NormalizeFullInfo { get; set; }

        public HomestayService? HomestayService { get; set; }
        public List<BookingServiceItem>? BookingServiceItems { get; set; }
    }
}
