namespace CRM_Homestay.Contract.OtpCodes
{
    public class OtpTokenDto
    {
        public Guid TokenCodeId { get; set; }
        public string? ReferenceId { get; set; }
        public string ReferenceTypes { get; set; } = string.Empty;
        public string RecipientTypes { get; set; } = string.Empty;
        public string Recipient { get; set; } = string.Empty;
        public string Type { get; set; } = "otp_token";
        public string Purpose { get; set; } = string.Empty;
        public int ExpiresIn { get; set; } = 600; // 10 phút
    }
}
