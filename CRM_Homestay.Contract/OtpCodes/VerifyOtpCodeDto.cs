using CRM_Homestay.Contract.Validations;

namespace CRM_Homestay.Contract.OtpCodes
{
    public class VerifyOtpCodeDto : IForceOverrideValidationMessage
    {
        public string? RecipientTypes { get; set; }
        public string? Recipient { get; set; }
        public string? Code { get; set; }
        public string? Purpose { get; set; }
        public string? ReferenceTypes { get; set; }
        public string? ReferenceId { get; set; }
    }

    public class VerifyOtpCodeResponseDto
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
