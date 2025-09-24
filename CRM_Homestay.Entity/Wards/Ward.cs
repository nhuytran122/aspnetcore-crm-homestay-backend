using CRM_Homestay.Entity.Districts;

namespace CRM_Homestay.Entity.Wards;

public class Ward
{
    public int Id { get; set; }
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DivisionType { get; set; } = string.Empty;

    public int DistrictId { get; set; }

    public District? District { get; set; }
}