using CRM_Homestay.Core.Enums;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.Branches;

namespace CRM_Homestay.Entity.HomestayMaintenances
{
    public class HomestayMaintenance : BaseEntity
    {
        public Guid BranchId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public MaintenanceStatuses Status { get; set; } = MaintenanceStatuses.Pending;
        public MaintenanceTypes Type { get; set; } = MaintenanceTypes.Other;
        public MaintenanceTargets Target { get; set; } = MaintenanceTargets.Other;
        public string Description { get; set; } = string.Empty;
        public string? NormalizeFullInfo { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Branch? Branch { get; set; }
        public List<MaintenanceRoom>? MaintenanceRooms { get; set; }
    }
}
