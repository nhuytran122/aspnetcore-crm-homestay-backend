namespace CRM_Homestay.Contract.Users;

public class PasswordChangeRequestDto
{
    public Guid UserId { get; set; }
    public string? OldPassword { get; set; }
    public string? NewPassword { get; set; }
}