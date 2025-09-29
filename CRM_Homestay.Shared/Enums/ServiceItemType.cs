using System.ComponentModel;

namespace CRM_Homestay.Core.Enums;

public enum ServiceItemTypes
{
    [Description("Khác")]
    Others = 0,
    [Description("Xe tay ga")]
    Scooter = 1,
    [Description("Xe số")]
    ManualBike = 2,
    [Description("Xe điện")]
    Electric = 3
}