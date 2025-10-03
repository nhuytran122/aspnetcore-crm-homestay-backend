using CRM_Homestay.Contract.OtpCodes;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Entity.Otps;

namespace CRM_Homestay.Service.OtpCodes.Providers
{
    public class DumpOtpProvider : IOtpProvider
    {
        // Implement the Code property
        public string ProviderName => RecipientTypes.dump.ToString();

        // Implement the Send method
        public Task<OtpProviderLogDto> SendAsync(string code, OtpCode otpCode)
        {
            // TODO: Implement email sending logic here using otpCode
            return Task.FromResult(new OtpProviderLogDto()
            {
                OtpCodeId = otpCode.Id,
                StatusCode = 200,
                RequestPayload = code,
                ResponsePayload = code,
                IsSuccessful = true,
                ProviderName = ProviderName,
            });
        }

        // Implement the Validate method
        public Task<SendOtpCodeDto> ValidateAsync(SendOtpCodeDto dto)
        {
            // TODO: Implement validation logic here
            // dto.RecipientTypes = RecipientTypes.phone.ToString();
            return Task.FromResult(dto);
        }

        // Implement the GetRecipientField method
        public string GetRecipientField(SendOtpCodeDto dto)
        {
            return "PhoneNumber";
        }

        // Implement the GetLockedUntil method
        public Task<DateTime?> GetLockedUntil(SendOtpCodeDto dto, object context)
        {
            // TODO: Return the locked until value if applicable
            return Task.FromResult<DateTime?>(null);
        }
    }
}