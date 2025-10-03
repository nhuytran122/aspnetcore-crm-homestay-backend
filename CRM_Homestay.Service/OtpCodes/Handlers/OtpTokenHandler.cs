using CRM_Homestay.Contract.OtpCodes.Handlers;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.Otps;
using CRM_Homestay.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace CRM_Homestay.Service.OtpCodes.Handlers;

public class OtpTokenHandler : IOtpTokenHandler
{
    private readonly Localizer L;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OtpTokenHandler(Localizer localizer, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        L = localizer;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<OtpCode> ParseJwtTokenAsync()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["x-otp-token"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new GlobalException(
                code: CustomerAccountErrorCode.Unauthorized,
                message: L[CustomerAccountErrorCode.Unauthorized],
                statusCode: HttpStatusCode.Unauthorized
            );
        }

        JwtSecurityTokenHandler tokenHandler = new();
        JwtSecurityToken jwtToken;

        try
        {
            jwtToken = tokenHandler.ReadJwtToken(token);
        }
        catch
        {
            throw new GlobalException(
                code: CustomerAccountErrorCode.Unauthorized,
                message: L[CustomerAccountErrorCode.Unauthorized],
                statusCode: HttpStatusCode.BadRequest
            );
        }

        // Extract claims
        var type = jwtToken.Claims.FirstOrDefault(c => c.Type == "type")?.Value;
        var purpose = jwtToken.Claims.FirstOrDefault(c => c.Type == "purpose")?.Value;
        var tokenCodeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "tokenCodeId")?.Value;
        var recipient = jwtToken.Claims.FirstOrDefault(c => c.Type == "recipient")?.Value;
        var RecipientTypes = jwtToken.Claims.FirstOrDefault(c => c.Type == "RecipientTypes")?.Value;
        var referenceId = jwtToken.Claims.FirstOrDefault(c => c.Type == "referenceId")?.Value;
        var ReferenceTypes = jwtToken.Claims.FirstOrDefault(c => c.Type == "ReferenceTypes")?.Value;

        // Validate required fields
        if (type != TokenTypes.otp_token.ToString())
        {
            throw new GlobalException(
                code: CustomerAccountErrorCode.OtpInvalidTokenType,
                message: L[CustomerAccountErrorCode.OtpInvalidTokenType],
                statusCode: HttpStatusCode.BadRequest
            );
        }

        if (string.IsNullOrEmpty(tokenCodeId) || !Guid.TryParse(tokenCodeId, out var tokenCodeGuid))
        {
            throw new GlobalException(
                code: CustomerAccountErrorCode.OtpInvalidTokenCodeId,
                message: L[CustomerAccountErrorCode.OtpInvalidTokenCodeId],
                statusCode: HttpStatusCode.BadRequest
            );
        }

        // Kiểm tra dữ liệu trong token có trong table OtpCodes hay không
        var otpCode = await _unitOfWork.NewGenericRepository<OtpCode>().GetQueryable().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == tokenCodeGuid
            && x.Purpose == purpose
            && x.Recipient == recipient
            && x.RecipientTypes == RecipientTypes
            && x.ReferenceId == referenceId
            && x.ReferenceTypes == ReferenceTypes
            && x.Status == OtpStatuses.Verified);

        if (otpCode == null)
        {
            throw new GlobalException(code: CustomerAccountErrorCode.Unauthorized,
                                  message: L[CustomerAccountErrorCode.Unauthorized],
                                  statusCode: HttpStatusCode.NotFound);
        }

        return otpCode;
    }
}