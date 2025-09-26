using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.RoomAmenities;
using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.Amenities
{
    public interface IAmenityService
    {
        Task<AmenityDto> CreateAsync(CreateUpdateAmenityDto input);
        Task<AmenityDto> GetByIdAsync(Guid id);
        Task<AmenityDto> UpdateAsync(Guid id, CreateUpdateAmenityDto input);
        Task<PagedResultDto<AmenityDto>> GetPagingWithFilterAsync(AmenityFilterDto input);
        Task<List<RoomAmenityDto>> GetRoomAmenitiesByAmenityAsync(Guid id);
        Task<List<AmenityDto>> GetAllAsync();
        Task<List<AmenityDto>> GetByTypeAsync(AmenityTypes? type);
        Task DeleteAsync(Guid id);
    }
}
