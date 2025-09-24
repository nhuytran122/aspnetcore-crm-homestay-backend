namespace CRM_Homestay.Contract.Users;

public class UserPropertiesDto
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AvatarURL { get; set; }
}