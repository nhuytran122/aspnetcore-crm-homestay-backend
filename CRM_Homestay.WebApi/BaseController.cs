using CRM_Homestay.Core.Consts;
using CRM_Homestay.Entity.Customers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRM_Homestay.App;

/// <summary>
/// BaseController
/// </summary>
public abstract class BaseController
{
    /// <summary>
    /// _httpContextAccessor
    /// </summary>
    protected readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// CurrentUserId
    /// </summary>
    public Guid CurrentUserId { get; set; }
    /// <summary>
    /// BaseController init
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public BaseController(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// GetCurrentUserId
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public Guid GetCurrentUserId()
    {
        var stringId = _httpContextAccessor.HttpContext?.User
            .FindFirstValue(ClaimTypes.NameIdentifier);
        Guid id;
        if (Guid.TryParse(stringId, out id))
        {
            return id;
        }
        return Guid.Empty;
    }

    /// <summary>
    /// GetCurrentRole
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public string GetCurrentRole()
    {
        return _httpContextAccessor.HttpContext?.User
            .FindFirstValue(ClaimTypes.Role)!;
    }

    /// <summary>
    /// GetCurrentCustomerAccountId
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public Guid GetCurrentCustomerAccountId()
    {
        if (_httpContextAccessor.HttpContext?.Items[CustomerAccountConst.HTTP_CONTEXT_ITEM_CUSTOMER_ACCOUNT_KEY] is CustomerAccount customerAccount)
        {
            return customerAccount.Id;
        }
        return Guid.Empty;
    }

    /// <summary>
    /// Lấy CustomerAccount từ HttpContext.Items
    /// </summary>
    [NonAction]
    public CustomerAccount GetCurrentCustomerAccount()
    {
        if (_httpContextAccessor.HttpContext?.Items[CustomerAccountConst.HTTP_CONTEXT_ITEM_CUSTOMER_ACCOUNT_KEY] is CustomerAccount customerAccount)
        {
            return customerAccount;
        }

        return new CustomerAccount();
    }

}