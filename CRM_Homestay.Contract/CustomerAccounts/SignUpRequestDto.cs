using CRM_Homestay.Contract.Validations;

namespace CRM_Homestay.Contract.CustomerAccounts;

public class SignUpRequestDto : IForceOverrideValidationMessage
{
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public string? Gender { get; set; }
}
