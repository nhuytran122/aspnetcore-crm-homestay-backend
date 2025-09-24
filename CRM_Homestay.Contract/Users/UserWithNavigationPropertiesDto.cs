using CRM_Homestay.Contract.Roles;

namespace CRM_Homestay.Contract.Users;

public class UserWithNavigationPropertiesDto
{
    public UserDto User { get; set; } = new UserDto();
    public RoleDto Role { get; set; } = new RoleDto();

}