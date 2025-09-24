using System.ComponentModel;

namespace CRM_Homestay.Core.Enums
{
    public enum MaintenanceTypes
    {
        [Description("Bảo trì định kỳ")]
        Periodic,
        [Description("Bảo trì khẩn cấp")]
        Emergency,
        [Description("Bảo trì khác")]
        Other
    }
}