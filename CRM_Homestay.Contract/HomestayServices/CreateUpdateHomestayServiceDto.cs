
using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.HomestayServices
{
    public class CreateUpdateHomestayServiceDto
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public bool IsPrepaid { get; set; }
        public bool HasInventory { get; set; }
        public string? Description { get; set; }
        public List<CreateUpdateServiceItemInServiceDto>? ServiceItems { get; set; }
    }

    public class CreateUpdateServiceItemInServiceDto
    {
        public Guid? Id { get; set; }
        public string? Identifier { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Description { get; set; }
        public ServiceItemStatuses Status { get; set; } = ServiceItemStatuses.Available;
        public ServiceItemTypes Type { get; set; } = ServiceItemTypes.Others;
    }
}
