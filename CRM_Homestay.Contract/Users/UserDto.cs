using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Models;

namespace CRM_Homestay.Contract.Users;

public class UserDto : BaseEntityDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }
    public string? UserName { get; set; }
    public Address? Address { get; set; }
    public string? Email { get; set; }
    public string? Introduction { get; set; } = "Hello";
    public string? AvatarURL { get; set; }
    public Gender Gender { get; set; } = Gender.Unknown;
    public string? PhoneNumber { get; set; }
    public DateTime? DOB { get; set; }
    public bool? IsLoggedIn { get; set; }
    public bool? IsActive { get; set; }
    public string? RoleName { get; set; }
    public long BaseSalary { get; set; }
}