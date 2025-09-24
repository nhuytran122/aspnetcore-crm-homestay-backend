using System.Text.Json.Serialization;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Entity.Bases;

namespace CRM_Homestay.Entity.Customers
{
    public class CustomerAccount : BaseEntity
    {
        public string PhoneNumber { get; set; } = "";
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string PasswordHash { get; set; } = "";
        public Gender Gender { get; set; } = Gender.Unknown;
        public DateTime? PhoneVerifiedAt { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }
        public Guid? CustomerId { get; set; }
        public DateTime? DeletedAt { get; set; }
        public CustomerStatuses Status { get; set; }

        [JsonIgnore]
        public Customer? Customer { get; set; }

        public List<CustomerAccountToken>? CustomerAccountTokens { get; set; }
    }
}
