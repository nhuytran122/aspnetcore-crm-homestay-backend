using CRM_Homestay.Entity.Districts;

namespace CRM_Homestay.Contract.Locations;

public interface ILocationService
{
    public Task<LocationDto> GetLocations(LocationRequestDto request);
    public Task<List<ProvinceDto>> GetProvinces();
    public Task<List<DistrictDto>> GetDistricts(int provinceId);
    public Task<List<WardDto>> GetWards(int districtId);
}

public interface ILocationServiceShared
{
    public Task<LocationDto> GetLocations(LocationRequestDto request);
}