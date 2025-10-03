using System.Net;
using System.Text.RegularExpressions;
using CRM_Homestay.Contract.OtpCodes;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Entity.Otps;
using CRM_Homestay.Localization;
using CRM_Homestay.Service.Helpers;
using Microsoft.Extensions.Configuration;

namespace CRM_Homestay.Service.OtpCodes.Providers
{
    public class EmailOtpProvider : BaseOtpProvider, IOtpProvider
    {
        public string ProviderName => RecipientTypes.email.ToString();

        public EmailOtpProvider(ILocalizer l, IConfiguration configuration) : base(l, configuration)
        {
        }
        public async Task<OtpProviderLogDto> SendAsync(string code, OtpCode otpCode)
        {
            var recipient = otpCode.Recipient;
            var emailClient = new SendingEmail(new EmailHostConfig
            {
                From = _configuration["MailConfiguration:From"],
                Host = _configuration["MailConfiguration:Host"],
                Port = int.Parse(_configuration["MailConfiguration:Port"]!),
                Sender = _configuration["MailConfiguration:DisplayName"],
                UserName = _configuration["MailConfiguration:UserName"],
                Password = _configuration["MailConfiguration:Password"],
            });

            var content = new EmailSendingContent
            {
                Body = $"Mã xác minh của bạn là: {code}",
                Subject = "Mã xác minh",
                ToEmails = [recipient]

            };
            var log = new OtpProviderLogDto
            {
                OtpCodeId = otpCode.Id,
                ProviderName = ProviderName,
                RequestPayload = $"To: {recipient}\nSubject: Mã xác minh\nBody: Mã xác minh của bạn là: {code}",
                StatusCode = 200,
            };
            try
            {
                var result = await emailClient.SendEmailOtp(content);
                log.IsSuccessful = result.Item1;
                log.ResponsePayload = result.Item1 ? "Email sent successfully" : null;
                log.ErrorMessage = result.Item1 ? null : result.Item2;
                log.StatusCode = result.Item1 ? 200 : 500;
            }
            catch (Exception err)
            {
                log.IsSuccessful = false;
                log.ResponsePayload = err.ToString();
                log.ErrorMessage = err.ToString();
                log.StatusCode = 500;
            }

            return log;
        }

        public Task<SendOtpCodeDto> ValidateAsync(SendOtpCodeDto dto)
        {
            // Check email có hợp lệ không
            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (string.IsNullOrWhiteSpace(dto.Recipient) || !Regex.IsMatch(dto.Recipient, emailRegex))
            {
                throw new GlobalException(code: OtpCodeErrorCode.InvalidEmail,
                    message: L[OtpCodeErrorCode.InvalidEmail],
                    statusCode: HttpStatusCode.BadRequest);
            }
            return Task.FromResult(dto);
        }

        public Task<bool> CanSendAsync(SendOtpCodeDto dto, object reference)
        {
            return Task.FromResult(true);
        }
        public string GetRecipientField(SendOtpCodeDto dto)
        {
            return "Email";
        }

        public Task<DateTime?> GetLockedUntil(SendOtpCodeDto dto, object reference)
        {
            return Task.FromResult<DateTime?>(null);
        }
    }
}