using System.Security.Claims;
using CRM_Homestay.Contract.Claims;
using CRM_Homestay.Contract.RoleClaims;

namespace CRM_Homestay.Contract.Roles;

public interface IRoleService
{
    Task<List<RoleDto>> GetListAsync();
    Task DeleteAsync(Guid id);
    Task<RoleDto> CreateAsync(CreateUpdateRoleDto input, bool save = true);
    Task<RoleDto> UpdateAsync(CreateUpdateRoleDto input, Guid id, bool save = true);
    Task CreateWithClaimsAsync(CreateUpdateRoleDto input);
    Task UpdateWithClaimsAsync(CreateUpdateRoleDto input, Guid id);
    Task UpdateClaimsToRole(Guid id, List<CreateUpdateClaimDto> claims);
    Task<List<ClaimDto>> GetClaims(Guid roleId);
}