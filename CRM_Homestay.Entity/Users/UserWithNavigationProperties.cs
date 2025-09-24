using CRM_Homestay.Entity.Roles;

namespace CRM_Homestay.Entity.Users;

public class UserWithNavigationProperties
{
    public User? User { get; set; }
    public Role? Role { get; set; }
}