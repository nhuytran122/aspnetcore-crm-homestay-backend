namespace CRM_Homestay.Contract.Coupons;

public class ApplyCouponRequestDto
{
    public Guid CustomerId { get; set; }
    public Guid? BookingId { get; set; }
    public decimal Total { get; set; }
    public string Code { get; set; } = "";
}
