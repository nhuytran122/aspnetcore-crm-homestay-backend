using CRM_Homestay.Contract.Validations;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Localization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace CRM_Homestay.App.Attributes
{
    /// <summary>
    /// CustomFluentResponse
    /// </summary>
    public class CustomFluentResponse
    {
        /// <summary>
        /// FluentValidationResponse
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        public static IActionResult FluentValidationResponse(ActionContext actionContext)
        {
            var modelType = actionContext.ActionDescriptor.Parameters
                .FirstOrDefault(p => p.ParameterType.GetInterfaces().Contains(typeof(IForceOverrideValidationMessage)))
                ?.ParameterType;

            var overrideMessage = modelType != null;

            var msgData = actionContext.ModelState
                    .Where(ms => ms.Value!.Errors.Any())
                    .Select(m => new
                    {
                        key = m.Key,
                        value = m.Value!.Errors.FirstOrDefault()!.ErrorMessage
                    })
                    .ToList();

            var responseObj = new FluentResponse
            {
                Success = false,
                StatusCode = (int)HttpStatusCode.BadRequest,
                Code = UserErrorCode.ValidationFailed,
            };

            // Nếu dto thuộc module customer account thì override message
            if (overrideMessage)
            {
                var localizer = actionContext.HttpContext.RequestServices.GetRequiredService<ILocalizer>();
                responseObj.Message = localizer[UserErrorCode.ValidationFailed].Value;
            }
            else
            {
                responseObj.Message = JsonConvert.SerializeObject(msgData);
            }

            responseObj.Errors = msgData.Select(m => new ValidationErrorItem
            {
                Key = m.key,
                Value = m.value
            }).ToList();

            var responseContext = new BadRequestObjectResult(responseObj)
            {
                StatusCode = (int)HttpStatusCode.BadRequest
            };
            responseContext.ContentTypes.Add("application/json");

            return responseContext;
        }
    }

    /// <summary>
    /// FluentResponse
    /// </summary>
    public class FluentResponse
    {
        /// <summary>
        /// Success
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// StatusCode
        /// </summary>
        public int StatusCode { get; set; } = (int)HttpStatusCode.UnprocessableEntity;
        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; } = string.Empty;
        /// <summary>
        /// Message
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// Errors
        /// </summary>
        public List<ValidationErrorItem> Errors { get; set; } = new();
    }

    /// <summary>
    /// ValidationErrorItem
    /// </summary>
    public class ValidationErrorItem
    {
        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; } = string.Empty;
        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; } = string.Empty;
    }
}