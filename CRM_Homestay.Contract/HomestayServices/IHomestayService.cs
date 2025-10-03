using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.ServiceItems;

namespace CRM_Homestay.Contract.HomestayServices
{
    public interface IHomestayServiceService
    {
        Task<HomestayServiceDto> CreateAsync(CreateUpdateHomestayServiceDto input);
        Task<HomestayServiceDetailDto> GetByIdAsync(Guid id);
        Task<HomestayServiceDto> UpdateAsync(Guid id, CreateUpdateHomestayServiceDto input);
        Task<PagedResultDto<HomestayServiceDto>> GetPagingWithFilterAsync(HomestayServiceFilterDto input);
        Task<List<HomestayServiceDto>> GetAllAsync();
        Task DeleteAsync(Guid id);
        Task<List<ServiceItemDto>> GetServiceItemsByServiceIdAsync(Guid id);

    }
}
