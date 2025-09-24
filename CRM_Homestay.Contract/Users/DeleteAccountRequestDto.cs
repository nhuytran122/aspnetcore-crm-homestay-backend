using CRM_Homestay.Contract.Validations;

namespace CRM_Homestay.Contract.Users;

public class DeleteAccountRequestDto : IForceOverrideValidationMessage
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
}
