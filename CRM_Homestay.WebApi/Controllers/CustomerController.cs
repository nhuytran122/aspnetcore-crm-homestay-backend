using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.Customers;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Entity.Customers;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Homestay.App.Controllers;

/// <summary>
/// CustomerController
/// </summary>
[ApiController]
[Route("api/customers/")]
[Authorize]
public class CustomerController : BaseController
{
    private ICustomerService _customerService;
    /// <summary>
    /// CustomerController init
    /// </summary>
    /// <param name="customerService"></param>
    /// <param name="httpContextAccessor"></param>
    public CustomerController(ICustomerService customerService, [NotNull] IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        _customerService = customerService;
    }

    /// <summary>
    ///  Lấy ra danh sách khách hàng cùng với chi tiết
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("get-list-with-navigation-properties")]
    public async Task<PagedResultDto<CustomerWithNavigationPropertiesDto>> GetListWithNavigationPropertiesAsync([FromQuery] CustomerFilterDto filter)
    {
        return await _customerService.GetListWithNavigationPropertiesAsync(filter);
    }

    /// <summary>
    /// GetListAsync
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<List<CustomerDto>> GetListAsync()
    {
        return await _customerService.GetListAsync();
    }

    /// <summary>
    /// Tạo khách hàng
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<CustomerDto> CreateAsync(CreateUpdateCustomerDto input)
    {
        return await _customerService.CreateAsync(input);
    }

    /// <summary>
    ///  Cập nhật khách hàng
    /// </summary>
    /// <param name="input"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<CustomerDto> UpdateAsync(CreateUpdateCustomerDto input, Guid id)
    {
        return await _customerService.UpdateAsync(input, id);
    }

    /// <summary>
    ///  Xóa khách hàng
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _customerService.DeleteAsync(id);
    }

    /// <summary>
    /// Load danh sách khách hàng (50 record) kết hợp điều kiện và: tên và sdt và địa chỉ, phân tách bởi dấu ','
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    [HttpGet("filter")]
    public async Task<PagedResultDto<CustomerDto>> GetCustomerWithFilter(string? filter)
    {
        return await _customerService.GetCustomerWithFilterAsync(filter);
    }

    /// <summary>
    /// Lấy thông tin chi tiết khách hàng
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<CustomerDto> GetById(Guid id)
    {
        return await _customerService.GetByIdAsync(id);
    }
}