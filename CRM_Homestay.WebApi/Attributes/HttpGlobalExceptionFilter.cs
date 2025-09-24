using CRM_Homestay.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Net;
using WatchDog;

namespace CRM_Homestay.App.Attributes
{
    /// <summary>
    /// HttpGlobalExceptionFilter
    /// </summary>
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;

        /// <summary>
        /// HttpGlobalExceptionFilter
        /// </summary>
        /// <param name="logger"></param>
        public HttpGlobalExceptionFilter(ILogger<HttpGlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// System exception
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var stackTrace = exception.StackTrace;
            string path = context.HttpContext.Request.Path.Value!;
            var developerMessage = "";
            while (exception.InnerException != null)
            {
                developerMessage += "\r\n--------------------------------------------------\r\n";
                exception = exception.InnerException;
                developerMessage += exception;
            }

            _logger.LogError(new EventId(context.Exception.HResult),
            context.Exception,
            developerMessage);

            WatchLogger.LogError(JsonConvert.SerializeObject(new { message = exception.Message, stackTrace = stackTrace, path = path }));

            if (context.ModelState.ErrorCount > 0)
            {
                var errors = context.ModelState.Where(v => v.Value!.Errors.Count > 0)
                    .ToDictionary(
                        kvp => $"{char.ToLower(kvp.Key[0])}{kvp.Key.Substring(1)}",
                        kvp => kvp.Value!.Errors.FirstOrDefault()?.ErrorMessage
                    );

                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                context.Result = new UnprocessableEntityObjectResult(new JsonResponse
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                });
                context.ExceptionHandled = true;
                return;
            }

            var json = new JsonResponse
            {
                Message = context.Exception.Message
            };

            var userName = context.HttpContext.User.Identity!.IsAuthenticated
                ? context.HttpContext.User.Identity.Name : "Guest"; //Gets user Name from user Identity 
            // 400 Bad Request
            if (context.Exception.GetType() == typeof(GlobalException))
            {
                var responseObj = new FluentResponse
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                };

                var statusCode = exception.Data[GlobalException.StatusCode];
                if (statusCode != null)
                {
                    responseObj.StatusCode = (int)statusCode;
                    context.HttpContext.Response.StatusCode = (int)statusCode;

                }
                else
                {
                    responseObj.StatusCode = StatusCodes.Status400BadRequest;
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }

                if (exception.Data[GlobalException.ErrorCode] is string code && !string.IsNullOrWhiteSpace(code))
                {
                    responseObj.Code = code;
                }

                if (exception.Data[GlobalException.ErrorList] is List<ValidationErrorItem> errorStrings)
                {
                    responseObj.Errors = errorStrings
                        .Select(msg => new ValidationErrorItem { Key = msg.Key, Value = msg.Value })
                        .ToList();
                }

                responseObj.Message = context.Exception.Message;

                context.Result = new BadRequestObjectResult(responseObj);
            }
            // 404 Not Found
            else if (context.Exception.GetType() == typeof(NotFoundException))
            {
                json.StatusCode = StatusCodes.Status404NotFound;
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Result = new NotFoundObjectResult(json);
            }
            // 500 Internal Server Error
            else
            {
                //json.Message = "Lỗi hệ thống";
                json.MessageDevelopment = developerMessage;
                json.StatusCode = StatusCodes.Status500InternalServerError;
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Result = new InternalServerErrorObjectResult(json);
            }
            context.ExceptionHandled = true;
        }
    }
}