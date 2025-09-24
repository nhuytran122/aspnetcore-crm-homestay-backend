using System.Security.Claims;
using CRM_Homestay.App.Attributes;
using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using CRM_Homestay.Contract.Users;

namespace CRM_Homestay.App.Middlewares
{
    /// <summary>
    /// TokenAuthorizationMiddleware
    /// </summary>
    public class TokenAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// TokenAuthorizationMiddleware init
        /// </summary>
        /// <param name="next"></param>
        public TokenAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// InvokeAsync
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // Chỉ kiểm tra các endpoint có authentication
            var endpoint = context.GetEndpoint();
            if (endpoint == null)
            {
                await _next(context);
                return;
            }

            var allowAnonymous = endpoint.Metadata.GetMetadata<IAllowAnonymous>();

            // Nếu có AllowAnonymous thì bỏ qua kiểm tra
            if (allowAnonymous != null)
            {
                await _next(context);
                return;
            }

            var type = context.User.Claims.FirstOrDefault(x => x.Type == CustomerAccountConst.CLAIM_TYPE_KEY)?.Value;

            if (endpoint.Metadata.Any(m => m is AuthorizeAttribute))
            {
                var userId = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                // Kiểm tra token có chứa type, và userId là null => customer token
                if (context.User.Identity!.IsAuthenticated && type != null && userId == null)
                {
                    context = await ModifyHttpContext(context);
                    return;
                }
            }

            var customerAuthorizeAttribute = endpoint.Metadata.GetMetadata<CustomerAuthorizeAttribute>();
            if (customerAuthorizeAttribute != null && type == null)
            {
                context = await ModifyHttpContext(context);
                return;
            }

            if (type == Core.Enums.TokenTypes.user_token.ToString())
            {

                var userAccountId = context.User.Claims.FirstOrDefault(x => x.Type == CustomerAccountConst.CLAIM_CUSTOMER_ACCOUNT_ID_KEY)?.Value;

                if (string.IsNullOrEmpty(userAccountId) || !Guid.TryParse(userAccountId, out var id))
                {
                    context = await ModifyHttpContext(context);
                    return;
                }
                var userAccountService = context.RequestServices.GetRequiredService<IUserService>();

                var userAccount = await userAccountService.GetAsync(id);
                if (userAccount == null)
                {
                    context = await ModifyHttpContext(context);
                    return;
                }
                context.Items[CustomerAccountConst.HTTP_CONTEXT_ITEM_CUSTOMER_ACCOUNT_KEY] = userAccount;
            }

            await _next(context);
            return;
        }

        private async Task<HttpContext> ModifyHttpContext(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var jsonResponse = @"{
                ""Messenger"": ""Unauthorized"",
                ""Code"": 401
                }";
            await context.Response.WriteAsync(jsonResponse);
            return context;
        }
    }
}