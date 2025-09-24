using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.Customers;

public interface ICustomerService
{
    public Task<PagedResultDto<CustomerWithNavigationPropertiesDto>> GetListWithNavigationPropertiesAsync(CustomerFilterDto filter);
    public Task<List<CustomerDto>> GetListAsync();
    public Task<CustomerDto> CreateAsync(CreateUpdateCustomerDto input);
    public Task<CustomerDto> UpdateAsync(CreateUpdateCustomerDto input, Guid id);
    public Task DeleteAsync(Guid id);
    Task<PagedResultDto<CustomerDto>> GetCustomerWithFilterAsync(string? text);
    Task<CustomerDto> GetByIdAsync(Guid id);
}