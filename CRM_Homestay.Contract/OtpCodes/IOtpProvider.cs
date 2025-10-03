using CRM_Homestay.Entity.Otps;

namespace CRM_Homestay.Contract.OtpCodes
{
    public interface IOtpProvider
    {
        string ProviderName { get; }

        // get column to check code
        string GetRecipientField(SendOtpCodeDto sendOtpCodeDto);
        Task<SendOtpCodeDto> ValidateAsync(SendOtpCodeDto sendOtpCodeDto);
        Task<DateTime?> GetLockedUntil(SendOtpCodeDto sendOtpCodeDto, object Reference);
        Task<OtpProviderLogDto> SendAsync(string code, OtpCode otpCode);

    }
}