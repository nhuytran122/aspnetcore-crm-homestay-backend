using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.CustomerGroups;

public class CreateUpdateCustomerGroupDto
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DiscountTypes? DiscountType { get; set; }
    public decimal? DiscountValue { get; set; }
    public double? MinPoints { get; set; }
}