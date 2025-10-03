namespace CRM_Homestay.Contract.OtpCodes
{
    public interface IOtpCodeService
    {
        Task<OtpCodeDto> SendAsync(SendOtpCodeDto dto);
        Task<VerifyOtpCodeResponseDto> VerifyAsync(VerifyOtpCodeDto dto);
    }
}
