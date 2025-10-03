using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using CRM_Homestay.Contract.OtpCodes;
using CRM_Homestay.Entity.Otps;
using CRM_Homestay.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CRM_Homestay.Database.Repositories;
using System.Net;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Consts;
using CRM_Homestay.Entity.SystemSettings;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Exceptions;

namespace CRM_Homestay.Service.OtpCodes.Providers
{
    public class ZaloOtpProvider : BaseOtpProvider, IOtpProvider
    {
        public const string DEVELOPMENT = "development";
        public const string PRODUCTION = "production";

        public const int MINUTES = 2;
        private const int TOKEN_BUFFER_SECONDS = 120; // Buffer để tránh token hết hạn

        // Implement the Code property
        public string ProviderName => RecipientTypes.phone.ToString();

        private IUnitOfWork _unitOfWork;

        private List<SystemSetting> _configs = new List<SystemSetting>();

        private string _development = "";
        private string _templateId = "";

        private string _adminPhone = "";
        private string? _currentAccessToken = null;

        public ZaloOtpProvider(ILocalizer l, IConfiguration configuration, IUnitOfWork unitOfWork) : base(l, configuration)
        {
            _unitOfWork = unitOfWork;
        }

        protected async Task Init()
        {
            _configs = await _unitOfWork.GenericRepository<SystemSetting>().GetQueryable()
                .Where(x => x.SystemName == ConfigKey.ZaloOAConfigKey)
                .ToListAsync();
            _development = (await GetOrCreateConfig(ZaloSystemConfigKeys.DEVELOPMENT_MODE.ToString())).ConfigValue;

            _templateId = (await GetOrCreateConfig(ZaloSystemConfigKeys.TEMPLATE_ID.ToString())).ConfigValue;

            _adminPhone = (await GetOrCreateConfig(ZaloSystemConfigKeys.ADMIN_PHONE.ToString())).ConfigValue;
        }

        protected async Task<SystemSetting> GetOrCreateConfig(string key)
        {
            var setting = _configs.FirstOrDefault(x => x.ConfigKey == key);
            var repo = _unitOfWork.GenericRepository<SystemSetting>();
            if (setting == null)
            {
                setting = new SystemSetting
                {
                    SystemName = ConfigKey.ZaloOAConfigKey,
                    ConfigKey = key,
                    ConfigValue = ""
                };
                await repo.AddAsync(setting);
                _unitOfWork.SaveChange();
                _configs.Add(setting); // để tránh tạo lại sau
            }
            return setting;
        }
        // Implement the Send method
        public async Task<OtpProviderLogDto> SendAsync(string code, OtpCode otpCode)
        {
            var client = new HttpClient();
            var log = new OtpProviderLogDto()
            {
                OtpCodeId = otpCode.Id,
                StatusCode = 500,
                RequestPayload = "",
                ResponsePayload = "",
                IsSuccessful = false,
                ProviderName = ProviderName,
            };

            try
            {
                var request = MakeHttpRequestMessage(HttpMethod.Post);
                var phone = _development == DEVELOPMENT ? _adminPhone : otpCode.Recipient;

                phone = phone.Trim();

                if (phone.StartsWith("0"))
                    phone = "84" + phone.Substring(1);

                var payload = JsonSerializer.Serialize(new
                {
                    mode = _development,
                    phone,
                    template_id = _templateId,
                    template_data = new { otp = code, minute = otpCode.Minutes },
                    tracking_id = otpCode.Id
                });

                // Tạo StringContent từ JSON
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                request.Content = content;

                var response = await client.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(responseString);
                var root = json.RootElement;

                var error = root.GetProperty("error").GetInt32();
                var isSuccessful = error == 0;
                // Nếu token invalid (-124) thì refresh và retry 1 lần
                if (error == -124)
                {
                    _currentAccessToken = await GetAccessToken(forceRefresh: true);
                    request = MakeHttpRequestMessage(HttpMethod.Post);
                    request.Content = content;

                    response = await client.SendAsync(request);
                    responseString = await response.Content.ReadAsStringAsync();
                    json = JsonDocument.Parse(responseString);
                    root = json.RootElement;

                    error = root.GetProperty("error").GetInt32();
                    isSuccessful = error == 0;
                }
                log.IsSuccessful = isSuccessful;
                log.RequestPayload = payload;
                log.StatusCode = isSuccessful ? 200 : 500;
                log.ResponsePayload = responseString;
                log.ErrorMessage = root.GetProperty("message").GetString();
            }
            catch (Exception e)
            {
                log.ResponsePayload = e.Message;
                log.ErrorMessage = "Error";
            }
            return log;
        }

        // Implement the Validate method
        public async Task<SendOtpCodeDto> ValidateAsync(SendOtpCodeDto dto)
        {
            if (!Regex.IsMatch(dto.Recipient, @"^0\d{9,10}$"))
            {
                throw new GlobalException(code: OtpProviderErrorCode.InvalidPhoneNumberFormat,
                            message: L[OtpProviderErrorCode.InvalidPhoneNumberFormat],
                            statusCode: HttpStatusCode.BadRequest);
            }

            await Init();

            if (string.IsNullOrEmpty(_templateId))
            {
                throw new GlobalException(code: OtpProviderErrorCode.TemplateIdNotProvided,
                            message: L[OtpProviderErrorCode.TemplateIdNotProvided],
                            statusCode: HttpStatusCode.BadRequest);
            }

            if (_development != DEVELOPMENT && _development != PRODUCTION)
            {
                throw new GlobalException(code: OtpProviderErrorCode.InvalidMode,
                            message: L[OtpProviderErrorCode.InvalidMode],
                            statusCode: HttpStatusCode.BadRequest);
            }

            if (_development != DEVELOPMENT && string.IsNullOrEmpty(_adminPhone))
            {
                throw new GlobalException(code: OtpProviderErrorCode.AdminPhoneNotProvided,
                            message: L[OtpProviderErrorCode.AdminPhoneNotProvided],
                            statusCode: HttpStatusCode.BadRequest);
            }

            _currentAccessToken = await GetAccessToken();

            dto.Minutes = MINUTES;

            return dto;
        }

