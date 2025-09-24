namespace CRM_Homestay.Contract.Locations;

public class WardDto
{
    public int Id { get; set; }
    public int Code { get; set; }
    public string? Name { get; set; }
    public string? DivisionType { get; set; }

    public int DistrictId { get; set; }
}