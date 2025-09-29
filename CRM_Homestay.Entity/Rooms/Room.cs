using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.Bookings;
using CRM_Homestay.Entity.Branches;
using CRM_Homestay.Entity.HomestayMaintenances;
using CRM_Homestay.Entity.Medias;
using CRM_Homestay.Entity.RoomAmenities;
using CRM_Homestay.Entity.RoomTypes;
using CRM_Homestay.Entity.RoomUsages;

namespace CRM_Homestay.Entity.Rooms
{
    public class Room : BaseEntity
    {
        public int RoomNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid BranchId { get; set; }
        public Guid RoomTypeId { get; set; }
        public Branch? Branch { get; set; }
        public RoomType? RoomType { get; set; }
        public List<Booking>? Bookings { get; set; }
        public List<MaintenanceRoom>? MaintenanceRooms { get; set; }
        public List<RoomAmenity>? RoomAmenities { get; set; }
        public List<MediaRoom>? Medias { get; set; }
        public List<RoomUsage>? RoomUsages { get; set; }
    }
}
