using CRM_Homestay.Entity.Bases;

namespace CRM_Homestay.Entity.FAQs;

public class FAQ : BaseEntity
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;

    public string? NormalizeFullInfo { get; set; }
    public bool IsActive { get; set; } = false;
}