using CRM_Homestay.Core.Enums;
using CRM_Homestay.Entity.Bases;

namespace CRM_Homestay.Entity.Otps
{
    public class OtpCode : BaseEntity
    {
        public string RecipientTypes { get; set; } = "";
        public string Recipient { get; set; } = "";
        public string Purpose { get; set; } = "";
        public string ReferenceTypes { get; set; } = "";
        public string ReferenceId { get; set; } = "";
        public string Code { get; set; } = "";

        public int Minutes { get; set; } = 5;
        public DateTime ExpiresAt { get; set; }
        public OtpStatuses Status { get; set; }
        public byte Attempts { get; set; } = 0;
        public byte MaxAttempts { get; set; } = 5;
        public DateTime? LockedUntil { get; set; }
    }
}
