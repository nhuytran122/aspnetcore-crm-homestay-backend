using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Models;
using CRM_Homestay.Entity.Customers;
using CRM_Homestay.Entity.Users;

namespace CRM_Homestay.Contract.Customers;

public class CustomerDto : BaseEntityDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }
    public DateTime? DOB { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public Gender Gender { get; set; } = Gender.Unknown;
    public CustomerTypes Type { get; set; } = CustomerTypes.Individual;
    public string? CompanyName { get; set; }
    public string? TaxCode { get; set; }
    public Address Address { get; set; } = new Address();
    public Guid GroupId { get; set; }
    public string? GroupName { get; set; }
    public bool? IsGatePassSent { get; set; }
    public DateTime? LastVisit { get; set; }
    public DateTime? NextVisit { get; set; }
    public decimal TotalPaid { get; set; } = 0;
    public int TotalVisited { get; set; } = 0;
    public string? NormalizeFullInfo { get; set; }
    public string? NormalizeAddress { get; set; }
    public CustomerAccount? CustomerAccount { get; set; }
}