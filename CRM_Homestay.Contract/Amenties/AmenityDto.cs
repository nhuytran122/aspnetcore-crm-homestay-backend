using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.Amenities
{
    public class AmenityDto : BaseEntityDto
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
    }
}