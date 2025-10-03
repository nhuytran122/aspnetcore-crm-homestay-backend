using CRM_Homestay.Entity.Bases;

namespace CRM_Homestay.Entity.Otps
{
    public class OtpRequest : BaseEntity
    {
        public Guid? OtpCodeId { get; set; }
        public string? ReferenceTypes { get; set; }
        public string? ReferenceId { get; set; }
        public string Recipient { get; set; } = null!;
        public string? ClientIp { get; set; }
        public string? UserAgent { get; set; }
        public virtual OtpCode? OtpCode { get; set; }
    }
}
