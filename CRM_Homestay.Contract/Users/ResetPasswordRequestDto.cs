using CRM_Homestay.Contract.Validations;

namespace CRM_Homestay.Contract.Users;

public class ResetPasswordRequestDto : IForceOverrideValidationMessage
{
    public string? NewPassword { get; set; }
    public string? PasswordConfirm { get; set; }

}