        public string GetRecipientField(SendOtpCodeDto dto)
        {
            return "PhoneNumber";
        }

        // Implement the GetLockedUntil method
        public async Task<DateTime?> GetLockedUntil(SendOtpCodeDto dto, object reference)
        {
            var count = await _unitOfWork.GenericRepository<OtpProviderLog>().GetQueryable()
            .Include(x => x.OtpCode)
            .Where(x => x.OtpCode != null && x.OtpCode.RecipientTypes == ProviderName && x.OtpCode.Recipient == dto.Recipient)
            .Where(x => x.StatusCode == 200)
            .CountAsync();

            var countToLock = (await GetOrCreateConfig(ZaloSystemConfigKeys.COUNT_TO_LOCK.ToString())).ConfigValue;
            int maxCount = 4;
            if (!string.IsNullOrEmpty(countToLock))
            {
                maxCount = int.Parse(countToLock);
            }

            if (count % maxCount == 0)
            {
                var totalLockDays = (await GetOrCreateConfig(ZaloSystemConfigKeys.TOTAL_DAYS_TO_LOCK.ToString())).ConfigValue;
                double daysToLock = 7;
                if (!string.IsNullOrWhiteSpace(totalLockDays))
                {
                    daysToLock = double.Parse(totalLockDays);
                }
                return DateTime.UtcNow.AddDays(daysToLock);
            }
            return null;
        }

        protected HttpRequestMessage MakeHttpRequestMessage(HttpMethod httpMethod)
        {
            var request = new HttpRequestMessage(httpMethod, "https://business.openapi.zalo.me/message/template");

            if (String.IsNullOrEmpty(_currentAccessToken))
            {
                throw new GlobalException(code: OtpProviderErrorCode.AccessTokenNotFound,
                            message: L[OtpProviderErrorCode.AccessTokenNotFound],
                            statusCode: HttpStatusCode.BadRequest);
            }
            request.Headers.Add("access_token", _currentAccessToken);
            return request;
        }

        protected async Task<string?> GetAccessToken(bool forceRefresh = false)
        {

            var repo = _unitOfWork.GenericRepository<SystemSetting>();
            var accessToken = await GetOrCreateConfig(ZaloSystemConfigKeys.ACCESS_TOKEN.ToString());
            var refreshToken = await GetOrCreateConfig(ZaloSystemConfigKeys.REFRESH_TOKEN.ToString());
            var appId = await GetOrCreateConfig(ZaloSystemConfigKeys.APP_ID.ToString());
            var secretKey = await GetOrCreateConfig(ZaloSystemConfigKeys.SECRET_KEY.ToString());
            var expiresIn = await GetOrCreateConfig(ZaloSystemConfigKeys.EXPIRES_IN.ToString());

            var now = DateTime.UtcNow;

            if (String.IsNullOrEmpty(refreshToken.ConfigValue) || String.IsNullOrEmpty(secretKey.ConfigValue) || String.IsNullOrEmpty(appId.ConfigValue))
                throw new GlobalException(
                    code: OtpProviderErrorCode.IncompleteZaloConfig,
                    message: L[OtpProviderErrorCode.IncompleteZaloConfig],
                    statusCode: HttpStatusCode.BadRequest
                );

            if (!forceRefresh &&
                    !String.IsNullOrEmpty(accessToken.ConfigValue) &&
                    !String.IsNullOrEmpty(expiresIn.ConfigValue) &&
                    DateTime.TryParse(expiresIn.ConfigValue, out var expireTime) &&
                    now.AddSeconds(TOKEN_BUFFER_SECONDS) < expireTime)
            {
                // Token is still valid
                return accessToken.ConfigValue;
            }

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.zaloapp.com/v4/oa/access_token");
            request.Headers.Add("secret_key", secretKey.ConfigValue);
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("app_id", appId.ConfigValue));
            collection.Add(new("grant_type", "refresh_token"));
            collection.Add(new("refresh_token", refreshToken.ConfigValue));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new GlobalException(code: OtpProviderErrorCode.AccessTokenNotFound,
                                message: L[OtpProviderErrorCode.AccessTokenNotFound],
                                statusCode: HttpStatusCode.BadRequest);

            var data = JsonDocument.Parse(responseString).RootElement;
            if (data.TryGetProperty("error", out var errorProp))
            {
                if (errorProp.GetDouble() != 0)
                    throw new GlobalException(responseString);
            }

            var newAccessToken = data.GetProperty("access_token").GetString();
            var newRefreshToken = data.GetProperty("refresh_token").GetString();
            var newExpiresIn = double.Parse(data.GetProperty("expires_in").GetString()!); // seconds
            var newExpiredAt = DateTime.UtcNow.AddSeconds(newExpiresIn);
            // Update DB
            accessToken.ConfigValue = newAccessToken!;
            refreshToken.ConfigValue = newRefreshToken!;
            expiresIn.ConfigValue = newExpiredAt.ToString("O");

            repo.Update(accessToken);
            repo.Update(refreshToken);
            repo.Update(expiresIn);
            await _unitOfWork.SaveChangeAsync();

            return newAccessToken;
        }
    }
}