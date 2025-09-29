using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.ServiceItems
{
    public class CreateServiceItemDto
    {
        public Guid HomestayServiceId { get; set; }
        public string? Identifier { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Description { get; set; }
        public ServiceItemStatuses Status { get; set; } = ServiceItemStatuses.Available;
        public ServiceItemTypes Type { get; set; } = ServiceItemTypes.Others;
    }
}