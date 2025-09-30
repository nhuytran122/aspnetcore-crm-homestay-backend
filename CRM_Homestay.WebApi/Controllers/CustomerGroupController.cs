using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.CustomerGroups;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Homestay.App.Controllers;

/// <summary>
/// CustomerGroupController
/// </summary>
[ApiController]
[Route("api/customer-groups/")]
[Authorize]
public class CustomerGroupController : BaseController
{
    private ICustomerGroupService _customerGroupService;
    /// <summary>
    /// CustomerGroupController init
    /// </summary>
    /// <param name="customerService"></param>
    /// <param name="httpContextAccessor"></param>
    public CustomerGroupController(ICustomerGroupService customerService, [NotNull] IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        _customerGroupService = customerService;
    }

    /// <summary>
    ///  Lấy ra danh sách nhóm
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<PagedResultDto<CustomerGroupDto>> GetListAsync([FromQuery] CustomerGroupFilterDto filter)
    {
        return await _customerGroupService.GetListAsync(filter);
    }

    /// <summary>
    /// Lấy danh sách nhóm khách hàng cho Dropdown
    /// </summary>
    /// <returns></returns>
    [HttpGet("dropdowns")]
    public async Task<List<CustomerGroupDto>> GetListDropdown()
    {
        return await _customerGroupService.GetListDropdownAsync();
    }
    /// <summary>
    ///  Tạo mới nhóm
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<CustomerGroupDto> CreateAsync(CreateUpdateCustomerGroupDto input)
    {
        return await _customerGroupService.CreateAsync(input);
    }

    /// <summary>
    ///  Cập nhật nhóm
    /// </summary>
    /// <param name="input"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("{id}")]
    public async Task<CustomerGroupDto> UpdateAsync(CreateUpdateCustomerGroupDto input, Guid id)
    {
        return await _customerGroupService.UpdateAsync(input, id);
    }

    /// <summary>
    ///  Xóa nhóm
    /// </summary>
    /// <param name="id"></param>
    [HttpDelete]
    [Route("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _customerGroupService.DeleteAsync(id);
    }

    /// <summary>
    /// GenerateCode
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("generate-code")]
    public async Task<string> GenerateCode()
    {
        return await _customerGroupService.GenerateCode();
    }
}