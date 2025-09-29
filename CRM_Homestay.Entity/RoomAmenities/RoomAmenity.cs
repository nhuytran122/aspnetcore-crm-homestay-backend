using CRM_Homestay.Entity.Amenities;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.Rooms;

namespace CRM_Homestay.Entity.RoomAmenities
{
    public class RoomAmenity : BaseEntity
    {
        public Guid RoomId { get; set; }
        public Guid AmenityId { get; set; }
        public int? Quantity { get; set; }
        public string? Note { get; set; }
        public Room? Room { get; set; }
        public Amenity? Amenity { get; set; }
    }
}
