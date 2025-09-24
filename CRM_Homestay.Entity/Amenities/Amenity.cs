using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.RoomAmenities;
using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Entity.Amenities
{
    public class Amenity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public AmenityTypes Type { get; set; } = AmenityTypes.Others;
        public List<RoomAmenity>? RoomAmenities { get; set; }
    }
}
