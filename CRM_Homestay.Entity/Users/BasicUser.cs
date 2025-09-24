namespace CRM_Homestay.Entity.Users;

public class BasicUser
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? AvatarURL { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
}