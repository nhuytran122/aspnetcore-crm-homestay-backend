using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.BookingServices;
using CRM_Homestay.Entity.ServiceItems;

namespace CRM_Homestay.Entity.HomestayServices
{
    public class HomestayService : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;
        public bool IsPrepaid { get; set; } = false;
        public bool HasInventory { get; set; } = false;
        public string? Description { get; set; }
        public string? NormalizeFullInfo { get; set; }

        public List<ServiceItem>? ServiceItems { get; set; }

        public List<BookingService>? BookingServices { get; set; }
    }
}
