namespace CRM_Homestay.Core.Models;

public class BookingPricingSnapshot
{
    public int BaseDuration { get; set; } = 3;
    public decimal BasePrice { get; set; }
    public decimal ExtraHourPrice { get; set; }
    public decimal OvernightPrice { get; set; }
    public decimal DailyPrice { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public string? Description { get; set; }
}
