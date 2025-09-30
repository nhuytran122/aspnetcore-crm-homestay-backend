using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Models;

namespace CRM_Homestay.Contract.Customers;

public class CreateUpdateCustomerDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DOB { get; set; }
    public string Email { get; set; } = string.Empty;
    public List<string>? PhoneNumber { get; set; }
    public Gender Gender { get; set; } = Gender.Unknown;
    public CustomerTypes Type { get; set; } = CustomerTypes.Individual;
    public string? CompanyName { get; set; }
    public string? TaxCode { get; set; }
    public CreateUpdateAddressDto Address { get; set; } = new CreateUpdateAddressDto();
    public Guid? GroupId { get; set; }
}