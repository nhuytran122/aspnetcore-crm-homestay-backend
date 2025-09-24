using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.CustomerGroups;

public interface ICustomerGroupService
{
    public Task<PagedResultDto<CustomerGroupDto>> GetListAsync(CustomerGroupFilterDto filter);
    public Task<CustomerGroupDto> CreateAsync(CreateUpdateCustomerGroupDto input);
    public Task<CustomerGroupDto> UpdateAsync(CreateUpdateCustomerGroupDto input, Guid id);
    public Task DeleteAsync(Guid id);

    public Task<string> GenerateCode();
    Task<List<CustomerGroupDto>> GetListDropdownAsync();
}