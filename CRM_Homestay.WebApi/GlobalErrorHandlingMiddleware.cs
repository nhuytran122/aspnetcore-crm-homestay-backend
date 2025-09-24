using Newtonsoft.Json;
using System.Net;
using WatchDog;

namespace CRM_Homestay.App;

/// <summary>
/// GlobalErrorHandlingMiddleware
/// </summary>
public class GlobalErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    /// <summary>
    /// GlobalErrorHandlingMiddleware init
    /// </summary>
    /// <param name="next"></param>
    public GlobalErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invoke
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            string message = exception.Message;
            string stackTrace = exception.StackTrace!;
            string path = context.Request.Path.Value!;
            HttpStatusCode status = HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            var exceptionResult = JsonConvert.SerializeObject(new { message = message, path = path });
            WatchLogger.LogError(JsonConvert.SerializeObject(new { message = message, stackTrace = stackTrace, path = path }));
            await context.Response.WriteAsync(exceptionResult);
        }
    }
}