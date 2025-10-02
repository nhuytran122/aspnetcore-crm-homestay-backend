using CRM_Homestay.Core.Models;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.RoomTypes;

namespace CRM_Homestay.Entity.RoomPricings
{
    public class RoomPricing : BaseEntity, IAuditable
    {
        public int BaseDuration { get; set; } = 3;
        public decimal BasePrice { get; set; } = 0;
        public decimal ExtraHourPrice { get; set; } = 0;
        public decimal OvernightPrice { get; set; } = 0;
        public decimal DailyPrice { get; set; } = 0;
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public string? Description { get; set; }
        public bool IsDefault { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public Guid RoomTypeId { get; set; }
        public RoomType? RoomType { get; set; }

    }
}
