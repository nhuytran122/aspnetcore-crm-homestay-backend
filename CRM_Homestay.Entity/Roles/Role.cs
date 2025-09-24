using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.UserRoles;
using Microsoft.AspNetCore.Identity;

namespace CRM_Homestay.Entity.Roles;

public class Role : IdentityRole<Guid>, IBaseEntity
{
    public string Description { get; set; } = string.Empty;

    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public Guid? CreatorId { get; set; }
    public ICollection<UserRole>? UserRoles { get; set; }
}


