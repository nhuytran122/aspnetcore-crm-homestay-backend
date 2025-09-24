using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Models;

namespace CRM_Homestay.Contract.Users;

public class UpdateProfileRequestDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public CreateUpdateAddressDto Address { get; set; } = new CreateUpdateAddressDto();
    public string? Email { get; set; }
    public string Introduction { get; set; } = "Hello";

    public Gender Gender { get; set; } = Gender.Male;
    public string? AvatarURL { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DOB { get; set; }
}