
using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.RoomTypes
{
    public class RoomTypeDto : BaseEntityDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int MaxGuests { get; set; }
        public string? MediaUrl { get; set; }
        public int BaseDuration { get; set; } = 3;
        public decimal BasePrice { get; set; }
        public decimal ExtraHourPrice { get; set; }
        public decimal OvernightPrice { get; set; }
        public decimal DailyPrice { get; set; }

    }
}
