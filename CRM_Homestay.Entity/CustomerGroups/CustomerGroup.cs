using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.Customers;
using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Entity.CustomerGroups;

public class CustomerGroup : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string NormalizedCode { get; set; } = string.Empty;
    public DiscountTypes DiscountType { get; set; } = DiscountTypes.Percentage;
    public decimal DiscountValue { get; set; } = 0;
    public double MinPoints { get; set; } = 0;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? DeletedAt { get; set; }

    //navigation
    public List<Customer>? Customers { get; set; }
    public string? NormalizeFullInfo { get; set; }
}