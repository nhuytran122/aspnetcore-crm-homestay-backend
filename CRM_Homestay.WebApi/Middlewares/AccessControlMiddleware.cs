using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Database.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CRM_Homestay.App.Middlewares;

/// <summary>
/// 
/// </summary>
/// 
public class AccessControlMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="next"></param>
    /// <param name="serviceProvider"></param>
    public AccessControlMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="GlobalException"></exception>
    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value!;

        if (!IsEndpointExempt(path))
        {
            var userId = GetUserIdFromContext(context);

            if (userId != null && !await IsUserLoggedIn(userId.Value))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var jsonResponse = @"{
                ""Messenger"": ""Unauthorized"",
                ""Code"": 401
                }";
                await context.Response.WriteAsync(jsonResponse);
                return;
            }

            var customerAccountId = GetCustomerIdFromContext(context);
            var token = GetTokenFromContext(context);
            if (customerAccountId != null && token != null && !await IsUserCustomerLoggedIn(customerAccountId.Value, token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var jsonResponse = @"{
                ""success"": ""false"",
                ""statusCode"": ""401"",
                ""message"": ""Unauthorized"",
                ""code"": 401
                }";
                await context.Response.WriteAsync(jsonResponse);
                return;
            }
        }

        await _next(context);
    }

    private bool IsEndpointExempt(string path)
    {
        return path.Contains("/api/user/sign-in") || path.Contains("/api/user/get-personal-login-status");
    }

    private Guid? GetUserIdFromContext(HttpContext context)
    {
        var claim = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        var tokenType = context.User.Claims.FirstOrDefault(x => x.Type == "Type");
        if ((tokenType == null || tokenType.Value != TokenTypes.customer_token.ToString()) && claim != null && Guid.TryParse(claim.Value, out Guid userId))
        {
            return userId;
        }
        return null;
    }

    private async Task<bool> IsUserLoggedIn(Guid userId)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<HomestayContext>();
            return await dbContext.Users.AnyAsync(x => x.Id == userId && x.IsLoggedIn && !x.IsDelete);
        }
    }

    private async Task<bool> IsUserCustomerLoggedIn(Guid customerAccountId, string token)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<HomestayContext>();
            return await dbContext.CustomerAccountTokens.AsNoTracking().AnyAsync(x => x.CustomerAccountId == customerAccountId && x.Token == token);
        }
    }

    private string? GetTokenFromContext(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }
        return null;
    }

    private Guid? GetCustomerIdFromContext(HttpContext context)
    {
        var claim = context.User.Claims.FirstOrDefault(x => x.Type == CustomerAccountConst.CLAIM_CUSTOMER_ID_KEY);
        var tokenType = context.User.Claims.FirstOrDefault(x => x.Type == "Type");
        if (tokenType != null && tokenType.Value == nameof(TokenTypes.customer_token) && claim != null && Guid.TryParse(claim.Value, out Guid userId))
        {
            return userId;
        }
        return null;
    }
}