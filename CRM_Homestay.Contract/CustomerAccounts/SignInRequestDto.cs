using CRM_Homestay.Contract.Validations;

namespace CRM_Homestay.Contract.CustomerAccounts;

public class SignInRequestDto : IForceOverrideValidationMessage
{
    public string? PhoneNumber { get; set; }
    public string? Password { get; set; }
}
