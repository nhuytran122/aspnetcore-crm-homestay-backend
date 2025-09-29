using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.ServiceItems
{
    public interface IServiceItemService
    {
        Task<ServiceItemDto> CreateAsync(CreateServiceItemDto input);
        Task<ServiceItemDto> GetByIdAsync(Guid id);
        Task<ServiceItemDto> UpdateAsync(Guid id, UpdateServiceItemDto input);
        Task<PagedResultDto<ServiceItemDto>> GetPagingWithFilterAsync(ServiceItemFilterDto input);
        Task<List<ServiceItemDto>> GetAllAsync();
        Task DeleteAsync(Guid id);
        Task<List<ServiceItemDto>> GetByHomestayServiceIdAsync(Guid serviceId);
    }
}
