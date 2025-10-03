using CRM_Homestay.Contract.OtpCodes.Handlers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRM_Homestay.App.Attributes;

/// <summary>
/// UseOtpTokenAttribute
/// </summary>
public class UseOtpTokenAttribute : Attribute, IAsyncActionFilter
{
    /// <summary>
    /// OnActionExecutionAsync
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var otpHandler = httpContext.RequestServices.GetRequiredService<IOtpTokenHandler>();

        var otpToken = await otpHandler.ParseJwtTokenAsync();
        httpContext.Items["OtpToken"] = otpToken;

        await next();
    }
}
