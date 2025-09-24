namespace CRM_Homestay.Contract.Locations;

public class LocationRequestDto
{
    public int ProvinceId { get; set; }
    public int DistrictId { get; set; }
    public int WardId { get; set; }
    public string? Locate { get; set; }

    public LocationRequestDto() { }
    public LocationRequestDto(int provinceId, int districtId, int wardId)
    {
        ProvinceId = provinceId;
        DistrictId = districtId;
        WardId = wardId;
    }
}