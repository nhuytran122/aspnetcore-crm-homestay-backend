using CRM_Homestay.Entity.Roles;
using CRM_Homestay.Entity.Users;
using Microsoft.AspNetCore.Identity;

namespace CRM_Homestay.Entity.UserRoles;

public class UserRole : IdentityUserRole<Guid>
{

    public virtual User? User { get; set; }
    public virtual Role? Role { get; set; }
}