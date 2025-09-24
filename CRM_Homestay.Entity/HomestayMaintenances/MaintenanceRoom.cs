using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.Rooms;

namespace CRM_Homestay.Entity.HomestayMaintenances
{
    public class MaintenanceRoom : BaseEntity
    {
        public Guid MaintenanceId { get; set; }
        public Guid RoomId { get; set; }
        public string? Description { get; set; }

        public HomestayMaintenance? Maintenance { get; set; }
        public Room? Room { get; set; }
    }
}
