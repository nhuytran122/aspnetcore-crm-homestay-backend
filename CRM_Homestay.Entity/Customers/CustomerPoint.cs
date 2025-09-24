using CRM_Homestay.Entity.Bases;

namespace CRM_Homestay.Entity.Customers;

public class CustomerPoint : BaseEntity
{
    public Guid CustomerId { get; set; }
    public int TotalAccumulated { get; set; } = 0;
    public int TotalUsed { get; set; } = 0;
    public int CurrentBalance { get; set; } = 0;
    public int HoldPoint { get; set; } = 0;
    public Customer? Customer { get; set; }
}
