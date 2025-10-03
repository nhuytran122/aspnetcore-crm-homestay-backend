namespace CRM_Homestay.Contract.OtpCodes
{
    public class OtpProviderLogDto
    {
        public Guid OtpCodeId { get; set; }
        public string? ProviderName { get; set; }
        public string? RequestPayload { get; set; }
        public string? ResponsePayload { get; set; }
        public string? ErrorMessage { get; set; }
        public int StatusCode { get; set; }
        public bool IsSuccessful { get; set; }
    }
}