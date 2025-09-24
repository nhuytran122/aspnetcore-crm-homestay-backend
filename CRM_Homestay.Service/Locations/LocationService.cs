using AutoMapper;
using CRM_Homestay.Contract.Locations;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.Districts;
using CRM_Homestay.Entity.Provinces;
using CRM_Homestay.Entity.Wards;
using CRM_Homestay.Localization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace CRM_Homestay.Service.Locations;

public class LocationService : BaseService, ILocationService
{
    private ILocationServiceShared _serviceShared;
    private readonly IDistributedCache _cache;
    public LocationService(IDistributedCache cache, [NotNull] IUnitOfWork unitOfWork, [NotNull] IMapper mapper, [NotNull] ILocalizer l, ILocationServiceShared shared) : base(unitOfWork, mapper, l)
    {
        _serviceShared = shared;
        _cache = cache;
    }

    public async Task<LocationDto> GetLocations(LocationRequestDto request)
    {
        return await _serviceShared.GetLocations(request);
    }

    public async Task<List<ProvinceDto>> GetProvinces()
    {
        var cacheKey = "ProvincesList";
        var cachedProvinces = await _cache.GetStringAsync(cacheKey);

        List<ProvinceDto> provinces;

        if (cachedProvinces != null)
        {
            provinces = JsonConvert.DeserializeObject<List<ProvinceDto>>(cachedProvinces)!;
        }
        else
        {
            provinces = await _unitOfWork.GenericRepository<Province>().GetQueryable()
                .Select(x => new ProvinceDto()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            cachedProvinces = JsonConvert.SerializeObject(provinces);
            await _cache.SetStringAsync(cacheKey, cachedProvinces, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(8)
            });
        }

        return provinces;
    }

    public async Task<List<DistrictDto>> GetDistricts(int provinceId)
    {
        var districts = await _unitOfWork
            .GenericRepository<District>()
            .GetListAsync(x => x.ProvinceId == provinceId);
        return ObjectMapper.Map<List<District>, List<DistrictDto>>(districts);
    }

    public async Task<List<WardDto>> GetWards(int districtId)
    {
        var wards = await _unitOfWork
            .GenericRepository<Ward>()
            .GetListAsync(x => x.DistrictId == districtId);
        return ObjectMapper.Map<List<Ward>, List<WardDto>>(wards);
    }


}