using CRM_Homestay.Contract.OtpCodes;
using CRM_Homestay.Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Homestay.App.Controllers;

/// <summary>
/// OtpCode Controller
/// </summary>
[ApiController]
[Route("api/otp/")]
public class OtpCodeController : BaseController
{
    private readonly IOtpCodeService _otpCodeService;

    /// <summary>
    /// OtpCode Controller init
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    /// <param name="otpCodeService"></param>
    public OtpCodeController(IHttpContextAccessor httpContextAccessor, IOtpCodeService otpCodeService) : base(httpContextAccessor)
    {
        _otpCodeService = otpCodeService;
    }

    /// <summary>
    /// Gửi OTP mới
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("send")]
    public async Task<OtpCodeDto> SendAsync(SendOtpCodeDto dto)
    {
        if (dto.RecipientTypes == RecipientTypes.dump.ToString())
        {
            dto.RecipientTypes = "CAN_NOT_USE";
        }
        return await _otpCodeService.SendAsync(dto);
    }

    /// <summary>
    /// Xác minh OTP
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("verify")]
    public async Task<VerifyOtpCodeResponseDto> VerifyAsync(VerifyOtpCodeDto dto)
    {
        return await _otpCodeService.VerifyAsync(dto);
    }
}