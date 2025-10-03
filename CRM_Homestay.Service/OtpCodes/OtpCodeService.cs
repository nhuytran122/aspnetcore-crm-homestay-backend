using AutoMapper;
using CRM_Homestay.Contract.OtpCodes;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Core.Helpers;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.Customers;
using CRM_Homestay.Entity.Otps;
using CRM_Homestay.Entity.Users;
using CRM_Homestay.Localization;
using CRM_Homestay.Service.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace CRM_Homestay.Service.OtpCodes
{
    public class OtpCodeService : BaseService, IOtpCodeService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IConfiguration _configuration;
        private IServiceProvider _serviceProvider;

        private IOtpProvider? _otpProvider = null;

        public OtpCodeService(IUnitOfWork unitOfWork, IMapper mapper, ILocalizer l, IConfiguration configuration,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, mapper, l)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        protected IOtpProvider GetOtpProvider(string? providerName = null)
        {

            if (providerName != null && _otpProvider == null)
            {
                _otpProvider = _serviceProvider.GetServices<IOtpProvider>().FirstOrDefault(h => h.ProviderName == providerName);
            }

            if (_otpProvider == null)
            {
                throw new GlobalException(code: OtpCodeErrorCode.ValidationFailed,
                        message: L[OtpCodeErrorCode.InvalidReferenceTypes],
                        statusCode: HttpStatusCode.BadRequest);
            }

            return _otpProvider;
        }


        protected async Task<object> GetReference(SendOtpCodeDto dto)
        {
            var handler = GetOtpProvider();
            if (dto.ReferenceTypes == ReferenceTypes.CustomerAccount.ToString())
            {
                if (!string.IsNullOrWhiteSpace(dto.ReferenceId))
                {
                    if (!Guid.TryParse(dto.ReferenceId, out var referenceId))
                    {
                        throw new GlobalException(code: OtpCodeErrorCode.ValidationFailed,
                            message: L[OtpCodeErrorCode.ValidationFailed],
                            statusCode: HttpStatusCode.BadRequest);
                    }

                    // Check reference có tồn tại hay không
                    var reference = await _unitOfWork.GenericRepository<CustomerAccount>().GetQueryable().FirstOrDefaultAsync(x => x.Id == referenceId && (x.Status != CustomerStatuses.Deleted || x.DeletedAt == null));
                    if (reference != null)
                    {
                        return reference;
                    }

                }
                else
                {
                    var instance = await _unitOfWork.GenericRepository<CustomerAccount>().GetQueryable()
                            .Where(x => EF.Property<string>(x, handler.GetRecipientField(dto)) == dto.Recipient)
                            .FirstOrDefaultAsync(x => x.Status != CustomerStatuses.Deleted || x.DeletedAt == null);

                    if (instance != null)
                    {
                        return instance;
                    }

                }
            }
            else if (dto.ReferenceTypes == ReferenceTypes.StaffAccount.ToString())
            {
                if (!string.IsNullOrWhiteSpace(dto.ReferenceId))
                {
                    if (!Guid.TryParse(dto.ReferenceId, out var referenceId))
                    {
                        throw new GlobalException(code: OtpCodeErrorCode.ValidationFailed,
                            message: L[OtpCodeErrorCode.ValidationFailed],
                            statusCode: HttpStatusCode.BadRequest);
                    }
                    // Check reference có tồn tại hay không
                    var reference = await _unitOfWork.GenericRepository<User>().GetQueryable().FirstOrDefaultAsync(x => x.Id == referenceId && x.IsActive);
                    if (reference != null)
                    {
                        return reference;
                    }
                }
                else
                {
                    var instance = await _unitOfWork.GenericRepository<User>().GetQueryable()
                            .Where(x => EF.Property<string>(x, handler.GetRecipientField(dto)) == dto.Recipient)
                            .FirstOrDefaultAsync(x => x.IsActive && !x.DeletedAt.HasValue);

                    if (instance != null)
                    {
                        return instance;
                    }
                }
            }
            else
            {
                throw new GlobalException(code: OtpCodeErrorCode.InvalidReferenceTypes,
                message: L[OtpCodeErrorCode.InvalidReferenceTypes],
                statusCode: HttpStatusCode.BadRequest);
            }
            throw new GlobalException(code: OtpCodeErrorCode.ReferenceNotFound,
                message: L[OtpCodeErrorCode.ReferenceNotFound],
                statusCode: HttpStatusCode.BadRequest);
        }

        public async Task<OtpCodeDto> SendAsync(SendOtpCodeDto dto)
        {
            var otpProvider = GetOtpProvider(dto.RecipientTypes);
            var reference = await GetReference(dto);

            // Validate
            if (dto.ReferenceTypes == ReferenceTypes.CustomerAccount.ToString())
            {
                CustomerAccount? customerAccount;
                if (reference is CustomerAccount ca)
                {
                    customerAccount = ca;
                    dto.ReferenceId = customerAccount.Id.ToString();
                }
                else
                {
                    throw new GlobalException(code: OtpCodeErrorCode.ReferenceNotFound,
                        message: L[OtpCodeErrorCode.ReferenceNotFound],
                        statusCode: HttpStatusCode.BadRequest);
                }

                // Check xem dto.purpose có hợp lệ không
                if (dto.Purpose != OtpPurposes.verify_account.ToString() && dto.Purpose != OtpPurposes.reset_password.ToString())
                {
                    throw new GlobalException(code: OtpCodeErrorCode.InvalidPurpose,
                        message: L[OtpCodeErrorCode.InvalidPurpose],
                        statusCode: HttpStatusCode.BadRequest);
                }
                // Check trạng thái
                if (dto.Purpose == OtpPurposes.verify_account.ToString() && customerAccount.Status != CustomerStatuses.Pending)
                {
                    throw new GlobalException(code: OtpCodeErrorCode.ValidationFailed,
                            message: L[OtpCodeErrorCode.InvalidPurpose],
                            statusCode: HttpStatusCode.BadRequest);
                }

                if (dto.Purpose == OtpPurposes.reset_password.ToString() && CustomerStatuses.Active != customerAccount.Status)
                {
                    throw new GlobalException(code: OtpCodeErrorCode.ValidationFailed,
                            message: L[OtpCodeErrorCode.InvalidPurpose],
                            statusCode: HttpStatusCode.BadRequest);
                }
            }
            else if (dto.ReferenceTypes == ReferenceTypes.StaffAccount.ToString())
            {
                User? technicalAccount;
                if (reference is User user)
                {
                    technicalAccount = user;
                    dto.ReferenceId = technicalAccount.Id.ToString();
                }
                else
                {
                    throw new GlobalException(code: OtpCodeErrorCode.ReferenceNotFound,
                        message: L[OtpCodeErrorCode.ReferenceNotFound],
                        statusCode: HttpStatusCode.BadRequest);
                }
                // Check xem dto.purpose có hợp lệ không
                if (dto.Purpose != OtpPurposes.reset_password.ToString())
                {
                    throw new GlobalException(code: OtpCodeErrorCode.InvalidPurpose,
                        message: L[OtpCodeErrorCode.InvalidPurpose],
                        statusCode: HttpStatusCode.BadRequest);
                }
                // Check trạng thái
                if (!technicalAccount.IsActive)
                {
                    throw new GlobalException(code: OtpCodeErrorCode.ValidationFailed,
                            message: L[OtpCodeErrorCode.ValidationFailed],
                            statusCode: HttpStatusCode.BadRequest);
                }
            }
            else
            {
                throw new GlobalException(code: OtpCodeErrorCode.ValidationFailed,
                        message: L[OtpCodeErrorCode.InvalidReferenceTypes],
                        statusCode: HttpStatusCode.BadRequest);
            }
            // Giới hạn 1 request trong 1 phút
            var oneMinuteAgo = DateTime.SpecifyKind(DateTime.UtcNow.AddMinutes(-1), DateTimeKind.Unspecified);
            var requestCount = await _unitOfWork.GenericRepository<OtpRequest>().CountAsync(x =>
                x.ReferenceId == dto.ReferenceId &&
                x.Recipient == dto.Recipient &&
                x.ReferenceTypes == dto.ReferenceTypes &&
                x.CreationTime >= oneMinuteAgo
            );
            if (requestCount > 0)
            {
                throw new GlobalException(code: OtpCodeErrorCode.OtpLimitExceeded,
                    message: L[OtpCodeErrorCode.OtpLimitExceeded],
                    statusCode: HttpStatusCode.TooManyRequests);
            }

            // Lưu log request vào OtpRequestLog
            var httpContext = _httpContextAccessor.HttpContext;
            var clientIp = httpContext?.Request?.Headers["X-Forwarded-For"].FirstOrDefault()
                            ?? httpContext?.Connection?.RemoteIpAddress?.ToString();
            var userAgent = httpContext?.Request?.Headers["User-Agent"].ToString();
            var otpRequestLog = new OtpRequest
            {
                ReferenceTypes = dto.ReferenceTypes,
                ReferenceId = dto.ReferenceId,
                Recipient = dto.Recipient,
                ClientIp = clientIp,
                UserAgent = userAgent,
            };
            await _unitOfWork.GenericRepository<OtpRequest>().AddAsync(otpRequestLog);
            await _unitOfWork.SaveChangeAsync();

            dto = await otpProvider.ValidateAsync(dto);


            // Lấy mã cuối cùng được gửi bởi phương thức
            var existingOtpCode = await _unitOfWork.GenericRepository<OtpCode>()
            .GetQueryable()
            .OrderByDescending(x => x.CreationTime)
            .FirstOrDefaultAsync(x =>
                x.ReferenceId == dto.ReferenceId &&
                x.RecipientTypes == dto.RecipientTypes &&
                x.Recipient == dto.Recipient &&
                x.Purpose == dto.Purpose &&
                x.ReferenceTypes == dto.ReferenceTypes);

            if (existingOtpCode != null)
            {
                var LockedUntil = existingOtpCode.LockedUntil.HasValue ? null : await otpProvider.GetLockedUntil(dto, reference);
                // Check to Lock 
                if (LockedUntil != null)
                {
                    existingOtpCode.LockedUntil = LockedUntil;
                    _unitOfWork.GenericRepository<OtpCode>().Update(existingOtpCode);
                    await _unitOfWork.SaveChangeAsync();
                }

                var currentTime = DateTime.UtcNow;
                if (existingOtpCode.LockedUntil.HasValue && existingOtpCode.LockedUntil.Value > currentTime)
                {
                    throw new GlobalException(code: OtpCodeErrorCode.OtpLocked,
                        message: L[OtpCodeErrorCode.OtpLocked],
                        statusCode: HttpStatusCode.Forbidden);
                }

            }

            var oldOtpCodes = await _unitOfWork.GenericRepository<OtpCode>().GetQueryable()
                .Where(x =>
                    x.ReferenceId == dto.ReferenceId &&
                    x.RecipientTypes == dto.RecipientTypes &&
                    x.Recipient == dto.Recipient &&
                    x.Purpose == dto.Purpose &&
                    x.ReferenceTypes == dto.ReferenceTypes &&
                    x.Status == OtpStatuses.Pending)
                .ToListAsync();

            if (oldOtpCodes.Any())
            {
                foreach (var oldOtp in oldOtpCodes)
                {
                    oldOtp.Status = OtpStatuses.Expired;
                    _unitOfWork.GenericRepository<OtpCode>().Update(oldOtp);
                }
                await _unitOfWork.SaveChangeAsync();
            }

            // Tạo mã OTP
            var code = RandomCodeHelper.GenerateRandomCode(0, 9, 6);

            var hash = HashHelper.Sha256(code);

            var otpCode = ObjectMapper.Map<SendOtpCodeDto, OtpCode>(dto);
            otpCode.Code = hash;
            otpCode.Status = OtpStatuses.Init;
            otpCode.Minutes = dto.Minutes;
            otpCode.ExpiresAt = DateTime.SpecifyKind(DateTime.UtcNow.AddMinutes(dto.Minutes), DateTimeKind.Unspecified);

            await _unitOfWork.GenericRepository<OtpCode>().AddAsync(otpCode);
            await _unitOfWork.SaveChangeAsync();

            // send
            var log = await otpProvider.SendAsync(code, otpCode);

            var otpProviderLog = ObjectMapper.Map<OtpProviderLogDto, OtpProviderLog>(log);
            await _unitOfWork.GenericRepository<OtpProviderLog>().AddAsync(otpProviderLog);
            await _unitOfWork.SaveChangeAsync();

            if (!log.IsSuccessful)
            {
                throw new GlobalException(code: OtpCodeErrorCode.ProviderSendFailed,
                        message: L[OtpCodeErrorCode.ProviderSendFailed],
                        statusCode: HttpStatusCode.Forbidden);
            }

            otpCode.Status = OtpStatuses.Pending;
            await _unitOfWork.SaveChangeAsync();

            otpRequestLog.OtpCodeId = otpCode.Id;
            _unitOfWork.GenericRepository<OtpRequest>().Update(otpRequestLog);
            await _unitOfWork.SaveChangeAsync();

            return new OtpCodeDto
            {
                Id = otpCode.Id,
                ReferenceId = Guid.Parse(otpCode.ReferenceId),
                Attempts = otpCode.Attempts,
                ExpiresAt = otpCode.ExpiresAt,
                LockedUntil = null,
                Status = otpCode.Status,
                Code = otpProvider.ProviderName == RecipientTypes.dump.ToString() ? code : null,
            };
        }

        public async Task<VerifyOtpCodeResponseDto> VerifyAsync(VerifyOtpCodeDto dto)
        {
            var otpCode = await _unitOfWork.GenericRepository<OtpCode>().GetAsync(x =>
                    x.ReferenceId == dto.ReferenceId &&
                    x.RecipientTypes == dto.RecipientTypes &&
                    x.Recipient == dto.Recipient &&
                    x.Purpose == dto.Purpose &&
                    x.ReferenceTypes == dto.ReferenceTypes &&
                    x.Status == OtpStatuses.Pending);

            if (otpCode == null || string.IsNullOrWhiteSpace(dto.Code))
            {
                throw new GlobalException(code: OtpCodeErrorCode.OtpInvalid,
                    message: L[OtpCodeErrorCode.OtpInvalid],
                    statusCode: HttpStatusCode.BadRequest);
            }

            // Check xem mã OTP đã bị khóa chưa
            if (otpCode.MaxAttempts <= otpCode.Attempts)
            {
                otpCode.LockedUntil = DateTime.SpecifyKind(DateTime.UtcNow.AddMinutes(10), DateTimeKind.Unspecified);
                _unitOfWork.GenericRepository<OtpCode>().Update(otpCode);
                await _unitOfWork.SaveChangeAsync();

                throw new GlobalException(code: OtpCodeErrorCode.OtpLocked,
                    message: L[OtpCodeErrorCode.OtpLocked],
                    statusCode: HttpStatusCode.Forbidden);
            }

            otpCode.Attempts += 1; // Tăng số lần thử

            // Check xem mã OTP đúng không
            if (!HashHelper.VerifySha256(dto.Code, otpCode.Code))
            {
                _unitOfWork.GenericRepository<OtpCode>().Update(otpCode);
                await _unitOfWork.SaveChangeAsync();

                throw new GlobalException(code: OtpCodeErrorCode.OtpInvalid,
                    message: L[OtpCodeErrorCode.OtpInvalid],
                    statusCode: HttpStatusCode.BadRequest);
            }

            // Check xem mã OTP đã hết hạn chưa
            var currentTime = DateTime.UtcNow;
            if (currentTime > otpCode.ExpiresAt)
            {
                otpCode.Status = OtpStatuses.Expired;
                _unitOfWork.GenericRepository<OtpCode>().Update(otpCode);
                await _unitOfWork.SaveChangeAsync();

                throw new GlobalException(code: OtpCodeErrorCode.OtpExpired,
                    message: L[OtpCodeErrorCode.OtpExpired],
                    statusCode: HttpStatusCode.BadRequest);
            }

            // Cập nhật trạng thái mã OTP
            otpCode.Status = OtpStatuses.Verified;

            var payload = new OtpTokenDto
            {
                TokenCodeId = otpCode.Id,
                ReferenceId = otpCode.ReferenceId,
                ReferenceTypes = otpCode.ReferenceTypes,
                RecipientTypes = otpCode.RecipientTypes,
                Recipient = otpCode.Recipient,
                Purpose = otpCode.Purpose,
                ExpiresIn = 600
            };
            var secretKey = _configuration["Jwt:Key"]!;
            var token = GenerateOtpJwtToken(payload, secretKey, out DateTime expireAt);

            _unitOfWork.GenericRepository<OtpCode>().Update(otpCode);
            await _unitOfWork.SaveChangeAsync();

            return new VerifyOtpCodeResponseDto
            {
                Success = true,
                Token = token,
                ExpireAt = expireAt
            };
        }

        private static string GenerateOtpJwtToken(OtpTokenDto payload, string secretKey, out DateTime expireAt)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            expireAt = now.AddSeconds(payload.ExpiresIn);

            var claims = new List<Claim>
        {
            new Claim("tokenCodeId", payload.TokenCodeId.ToString()),
            new Claim("referenceId", payload.ReferenceId ?? ""),
            new Claim("ReferenceTypes", payload.ReferenceTypes),
            new Claim("RecipientTypes", payload.RecipientTypes),
            new Claim("recipient", payload.Recipient),
            new Claim("type", payload.Type),
            new Claim("purpose", payload.Purpose)
        };

            var token = new JwtSecurityToken(
                claims: claims,
                notBefore: now,
                expires: expireAt,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
