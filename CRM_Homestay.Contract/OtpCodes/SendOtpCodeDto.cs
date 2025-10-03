using CRM_Homestay.Contract.Validations;

namespace CRM_Homestay.Contract.OtpCodes
{
    public class SendOtpCodeDto : IForceOverrideValidationMessage
    {
        public string RecipientTypes { get; set; } = "";
        public string Recipient { get; set; } = "";
        public string Purpose { get; set; } = "";

        public int Minutes { get; set; } = 5;
        public string ReferenceTypes { get; set; } = "";
        public string? ReferenceId { get; set; }
    }
}
