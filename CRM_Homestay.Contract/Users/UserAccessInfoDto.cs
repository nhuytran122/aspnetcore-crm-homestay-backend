using CRM_Homestay.Contract.Roles;
using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.Users;

public class UserAccessInfoDto
{
    public Guid Id { get; set; }
    public RoleDto Role { get; set; } = new RoleDto();
    public string? AccessToken { get; set; }

}