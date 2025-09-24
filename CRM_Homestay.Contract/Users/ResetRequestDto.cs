namespace CRM_Homestay.Contract.Users;

public class ResetRequestDto
{
    public string? NewPassword { get; set; }
    public string? PasswordConfirm { get; set; }

}