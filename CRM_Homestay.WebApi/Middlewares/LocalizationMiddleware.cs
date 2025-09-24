using System.Globalization;

namespace CRM_Homestay.App.Middlewares;

/// <summary>
/// LocalizationMiddleware
/// </summary>
public class LocalizationMiddleware
{
    private readonly RequestDelegate _next;
    /// <summary>
    /// LocalizationMiddleware init
    /// </summary>
    /// <param name="next"></param>
    public LocalizationMiddleware(RequestDelegate next)
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
        var cultureKey = context.Request.Headers["Accept-Language"];
        if (!string.IsNullOrEmpty(cultureKey))
        {
            if (DoesCultureExist(cultureKey!))
            {
                var culture = new CultureInfo(cultureKey!);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
        }
        await _next(context);
    }
    private static bool DoesCultureExist(string cultureName)
    {
        return GlobalSetting.GetCultureCodes().Any(x => x.ToLower() == cultureName.ToLower());
    }
}