using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.CustomerGroups;

public class CreateUpdateCustomerGroupDto
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DiscountTypes DiscountType { get; set; } = DiscountTypes.Percentage;
    public decimal DiscountValue { get; set; } = 0;
    public double MinPoints { get; set; } = 0;
}