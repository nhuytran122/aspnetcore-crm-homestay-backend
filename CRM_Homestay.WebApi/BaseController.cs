using CRM_Homestay.Core.Consts;
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

}