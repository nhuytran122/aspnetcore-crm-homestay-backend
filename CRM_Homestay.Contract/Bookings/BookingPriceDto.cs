using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Models;

namespace CRM_Homestay.Contract.Bookings
{
    public class BookingPriceDto
    {
        public decimal BaseTotalPrice { get; set; }
        public decimal FinalTotalPrice { get; set; }
        public RoomPricingTypes PricingType { get; set; }
        public double ExtraHours { get; set; }
        public double TotalDays { get; set; }
        public double TotalNights { get; set; }
        public decimal MembershipDiscount { get; set; }
        public decimal CouponDiscount { get; set; }

        public BookingPricingSnapshot PricingSnapshot { get; set; } = new BookingPricingSnapshot();
    }
}
