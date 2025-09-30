using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.FAQs
{
    public interface IFAQService
    {
        Task<FAQDto> CreateAsync(CreateUpdateFAQDto input);
        Task<FAQDto> GetByIdAsync(Guid id);
        Task<FAQDto> UpdateAsync(Guid id, CreateUpdateFAQDto input);
        Task<PagedResultDto<FAQDto>> GetPagingWithFilterAsync(FAQFilterDto input);
        Task<List<FAQDto>> GetAllAsync();
        Task DeleteAsync(Guid id);
    }
}
