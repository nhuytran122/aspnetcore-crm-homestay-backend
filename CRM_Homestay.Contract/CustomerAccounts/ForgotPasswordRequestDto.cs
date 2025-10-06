using CRM_Homestay.Contract.Validations;

namespace CRM_Homestay.Contract.CustomerAccounts;

public class ForgotPasswordRequestDto : IForceOverrideValidationMessage
{
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
}

