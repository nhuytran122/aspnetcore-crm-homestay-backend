using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.OtpCodes
{
    public class OtpCodeDto
    {
        public Guid Id { get; set; }

        public Guid ReferenceId { get; set; }
        public OtpStatuses Status { get; set; }
        public byte Attempts { get; set; } = 0;
        public DateTime ExpiresAt { get; set; }
        public DateTime? LockedUntil { get; set; }
        public string? Code { get; set; }
    }
}
