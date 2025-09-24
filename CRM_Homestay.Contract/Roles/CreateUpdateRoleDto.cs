using CRM_Homestay.Contract.Claims;

namespace CRM_Homestay.Contract.Roles;

public class CreateUpdateRoleDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<CreateUpdateClaimDto> Claims { get; set; } = new List<CreateUpdateClaimDto>();
}