using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Entity.Customers;
using CRM_Homestay.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRM_Homestay.App.Attributes;

/// <summary>
/// CustomerAuthorizeAttribute
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CustomerAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    /// <summary>
    /// AllowInActive
    /// </summary>
    public bool AllowInActive { get; }

    /// <summary>
    /// CustomerAuthorizeAttribute init
    /// </summary>
    /// <param name="allowInActive"></param>
    public CustomerAuthorizeAttribute(bool allowInActive = false)
    {
        AllowInActive = allowInActive;
    }

    /// <summary>
    /// OnAuthorizationAsync
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var httpContext = context.HttpContext;
        var user = httpContext.User;

        var localizer = httpContext.RequestServices.GetRequiredService<ILocalizer>();

        var type = user.FindFirst("Type")?.Value;
        if (type != nameof(TokenTypes.customer_token))
        {
            context.Result = new JsonResult(new
            {
                success = false,
                statusCode = StatusCodes.Status401Unauthorized,
                code = CustomerAccountErrorCode.Unauthorized,
                message = localizer[CustomerAccountErrorCode.Unauthorized].Value
            })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };

            return Task.CompletedTask;
        }

        if (!httpContext.Items.TryGetValue("CustomerAccount", out var customerObj) || customerObj is not CustomerAccount customerAccount)
        {
            context.Result = new JsonResult(new
            {
                success = false,
                statusCode = StatusCodes.Status401Unauthorized,
                code = CustomerAccountErrorCode.Unauthorized,
                message = localizer[CustomerAccountErrorCode.Unauthorized].Value
            })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };

            return Task.CompletedTask;
        }

        if (!AllowInActive && (customerAccount.Status != CustomerStatuses.Active || customerAccount.CustomerId == null || customerAccount.CustomerId == Guid.Empty))
        {
            context.Result = new JsonResult(new
            {
                success = false,
                statusCode = StatusCodes.Status403Forbidden,
                code = CustomerAccountErrorCode.Forbidden,
                message = localizer[CustomerAccountErrorCode.Forbidden].Value
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        return Task.CompletedTask;
    }
}