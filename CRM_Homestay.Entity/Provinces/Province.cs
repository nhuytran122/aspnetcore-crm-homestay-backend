using CRM_Homestay.Entity.Districts;

namespace CRM_Homestay.Entity.Provinces;

public class Province
{
    public int Id { get; set; }
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DivisionType { get; set; } = String.Empty;

    public List<District>? Districts { get; set; }
}