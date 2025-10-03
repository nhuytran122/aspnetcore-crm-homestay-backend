using CRM_Homestay.Entity.Bases;

namespace CRM_Homestay.Entity.Otps
{
    public class OtpProviderLog : BaseEntity
    {
        public Guid OtpCodeId { get; set; }
        public string ProviderName { get; set; } = "";
        public string? RequestPayload { get; set; }
        public string? ResponsePayload { get; set; }
        public int? StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
        public virtual OtpCode? OtpCode { get; set; }
    }
}
