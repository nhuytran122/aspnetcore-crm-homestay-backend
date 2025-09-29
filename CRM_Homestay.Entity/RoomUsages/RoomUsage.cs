using CRM_Homestay.Core.Enums;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.BookingRooms;
using CRM_Homestay.Entity.HomestayMaintenances;
using CRM_Homestay.Entity.Rooms;

namespace CRM_Homestay.Entity.RoomUsages
{
    public class RoomUsage : BaseEntity
    {
        public Guid RoomId { get; set; }
        public Guid? BookingRoomId { get; set; }
        public Guid? MaintenanceId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public RoomStatuses Status { get; set; } = RoomStatuses.Booked;

        public BookingRoom? BookingRoom { get; set; }
        public MaintenanceRoom? MaintenanceRoom { get; set; }
        public Room? Room { get; set; }
    }
}
