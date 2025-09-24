using CRM_Homestay.Contract.Validations;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Models;

namespace CRM_Homestay.Contract.Users;

public class CreateUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public CreateUpdateAddressDto Address { get; set; } = new CreateUpdateAddressDto();
    public string? Email { get; set; }
    public string Introduction { get; set; } = "Hello";

    public Gender Gender { get; set; } = Gender.Male;

    public string? UserName { get; set; }

    public string? Password { get; set; }
    public string? PasswordConfirm { get; set; }
    public string? PhoneNumber { get; set; }

    public DateTime? DOB { get; set; }

    public bool IsActive { get; set; }
    public Guid RoleId { get; set; }
    public long BaseSalary { get; set; } = 0;
}

public class CreateUserKtvDto : IForceOverrideValidationMessage
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public Gender Gender { get; set; } = Gender.Male;
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? PasswordConfirm { get; set; }
    public string? PhoneNumber { get; set; }
}