using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.Amenities
{
    public class CreateUpdateAmenityDto
    {
        public string? Name { get; set; }
        public AmenityTypes? Type { get; set; }
    }
}