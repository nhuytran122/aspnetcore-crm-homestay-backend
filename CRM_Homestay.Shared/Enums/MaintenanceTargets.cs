using System.ComponentModel;

namespace CRM_Homestay.Core.Enums
{
    public enum MaintenanceTargets
    {
        [Description("Bảo trì phòng")]
        Room,
        [Description("Bảo trì thiết bị")]
        Equipment,
        [Description("Bảo trì khu vực chung (Lobby, WC, Hành lang, ...)")]
        Facility,
        [Description("Bảo trì khác")]
        Other
    }
}