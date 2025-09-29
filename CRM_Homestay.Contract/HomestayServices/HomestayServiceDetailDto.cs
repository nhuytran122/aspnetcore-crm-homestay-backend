using CRM_Homestay.Contract.ServiceItems;

namespace CRM_Homestay.Contract.HomestayServices
{
    public class HomestayServiceDetailDto
    {
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public bool IsPrepaid { get; set; }
        public bool HasInventory { get; set; }
        public string? Description { get; set; }
        public List<ServiceItemDto>? ServiceItems { get; set; }
    }
}