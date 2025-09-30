using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.Rules
{
    public interface IRuleService
    {
        Task<RuleDto> CreateAsync(CreateUpdateRuleDto input);
        Task<RuleDto> UpdateAsync(Guid id, CreateUpdateRuleDto input);
        Task<PagedResultDto<RuleDto>> GetPagingWithFilterAsync(RuleFilterDto input);
        Task<List<RuleDto>> GetAllAsync();
        Task DeleteAsync(Guid id);
    }
}
