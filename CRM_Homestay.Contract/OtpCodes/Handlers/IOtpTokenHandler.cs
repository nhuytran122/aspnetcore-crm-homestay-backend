using CRM_Homestay.Entity.Otps;

namespace CRM_Homestay.Contract.OtpCodes.Handlers;

public interface IOtpTokenHandler
{
    Task<OtpCode> ParseJwtTokenAsync();
}
