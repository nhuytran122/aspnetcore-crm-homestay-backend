using Microsoft.AspNetCore.Authorization;

namespace CRM_Homestay.Core.AuthorizationHandlers;

public class ClaimRequirement : IAuthorizationRequirement
{
    public string Claim { get; private set; }

    public ClaimRequirement(string claim)
    {
        Claim = claim;
    }
}