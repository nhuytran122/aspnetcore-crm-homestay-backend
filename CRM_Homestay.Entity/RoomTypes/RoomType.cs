using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.Medias;
using CRM_Homestay.Entity.RoomPricings;
using CRM_Homestay.Entity.Rooms;

namespace CRM_Homestay.Entity.RoomTypes
{
    public class RoomType : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxGuests { get; set; } = 1;
        public Guid? MediaId { get; set; }
        public DateTime? DeletedAt { get; set; }
        public List<Room>? Rooms { get; set; }
        public List<RoomPricing>? RoomPricings { get; set; }
        public BaseMedia? Media { get; set; }
    }
}
