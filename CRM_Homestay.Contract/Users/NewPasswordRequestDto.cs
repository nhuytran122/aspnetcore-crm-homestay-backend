namespace CRM_Homestay.Contract.Users;

public class NewPasswordRequestDto
{
    public string? OldPassword { get; set; }

    public string? NewPassword { get; set; }

    public string? PasswordConfirm { get; set; }
}
