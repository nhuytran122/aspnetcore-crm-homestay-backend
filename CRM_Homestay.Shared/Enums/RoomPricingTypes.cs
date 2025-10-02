using System.ComponentModel;

namespace CRM_Homestay.Core.Enums
{
    public enum RoomPricingTypes
    {
        [Description("Giá theo giờ")]
        Hourly,

        [Description("Giá theo ngày")]
        Daily,

        [Description("Giá theo đêm")]
        Overnight,

        [Description("Giá theo nhiều phương thức")]
        Mixed
    }

}