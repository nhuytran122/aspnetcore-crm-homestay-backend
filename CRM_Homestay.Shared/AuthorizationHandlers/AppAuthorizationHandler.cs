using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using CRM_Homestay.Core.Consts.Permissions;

namespace CRM_Homestay.Core.AuthorizationHandlers;

public class AppAuthorizationHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        var pendingRequirements = context.PendingRequirements.ToList();

        foreach (var requirement in pendingRequirements)
        {
            if (requirement is ClaimRequirement)
            {
                if (IsAllowed((ClaimRequirement)requirement, context.User))
                {
                    context.Succeed(requirement);
                }
            }
        }

        return Task.CompletedTask;
    }


    private bool IsAllowed(ClaimRequirement requirement, ClaimsPrincipal user)
    {
        if (user.HasClaim(x => x.Type == ExtendClaimTypes.Permission && x.Value == requirement.Claim))
        {
            return true;
        }
        return false;
    }
}