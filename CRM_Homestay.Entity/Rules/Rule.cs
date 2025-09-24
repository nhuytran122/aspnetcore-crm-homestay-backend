using CRM_Homestay.Entity.Bases;

namespace CRM_Homestay.Entity.Rules;

public class Rule : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;
}