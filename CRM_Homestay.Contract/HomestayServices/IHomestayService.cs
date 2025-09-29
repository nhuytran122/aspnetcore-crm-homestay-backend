using CRM_Homestay.Contract.Bases;

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
    }
}
