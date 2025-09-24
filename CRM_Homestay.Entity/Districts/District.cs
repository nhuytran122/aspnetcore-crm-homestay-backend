using CRM_Homestay.Entity.Provinces;
using CRM_Homestay.Entity.Wards;

namespace CRM_Homestay.Entity.Districts;

public class District
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Code { get; set; }
    public string DivisionType { get; set; } = string.Empty;

    public int ProvinceId { get; set; }
    public List<Ward>? Wards { get; set; }
    public Province? Province { get; set; }
}