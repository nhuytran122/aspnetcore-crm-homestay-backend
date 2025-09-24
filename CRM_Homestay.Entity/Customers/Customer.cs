using System.ComponentModel.DataAnnotations.Schema;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Models;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.Bookings;
using CRM_Homestay.Entity.Coupons;
using CRM_Homestay.Entity.CustomerGroups;
using CRM_Homestay.Entity.Users;

namespace CRM_Homestay.Entity.Customers;

public class Customer : BaseEntity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public DateTime? DOB { get; set; }
    public CustomerTypes Type { get; set; } = CustomerTypes.Individual;
    public string? CompanyName { get; set; }
    public string? NormalizedCompanyName { get; set; }
    public string? TaxCode { get; set; }
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public Gender Gender { get; set; } = Gender.Unknown;
    public DateTime? DeletedAt { get; set; }
    [Column(TypeName = "json")]
    public Address? Address { get; set; }

    public Guid GroupId { get; set; }

    //navigation
    public CustomerGroup? Group { get; set; }
    public string? NormalizeFullInfo { get; set; }
    public string? NormalizeAddress { get; set; }
    public CustomerAccount? CustomerAccount { get; set; }
    public List<Coupon>? Coupons { get; set; }
    public List<Booking>? Bookings { get; set; }
}