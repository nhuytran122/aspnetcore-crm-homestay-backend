using System.ComponentModel;

namespace CRM_Homestay.Core.Enums;

public enum AmenityTypes
{
    [Description("Tổng quát")]
    General = 0,
    [Description("Phòng ngủ")]
    Bedroom = 1,
    [Description("Phòng tắm")]
    Bathroom = 2,
    [Description("Tiện nghi phòng khách")]
    LivingRoom = 3,
    [Description("Nhà bếp")]
    Kitchen = 4,
    [Description("Ngoài trời")]
    Outdoor = 5,
    [Description("An ninh")]
    Security = 6,
    [Description("Tiện nghi phòng")]
    RoomAmenities = 7,
    [Description("Hoạt động")]
    Activities = 8,
    [Description("Truyền thông & Công nghệ")]
    MediaAndTechnology = 9,
    [Description("Đồ ăn và thức uống")]
    FoodAndDrink = 10,
    [Description("Internet")]
    Internet = 11,
    [Description("Đỗ xe")]
    Parking = 12,
    [Description("Phương tiện di chuyển")]
    Transportation = 13,
    [Description("Dịch vụ lễ tân")]
    FrontDeskServices = 14,
    [Description("Dịch vụ dọn phòng")]
    CleaningServices = 15,
    [Description("Khác")]
    Others = 16
}