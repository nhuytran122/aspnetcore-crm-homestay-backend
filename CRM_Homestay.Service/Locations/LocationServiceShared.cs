using AutoMapper;
using CRM_Homestay.Contract.Locations;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.Districts;
using CRM_Homestay.Entity.Provinces;
using CRM_Homestay.Entity.Wards;
using CRM_Homestay.Localization;
using JetBrains.Annotations;

namespace CRM_Homestay.Service.Locations;

public class LocationServiceShared : BaseService, ILocationServiceShared
{
    public LocationServiceShared([NotNull] IUnitOfWork unitOfWork, [NotNull] IMapper mapper, [NotNull] ILocalizer l) : base(unitOfWork, mapper, l)
    {
    }


    public async Task<LocationDto> GetLocations(LocationRequestDto request)
    {
        var province = await _unitOfWork
            .GenericRepository<Province>()
            .GetAsync(x => x.Id == request.ProvinceId);
        var district = await _unitOfWork
            .GenericRepository<District>()
            .GetAsync(x => x.Id == request.DistrictId);
        var ward = await _unitOfWork
            .GenericRepository<Ward>()
            .GetAsync(x => x.Id == request.WardId);

        return new LocationDto()
        {
            Province = province?.Name,
            District = district?.Name,
            Ward = ward?.Name,
            Locate = request?.Locate
        };
    }
}