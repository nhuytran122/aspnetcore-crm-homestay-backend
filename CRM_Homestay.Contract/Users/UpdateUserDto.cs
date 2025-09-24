using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Models;

namespace CRM_Homestay.Contract.Users;

public class UpdateUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public CreateUpdateAddressDto? Address { get; set; }
    public string? Email { get; set; }
    public Gender Gender { get; set; } = Gender.Unknown;
    public string? PhoneNumber { get; set; }
    public DateTime? DOB { get; set; }
    public bool IsActive { get; set; }
    public Guid RoleId { get; set; } = Guid.Empty;
    public long BaseSalary { get; set; } = 0;
}