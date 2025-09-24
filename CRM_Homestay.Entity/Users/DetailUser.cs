using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Entity.Users;

public class DetailUser : BasicUser
{
    public int Age { get; set; }
    public Cities City { get; set; } = Cities.Unknown;
    public Gender Gender { get; set; }

}