using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.ServiceItems
{
    public class ServiceItemDto : BaseEntityDto
    {
        public Guid HomestayServiceId { get; set; }
        public string? Identifier { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public ServiceItemStatuses Status { get; set; }
        public ServiceItemTypes Type { get; set; }
        public string? Description { get; set; }
        public string? ServiceName { get; set; }
    }
}