using System.ComponentModel.DataAnnotations.Schema;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Models;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.AuditLogs;
using CRM_Homestay.Entity.UserRoles;
using Microsoft.AspNetCore.Identity;
using CRM_Homestay.Entity.ImportProducts;
using CRM_Homestay.Entity.BookingPayments;
using CRM_Homestay.Entity.BookingServices;

namespace CRM_Homestay.Entity.Users;

public class User : IdentityUser<Guid>, IBaseEntity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName
    {
        get
        {
            return FirstName == null ? LastName : FirstName + " " + LastName;
        }
    }
    public DateTime? DOB { get; set; }
    public string Introduction { get; set; } = "Hello";
    public Gender Gender { get; set; } = Gender.Unknown;
    public bool IsActive { get; set; }
    public bool IsDelete { get; set; }
    public string? AvatarURL { get; set; }

    [Column(TypeName = "json")]
    public Address? Address { get; set; }
    public bool IsLoggedIn { get; set; } = false;
    public string NormalizeFullInfo { get; set; } = string.Empty;
    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public Guid? CreatorId { get; set; }
    public long BaseSalary { get; set; }

    //navigation
    public ICollection<UserRole>? UserRoles { get; set; }
    public List<AuditLog>? AuditLogs { get; set; }
    public string? NormalizeAddress { get; set; }

    public List<ImportProduct>? ImportProducts { get; set; }
    public List<BookingPayment>? BookingPayments { get; set; }
    public List<BookingService>? AssignedServices { get; set; }
}

