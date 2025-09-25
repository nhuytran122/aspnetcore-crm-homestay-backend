using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.Branches
{
    public interface IBranchService
    {
        Task<BranchDto> CreateAsync(CreateUpdateBranchDto input);
        Task<BranchDto> GetByIdAsync(Guid id);
        Task<BranchDto> UpdateAsync(Guid id, CreateUpdateBranchDto input);
        Task<PagedResultDto<BranchDto>> GetPagingWithFilterAsync(BranchFilterDto input);
        Task<List<BranchDto>> GetAllActiveAsync();
        Task<List<BranchDto>> GetAllAsync();
        Task DeleteAsync(Guid id);
    }
}
