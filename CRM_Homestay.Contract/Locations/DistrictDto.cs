namespace CRM_Homestay.Contract.Locations;

public class DistrictDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Code { get; set; }
    public string? DivisionType { get; set; }

    public int ProvinceId { get; set; }
}