using CRM_Homestay.Contract.Validations;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Models;
using CRM_Homestay.Entity.Customers;

namespace CRM_Homestay.Contract.CustomerAccounts;

public class ProfileDto
{
    public Guid Id { get; set; }
    public string? PhoneNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public DateTime? DOB { get; set; }
    public Gender Gender { get; set; }
    public Address? Address { get; set; }
    public PointWalletDto? PointWallet { get; set; }
    public MetaDto? Meta { get; set; }
    public Customer customer { get; set; } = new Customer();
}

public class ProfileRequestDto : IForceOverrideValidationMessage
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public DateTime? DOB { get; set; }
    public Gender Gender { get; set; }
    public CreateUpdateAddressDto Address { get; set; } = new CreateUpdateAddressDto();
}

public class MetaDto
{
    public RequirementDto Requirements { get; set; } = new();
}

public class RequirementDto
{
    public string? NextStep { get; set; }
    public string? Message { get; set; }
    public RequirementActionDto Action { get; set; } = new();
    public ProfileCustomerDto? Data { get; set; }
}

public class RequirementActionDto
{
    public string Type { get; set; } = "navigate";
    public RequirementParamsDto Params { get; set; } = new();
}

public class RequirementParamsDto
{
    public string? RecipientTypes { get; set; }
    public string? Recipient { get; set; }
    public string? ReferenceTypes { get; set; }
    public string? ReferenceId { get; set; }
    public string? Purpose { get; set; }
    public string? CustomerId { get; set; }
}

public class ProfileCustomerDto
{
    public string? PhoneNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public DateTime? DOB { get; set; }
    public Gender Gender { get; set; }
    public Address? Address { get; set; }
}

public class PointWalletDto
{
    public Guid Id { get; set; }
    public int TotalAccumulated { get; set; }
    public int TotalUsed { get; set; }
    public int CurrentBalance { get; set; }
    public int HoldPoint { get; set; }
}